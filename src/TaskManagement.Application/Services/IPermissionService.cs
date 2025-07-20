using TaskManagement.Application.Models;
using TaskManagement.Application.Models.Permissions;
using TaskManagement.Application.Security;
using TaskManagement.Application.Security.AuthEnums;

namespace TaskManagement.Application.Services;

public interface IPermissionService
{
    List<PermissionCodeDescription> GetAllPermissionDescriptions();
    string GetPermissionShortName(ApplicationPermissionCode permissionCode);
    Task<ApiResult<List<PermissionGroupListModel>>> GetPermissionsFromDbAsync();
}
