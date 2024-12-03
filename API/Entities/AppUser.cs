namespace API.Entities;

public class AppUser
{
    public int Id { get; set; }
    // Default value for int will be Zero

    public required String UserName { get; set; }
    // public String? Username { get; set; } for optional we use ?
    
 
}
