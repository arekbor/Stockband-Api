namespace Stockband.Api.Dtos.User;

public class UpdateUserDto
{
    public UpdateUserDto(int userId, string username, string email)
    {
        UserId = userId;
        Username = username;
        Email = email;
    }
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}