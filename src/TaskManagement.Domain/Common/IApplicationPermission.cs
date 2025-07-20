namespace TaskManagement.Domain.Common;

public interface IApplicationPermission<TModulePermissionGroup>
where TModulePermissionGroup : class, IApplicationPermissionGroup
{
    string ShortName { get; set; }
    string FullName { get; set; }
    TModulePermissionGroup PermissionGroup { get; set; }
}
