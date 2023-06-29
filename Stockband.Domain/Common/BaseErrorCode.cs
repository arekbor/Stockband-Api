namespace Stockband.Domain.Common;

public enum BaseErrorCode
{
    DefaultCode = 0,
    
    /// <summary>
    /// This code indicates that the Fluent validation process returned some errors.
    /// </summary>
    FluentValidationCode = 1,
    
    /// <summary>
    /// This code means that the user has already been created.
    /// </summary>
    UserAlreadyCreated = 2,
    
    /// <summary>
    /// This code indicates that the user is unable to perform an operation
    /// if he is not authorized or if the owner of a particular entity is invalid.
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
    /// This code indicates that the project has already been created.
    /// </summary>
    ProjectAlreadyCreated = 6,
    
    /// <summary>
    /// This code means that a project member has already been created.
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
    
    /// <summary>
    /// This code indicates that the member (user) could not be found.
    /// </summary>
    MemberForProjectMemberNotExists = 10,
    
    /// <summary>
    /// This code indicates that the limit of created projects (per user) has been exceeded
    /// </summary>
    ProjectsLimitPerUserExceeded = 11,
    
    /// <summary>
    /// This code indicates that the maximum number of created project members (per project) has been exceeded
    /// </summary>
    ProjectMembersLimitPerProjectExceeded = 12,
    
    /// <summary>
    /// This code indicates that the user has entered an incorrect email or password
    /// </summary>
    WrongEmailOrPasswordLogin = 13,
}