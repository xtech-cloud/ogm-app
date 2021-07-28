@echo off
cd startkit
IF EXIST vs2019 DEL /Q/S vs2019
python ..\generator.py
cd ..

pause