public class HolyShiftUser
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PasswordSalt { get; set; } 
    public string PasswordHash { get; set; } 

    public HolyShiftUser()
    {
        Id = string.Empty;
        UserName = string.Empty;
        Email = string.Empty;
        PasswordSalt = string.Empty;
        PasswordHash = string.Empty;
    }
}
