using TaskManagement.Application.Models;

namespace TaskManagement.Application.Services;

public interface IExpEstimatorService
{
    Task<ApiResult<int>> EstimateExpAsync(string taskName, string? description = null);
}