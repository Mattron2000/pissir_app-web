namespace Backend.Models;

public class UserType
{
    public UserTypeEnum Name { get; set; }

    // Navigation property
    public ICollection<User> Users { get; set; }
}

public enum UserTypeEnum
{
    ADMIN,
    BASE,
    PREMIUM
}
