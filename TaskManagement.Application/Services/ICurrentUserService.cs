using TaskManagement.Application.Security;

namespace TaskManagement.Application.Services;

public interface ICurrentUserService
{
    int? UserId { get; }
    IUser? User { get; }
}