namespace Backend.Models;

public partial class UsersType
{
    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

public enum UsersTypeEnum
{
    ADMIN,
    BASE,
    PREMIUM
}
