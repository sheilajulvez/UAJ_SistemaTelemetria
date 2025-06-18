# Análisis de Trazas - Práctica 3: Sistema de Telemetría

Este proyecto analiza los archivos de trazas generados por un videojuego instrumentado, calculando métricas como número de eventos, muertes, tiempo jugado, etc. Se entrega como parte de la **Práctica 3** de la asignatura *Usabilidad y análisis de juegos*.

---

## Requisitos

- Tener instalado Python 3.8 o superior
- Acceso a internet para instalar dependencias (solo la primera vez)

---

## Ejecución automática

1. **Coloca los archivos de trazas** (formato `.json`) dentro de la carpeta `trazas_json/`.

2. Cada .json es una sesión medida, es decir, solo puede haber un solo evento de SessionStart y SessionEnd por cada .json. No debes juntar varios json en uno solo.

3. **Si puedes ejecutar py en consola**  

4. **Haz doble clic** en el archivo `run_analysis_py.bat` o `run_analysis_python.bat` (según tengas configurado en tu ordenador (`py --version` o `python --version`)).

   Esto hará lo siguiente automáticamente:
   - Instalará las dependencias necesarias (como `pandas`).
   - Ejecutará el análisis sobre los archivos de trazas.

---

## Posibles errores y soluciones

### *“No se encontró Python” o “Python no se reconoce como un comando interno o externo...”*

**Solución:**
- Si has ejecutado `run_analysis_py.bat` prueba a ejecutar `run_analysis_python.bat` o viceversa.

- Si el problema persiste, asegúrate de tener Python instalado desde [https://www.python.org](https://www.python.org)
- Durante la instalación, activa la casilla **“Add Python to PATH”**

### config.json
Archivo de configuración sobre los valores del análisis. Debería hacerse un estudio para establecer estos valores, para así medir con precisión y de manera realista el resultado del análisis.

---
