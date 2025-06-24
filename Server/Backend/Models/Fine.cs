namespace Backend.Models;

public partial class Fine
{
    public string Email { get; set; } = null!;

    public DateTime DatetimeStart { get; set; }

    public DateTime DatetimeEnd { get; set; }

    public int Kw { get; set; }

    public bool? Paid { get; set; }

    public virtual User EmailNavigation { get; set; } = null!;
}
