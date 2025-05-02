namespace Backend.Models;

public partial class Price
{
    public string Type { get; set; } = null!;

    public decimal Amount { get; set; }

    public virtual PricesType TypeNavigation { get; set; } = null!;
}
