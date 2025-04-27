namespace Backend.Models;

public class UserType
{
    public UserTypeEnum Name { get; set; }
}

public enum UserTypeEnum
{
    ADMIN,
    BASE,
    PREMIUM
}
