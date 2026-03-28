using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drive.Models;
[Table("users")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int id { get; set; }

    [Required]
    [Column("name")]
    public string name { get; set; } = null!;

    [Required]
    [Column("email")]
    public string email { get; set; } = null!;

    [Required]
    [Column("pass")]
    public string pass { get; set; } = null!;

    private DateTime _birth;
    [Required]
    [Column("birth")]
    public DateTime birth 
    { 
        get => _birth; 
        set => _birth = DateTime.SpecifyKind(value, DateTimeKind.Utc); 
    }
    
    [Required]
    [Column("roll")]
    public string roll { get; set; } = null!;

    public static string GetHash(string input)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hashedBytes = MD5.HashData(inputBytes);
        return BitConverter.ToString(hashedBytes);
    }
}

public class UserCredentials
{
    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string pass { get; set; } = null!;
}

public class CreateUser
{
    [Required]
    public string name { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "La dirección no pertenece a un dirección de correo válida")]
    [Required(ErrorMessage = "El campo es obligatorio")]
    public string Email { get; set; } = string.Empty;

    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [Required]
    public DateTime birth { get; set; }

    [DataType(DataType.Password)]
    [Required]
    public string pass { get; set; } = string.Empty;

    [DataType(DataType.Password )]
    [Required]
    [Compare("pass", ErrorMessage = "Las contraseñas no coinciden")]
    [DisplayName("Password Confirm")]
    public string PasswordConfirm { get; set; } = string.Empty;
}