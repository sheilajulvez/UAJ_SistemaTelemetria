import pandas as pd
import json
import os

def cargar_multiples_trazas(carpeta):
    dataframes = []
    for archivo in os.listdir(carpeta):
        if archivo.endswith('.json') or archivo.endswith('.jsonl'):
            ruta = os.path.join(carpeta, archivo)
            with open(ruta, 'r', encoding='utf-8') as file:
                lineas = file.readlines()
                datos = [json.loads(linea) for linea in lineas]
                df = pd.DataFrame(datos)
                df['archivo'] = archivo
                dataframes.append(df)
    return pd.concat(dataframes, ignore_index=True)

def analizar(df):
    df['event_timestamp'] = pd.to_datetime(df['event_timestamp'], errors='coerce', utc=True)

    print("🎮 Conteo de eventos:")
    conteos = df['event_type'].value_counts()
    print(conteos)
    conteos.to_csv("eventos_conteo.csv")

    saltos = df[df['event_type'] == 'Jump']
    print(f"\nTotal de saltos: {len(saltos)}")

    muertes = df[df['event_type'] == 'Death']
    print(f"\nTotal de muertes: {len(muertes)}")
    if not muertes.empty and 'death_type' in muertes:
        print(muertes['death_type'].value_counts())

    slimes = df[df['event_type'] == 'BlueSlime']
    print(f"\nColisiones con BlueSlime: {len(slimes)}")

    level_end = df[df['event_type'] == 'LevelEnd'].copy()
    if 'time' in level_end:
        level_end['tiempo_nivel_segundos'] = (
            level_end['time'].str.replace(",", ".", regex=False).astype(float)
        )
        print("\n⏱️ Duración por nivel:")
        print(level_end[['level_id', 'tiempo_nivel_segundos']].dropna())
        level_end[['level_id', 'tiempo_nivel_segundos']].to_csv("niveles_duracion.csv", index=False)

    total_tiempo = (df['event_timestamp'].max() - df['event_timestamp'].min()).total_seconds()
    print(f"\n🕒 Tiempo total jugado: {total_tiempo:.2f} segundos")

if __name__ == "__main__":
    carpeta = "trazas_json"
    df = cargar_multiples_trazas(carpeta)
    analizar(df)