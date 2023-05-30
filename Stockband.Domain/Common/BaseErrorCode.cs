namespace Stockband.Domain.Common;

public enum BaseErrorCode
{
    DefaultCode = 0,
    /// <summary>
    /// This code indicates that the Fluent Validation process returned some errors.
    /// </summary>
    FluentValidationCode = 1,
    /// <summary>
    /// This code signifies that the user has already been created.
    /// </summary>
    UserAlreadyCreated = 2,
    /// <summary>
    /// This code indicates that the user is unable to perform an operation when they are not 
    /// authorized or when the owner of a certain entity is invalid.
    /// </summary>
    UserUnauthorizedOperation = 3,
    /// <summary>
    /// This code indicates that the user could not be found.
    /// </summary>
    UserNotExists = 4,
    /// <summary>
    /// This code indicates that the project could not be found.
    /// </summary>
    ProjectNotExists = 5,
    /// <summary>
    /// This code indicates that the project has already been created
    /// </summary>
    ProjectAlreadyCreated = 6,
    /// <summary>
    /// This code signifies that a project member has already been created.
    /// </summary>
    ProjectMemberAlreadyCreated = 7,
    /// <summary>
    /// This code indicates that the user is unable to perform an operation due to a blocking 
    /// condition. For example:
    /// <br/>
    /// - Certain entity dependencies may block the operation or it may produce messy results.
    /// </summary>
    UserOperationRestricted = 8,
    /// <summary>
    /// This code indicates that the project member could not be found.
    /// </summary>
    ProjectMemberNotExists = 9,
}