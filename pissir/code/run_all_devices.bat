@echo off

cd ..\backend
:: Run backend jar
start java -jar .\build\libs\backend.jar

cd ..\monitor
:: Run monitor jar
start java -jar .\build\libs\monitor.jar true

cd ..\sensor
start java -jar .\build\libs\sensor.jar 1 FREE true
start java -jar .\build\libs\sensor.jar 2 FREE true
start java -jar .\build\libs\sensor.jar 3 FREE true
start java -jar .\build\libs\sensor.jar 4 FREE true
start java -jar .\build\libs\sensor.jar 5 FREE true
start java -jar .\build\libs\sensor.jar 6 FREE true
start java -jar .\build\libs\sensor.jar 7 FREE true
start java -jar .\build\libs\sensor.jar 8 FREE true
start java -jar .\build\libs\sensor.jar 9 FREE true
start java -jar .\build\libs\sensor.jar 10 FREE true

cd ..\mwbot
:: Run mwbot jar
start java -jar .\build\libs\mwbot.jar true

:: Run frontend server
cd ..\frontend
start node server.js
cd ..

:: Wait for all background processes to finish
timeout /t -1
