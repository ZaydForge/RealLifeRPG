using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Filters;
using TaskManagement.Application.Security.AuthEnums;

namespace TaskManagement.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizeAttribute : TypeFilterAttribute
    {
        public AuthorizeAttribute()
        : base(typeof(AuthorizeFilter))
        {
            Arguments = [Array.Empty<ApplicationPermissionCode>()];
        }

        public AuthorizeAttribute(params ApplicationPermissionCode[] permissionCodes)
            : base(typeof(AuthorizeFilter))
        {
            Arguments = [permissionCodes];
        }

    }
}
