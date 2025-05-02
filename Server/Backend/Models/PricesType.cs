namespace Backend.Models;

public partial class PricesType
{
    public string Name { get; set; } = null!;

    public virtual Price? Price { get; set; }
}

public enum PricesTypeEnum
{
    PARKING,
    CHARGING
}
