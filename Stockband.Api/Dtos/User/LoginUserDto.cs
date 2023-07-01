namespace Stockband.Api.Dtos.User;

public class LoginUserDto
{
    public LoginUserDto(string email, string password)
    {
        Email = email;
        Password = password;
    }
    public string Email { get; set; }
    public string Password { get; set; }
}