using Stockband.Domain.Common;

namespace Stockband.Application.Common.Attributes;

public class AllowRoleAttribute:Attribute
{
    public UserRole[] Roles { get; set; }

    public AllowRoleAttribute(params UserRole[] roles)
    {
        Roles = roles;
    }
}