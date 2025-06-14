import pandas as pd
import json
import os

def cargar_multiples_trazas(carpeta):
    dataframes = []
    session_id = 1 

    for archivo in os.listdir(carpeta):
        if archivo.endswith('.json') or archivo.endswith('.jsonl'):
            ruta = os.path.join(carpeta, archivo)
            with open(ruta, 'r', encoding='utf-8') as file:
                lineas = file.readlines()
                datos = [json.loads(linea) for linea in lineas]
                df = pd.DataFrame(datos)
                df['archivo'] = archivo
                df['session_id'] = session_id
                dataframes.append(df)
                session_id += 1
    return pd.concat(dataframes, ignore_index=True)

def analizar(df):
    print(df)
    df['event_timestamp'] = pd.to_datetime(df['event_timestamp'], errors='coerce', utc=True)

    print("Conteo de eventos:")
    conteos = df['event_type'].value_counts()
    print(conteos)
    conteos.to_csv("eventos_conteo.csv")


    numero_de_sesiones = len(df[df['event_type'] == 'SessionStart'])


    saltos = df[df['event_type'] == 'Jump']
    print(f"\nTotal de saltos: {len(saltos)}")
    if (len(saltos) / numero_de_sesiones) < config["min_saltos"]:
        print('Pocos saltos: puede indicar falta de interacciones.')
    elif (len(saltos) / numero_de_sesiones) > config["max_saltos"]:
        print('Muchos saltos: jugador activo o nivel largo.')
    else:
        print('Cantidad moderada de saltos: ritmo de juego equilibrado.')


    muertes = df[df['event_type'] == 'Death']
    print(f"\nTotal de muertes: {len(muertes)}")
    if not muertes.empty and 'death_type' in muertes:
        print(muertes['death_type'].value_counts())
    if len(muertes) == 0:
        print('¡No hubo muertes! El nivel puede ser demasiado fácil.')
    elif muertes['death_type'].value_counts().max() > 10:
        print('Hay una causa de muerte predominante que conviene revisar.')
    else:
        print('Distribución equilibrada de causas de muerte.')



    level_end = df[df['event_type'] == 'LevelEnd'].copy()
    if not level_end.empty:
        if 'time' in level_end:
            level_end['tiempo_nivel_segundos'] = (
                level_end['time'].str.replace(",", ".", regex=False).astype(float)
            )
            print("\nDuración por nivel:")
            print(level_end[['level_id', 'tiempo_nivel_segundos']].dropna())
            level_end[['level_id', 'tiempo_nivel_segundos']].to_csv("niveles_duracion.csv", index=False)
        if not level_end['tiempo_nivel_segundos'].dropna().empty:
            promedio = level_end['tiempo_nivel_segundos'].mean()
            print(f"Duración media por nivel: {promedio:.2f} segundos")
            if promedio < config["tiempo_nivel_rapido"]:
                print('🟢 Los niveles se están completando muy rápido.')
            elif promedio > config["tiempo_nivel_lento"]:
                print('🔴 Los niveles tardan demasiado en completarse.')
            else:
                print('🟡 Tiempo adecuado para mantener atención y reto.')
        else:
            print("No hay datos suficientes para calcular la duración promedio por nivel.")
    else:
        print("\n⛔ No se completó ni un solo nivel.")



    if numero_de_sesiones > 0:
        duraciones = []

        for session_id, grupo in df.groupby('session_id'):
            inicio = grupo['event_timestamp'].min()
            fin = grupo['event_timestamp'].max()
            duracion = (fin - inicio).total_seconds()
            duraciones.append(duracion)
            print(f"\nTiempo jugado en sesión {session_id}: {duracion:.2f} segundos")

        if duraciones:
            promedio = sum(duraciones) / len(duraciones)
            print(f"\nTiempo promedio por sesión: {promedio:.2f} segundos")
    else:
        print("\nNo hay datos válidos para calcular duración de sesiones.")


if __name__ == "__main__":
    with open("config.json", "r", encoding="utf-8") as f:
        config = json.load(f)
    carpeta = config["carpeta"]
    df = cargar_multiples_trazas(carpeta)
    analizar(df)