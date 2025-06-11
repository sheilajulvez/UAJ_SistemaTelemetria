@echo off
echo Instalando dependencias...
python -m pip install --upgrade pip
python -m pip install -r requirements.txt

echo Ejecutando analisis de trazas...
python main.py

pause

