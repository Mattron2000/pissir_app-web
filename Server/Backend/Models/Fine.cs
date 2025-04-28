namespace Backend.Models;

public class Fine
{
    public string UserEmail { get; set; }
    public DateTime DateTimeStart { get; set; }
    public DateTime DateTimeEnd { get; set; }
    public bool Paid { get; set; } = false;

    // Navigation property
    public User User { get; set; }
}
