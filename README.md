# PISSIR project + ASP.NET Core

## Prerequisities

- OpenJDK21 (to have gradlew working to build jar files)
- .NET 8.0 LTS (to have Blazor ASP.NET working)

## How to start

### Mosquitto

```bash
# Go to mosquitto folder
cd pissir/code/mosquitto

# start mosquitto broker
mosquitto -c broker.conf -v
```

### ASP.NET Core

```bash
dotnet run --project Server/Backend
```

### Debug

a web page, external from the rest of the system, used to check the IoT devices status

```bash
cd pissir/code

./gradlew build

java -jar debug/build/libs/debug.jar
```

### Monitor

An IoT device that count the number of free slots for users at entrance of the parking slot

```bash
cd pissir/code

./gradlew build

java -jar monitor/build/libs/monitor.jar <true|false>
```

### Sensor

Many IoT devices that send the occupational state the the corresponding slot

```bash
cd pissir/code

./gradlew build

java -jar sensor/build/libs/sensor.jar <slot_id> <"FREE"|"OCCUPIED"> <true|false>
```

### MWbot

An IoT device that receive charging orders and go to recharge the users' car

```bash
cd pissir/code

./gradlew build

java -jar mwbot/build/libs/mwbot.jar <true|false>
```

### Mobile

A IoT device that represent users' smartphones that have the only purpose to receive the notification of complete charge of own car

```bash
cd pissir/code

./gradlew build

java -jar mobile/build/libs/mobile.jar <phone_number> <true|false>
```
