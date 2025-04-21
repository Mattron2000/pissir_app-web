#!/bin/bash

# Run backend jar
java -jar ./backend/build/libs/backend.jar &

for i in {1..10}; do
    java -jar ./sensor/build/libs/sensor.jar "$i" FREE true &
    sleep 1  # Wait for 2 seconds before starting the next command
done

java -jar ./monitor/build/libs/monitor.jar true &
sleep 1
java -jar ./mwbot/build/libs/mwbot.jar true &

cd frontend
node server.js &
cd ..

wait  # Wait for all background processes to finish
