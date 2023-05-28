namespace Stockband.Domain.Common;

public enum BaseErrorCode
{
    DefaultCode = 0,
    FluentValidationCode = 1,
    UserAlreadyCreated = 2,
    UserUnauthorizedOperation = 3,
    UserNotExists = 4,
    ProjectNotExists = 5,
    ProjectAlreadyCreated = 6,
    ProjectMemberAlreadyCreated = 7,
    UserOperationRestricted = 8,
    ProjectMemberNotExists = 9
}