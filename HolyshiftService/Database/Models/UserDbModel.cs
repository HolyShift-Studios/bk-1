using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Users")]
public class UserDbModel
{
    [Key]
    [Column(TypeName = "char(36)")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(255)]
    [Column(TypeName = "varchar(255)")]
    public string UserName { get; set; } = string.Empty;

    [MaxLength(255)]
    [Column(TypeName = "varchar(255)")]
    public string Email { get; set; } = string.Empty;

    [MaxLength(64)]
    [Column(TypeName = "varchar(64)")]
    public string Salt { get; set; } = string.Empty;

    [MaxLength(64)]
    [Column(TypeName = "varchar(64)")]
    public string PasswordHash { get; set; } = string.Empty;
}
