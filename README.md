# PISSIR project + ASP.NET Core

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
