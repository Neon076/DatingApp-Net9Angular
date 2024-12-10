namespace API.Entities;

public class AppUser
{
    public int Id { get; set; }
    // Default value for int will be Zero

    public required String UserName { get; set; }
    // public String? Username { get; set; } for optional we use ?

    public byte[] PasswordHash { get; set; } = [];
    public byte[] PasswordSalt { get; set; } = [];
    public DateOnly DateOfBirth { get; set; }
    public required string KnownAs { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastActive { get; set; }
    public required string Gender { get; set; }
    public String? Introduction { get; set; }
    public string? LookingFor { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    public List<Photo> Photos { get; set; } = [];

    // public int GetAge(){
    //     return DateOfBirth.CalculateAge();
    // }
}
