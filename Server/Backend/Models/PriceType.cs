namespace Backend.Models;

public class PriceType
{
    public PriceTypeEnum Name { get; set; }
}

public enum PriceTypeEnum
{
    PARKING,
    CHARGING
}
