namespace Backend.Models;

public class PriceType
{
    public PriceTypeEnum Name { get; set; }

    // Navigation property
    public Price Price { get; set; }
}

public enum PriceTypeEnum
{
    PARKING,
    CHARGING
}
