import pandas as pd
import json
import os
import io
import sys
from docx import Document

# Para escribir en un buffer y luego volcarlo en el documento
buffer = io.StringIO()
sys.stdout = buffer


def cargar_multiples_trazas(carpeta):
    dataframes = []
    session_id = 1 

    for archivo in os.listdir(carpeta):
        if archivo.endswith('.json') or archivo.endswith('.jsonl'):
            ruta = os.path.join(carpeta, archivo)
            with open(ruta, 'r', encoding='utf-8') as file:
                lineas = file.readlines()
                datos = [json.loads(linea) for linea in lineas if linea.strip()]
                df = pd.DataFrame(datos)
                df['archivo'] = archivo
                df['session_id'] = session_id
                dataframes.append(df)
                session_id += 1
    return pd.concat(dataframes, ignore_index=True)

def analizar(df):
    df['event_timestamp'] = pd.to_datetime(df['event_timestamp'], errors='coerce', utc=True)

    print("Conteo de eventos:")
    conteos = df['event_type'].value_counts()
    print(conteos)

    print(f"\n---\n")


    numero_de_sesiones = len(df[df['event_type'] == 'SessionStart'])


    saltos = df[df['event_type'] == 'Jump']
    print(f"Total de saltos: {len(saltos)}")
    if (len(saltos) / numero_de_sesiones) < config["min_saltos"]:
        print('Pocos saltos: puede indicar falta de interacciones.')
    elif (len(saltos) / numero_de_sesiones) > config["max_saltos"]:
        print('Muchos saltos: jugador activo o nivel largo.')
    else:
        print('Cantidad moderada de saltos: ritmo de juego equilibrado.')


    print(f"\n---\n")


    muertes = df[df['event_type'] == 'Death']
    print(f"Total de muertes: {len(muertes)}")
    if not muertes.empty and 'death_type' in muertes:
        print(muertes['death_type'].value_counts())
    if len(muertes) == 0:
        print('¡No hubo muertes! El nivel puede ser demasiado fácil.')
    elif muertes['death_type'].value_counts().max() > config["max_muertes"]:
        print('Hay una causa de muerte predominante que conviene revisar.')
    else:
        print('Distribución equilibrada de causas de muerte.')


    print(f"\n---\n") 


    level_end = df[df['event_type'] == 'LevelEnd'].copy()

    if not level_end.empty:
        if 'session_id' in level_end.columns:
            tiempos_por_sesion = []

            for session_id, grupo in level_end.groupby('session_id'):
                grupo = grupo.copy()

                # Convertir level_time a float si es string
                if grupo['level_time'].dtype == 'object':
                    grupo['level_time'] = grupo['level_time'].str.replace(",", ".", regex=False).astype(float)

                if not grupo['level_time'].dropna().empty:
                    tiempo_total = grupo['level_time'].max()
                    niveles_completados = grupo.shape[0]
                    promedio_sesion = tiempo_total / niveles_completados
                    tiempos_por_sesion.append(promedio_sesion)

                    print(f"Sesión {session_id}: {niveles_completados} niveles, {tiempo_total:.2f}s totales → promedio: {promedio_sesion:.2f}s")

            if tiempos_por_sesion:
                promedio_general = sum(tiempos_por_sesion) / len(tiempos_por_sesion)
                print(f"\nDuración media por nivel: {promedio_general:.2f} segundos")

                if promedio_general < config["tiempo_nivel_rapido"]:
                    print('Los niveles se están completando muy rápido.')
                elif promedio_general > config["tiempo_nivel_lento"]:
                    print('Los niveles tardan demasiado en completarse.')
                else:
                    print('Tiempo adecuado para mantener atención y reto.')
            else:
                print("No hay datos suficientes para calcular el promedio general.")
        else:
            print("Falta la columna 'session_id' para calcular duración por sesión.")
    else:
        print("No se completó ni un solo nivel.")



    print(f"\n---\n")
          
    if numero_de_sesiones > 0:
        duraciones = []

        for session_id, grupo in df.groupby('session_id'):
            inicio = grupo['event_timestamp'].min()
            fin = grupo['event_timestamp'].max()
            duracion = (fin - inicio).total_seconds()
            duraciones.append(duracion)
            print(f"Tiempo jugado en sesión {session_id}: {duracion:.2f} segundos")

        if duraciones:
            promedio = sum(duraciones) / len(duraciones)
            print(f"Tiempo promedio por sesión: {promedio:.2f} segundos")
    else:
        print("No hay datos válidos para calcular duración de sesiones.")


if __name__ == "__main__":
    print(f"\n---\n")
    
    with open("config.json", "r", encoding="utf-8") as f:
        config = json.load(f)
    carpeta = config["carpeta"]
    df = cargar_multiples_trazas(carpeta)
    analizar(df)
    
    sys.stdout = sys.__stdout__
    contenido = buffer.getvalue()

    doc = Document()
    doc.add_heading("Informe automático de análisis – AMONRA", level=1)
    doc.add_paragraph(contenido)
    doc.save("informe_resultados.docx")

    print("\n✅ Se ha guardado un informe en 'informe_resultados.docx'\n")
