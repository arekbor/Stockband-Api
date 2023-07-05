namespace Stockband.Application.Interfaces.FeatureServices;

public interface IUserFeaturesService
{
    Task<bool> IsEmailAlreadyUsed(string email);
    string HashPassword(string password);
    bool VerifyHashedPassword(string hashedPassword, string password);
}