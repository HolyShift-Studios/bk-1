using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Users")]
public class UserDbModel
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(100)]
    public string UserName { get; set; } = default!;

    [MaxLength(100)]
    public string Email { get; set; } = default!;

    [MaxLength(25)] 
    public string Salt { get; set; } = default!;

    [MaxLength(45)]
    public string PasswordHash { get; set; } = default!;

    [MaxLength(128)]
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiration { get; set; }
    public UserRole Role { get; set; }
}

public enum UserRole
{
    Customer,
    Admin, 
    SuperAdmin 
}

