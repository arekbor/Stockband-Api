namespace Stockband.Api.Dtos.User;

public class RegisterUserDto
{
    public RegisterUserDto()
    {
        
    }
    public RegisterUserDto(string username, string email, string password, string confirmPassword)
    {
        Username = username;
        Email = email;
        Password = password;
        ConfirmPassword = confirmPassword;
    }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}