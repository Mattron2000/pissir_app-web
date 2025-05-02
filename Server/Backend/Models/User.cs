namespace Backend.Models;

public partial class User
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public virtual ICollection<Fine> Fines { get; set; } = new List<Fine>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual UsersType TypeNavigation { get; set; } = null!;
}
