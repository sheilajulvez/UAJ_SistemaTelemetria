@echo off
echo Instalando dependencias...
py -m pip install --upgrade pip
py -m pip install -r requirements.txt

echo Ejecutando analisis de trazas...
py main.py

pause

