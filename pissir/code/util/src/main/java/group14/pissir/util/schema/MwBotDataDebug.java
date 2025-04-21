package group14.pissir.util.schema;

import group14.pissir.util.MWbotStatus;

public record MwBotDataDebug(MWbotStatus status, Integer position, String model, Integer percentage) {}
