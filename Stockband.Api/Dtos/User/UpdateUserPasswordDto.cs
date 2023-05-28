namespace Stockband.Api.Dtos.User;

public class UpdateUserPasswordDto
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }
}