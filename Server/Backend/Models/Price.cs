namespace Backend.Models;

public class Price
{
    public PriceTypeEnum Type { get; set; }

    private double _amount;
    public double Amount
    {
        get => _amount;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
            _amount = value;
        }
    }

    // Navigation property
    public PriceType PriceType { get; set; }
}
