using MediatR;
using Stockband.Domain;

namespace Stockband.Application.Features.UserFeatures.Commands.UpdatePassword;

public class UpdatePasswordCommand:IRequest<BaseResponse>
{
    public UpdatePasswordCommand()
    {
        
    }
    
    public UpdatePasswordCommand(string currentPassword, string newPassword, string confirmNewPassword)
    {
        CurrentPassword = currentPassword;
        NewPassword = newPassword;
        ConfirmNewPassword = confirmNewPassword;
    }
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }
}