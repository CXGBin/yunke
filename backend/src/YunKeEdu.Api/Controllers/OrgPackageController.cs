using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/org-package")]
public class OrgPackageController : ControllerBase
{
    private readonly OrgPackageService _service;
    public OrgPackageController(OrgPackageService service) => _service = service;

    [HttpGet("page")]
    public async Task<ApiResponse<PagedResult<PackageDto>>> Page([FromQuery] PageRequest req)
        => ApiResponse<PagedResult<PackageDto>>.Ok(await _service.GetPageAsync(req));

    [HttpGet("{id}")]
    public async Task<ApiResponse<PackageDto>> Get(long id)
        => ApiResponse<PackageDto>.Ok(await _service.GetByIdAsync(id));

    [HttpPost]
    public async Task<ApiResponse<long>> Create([FromBody] CreatePackageRequest req)
        => ApiResponse<long>.Ok(await _service.CreateAsync(req));

    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(long id, [FromBody] UpdatePackageRequest req)
    {
        await _service.UpdateAsync(id, req);
        return ApiResponse<bool>.Ok(true);
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(long id)
    {
        await _service.DeleteAsync(id);
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPost("{id}/feature")]
    public async Task<ApiResponse<bool>> AddFeature(long id, [FromBody] AddFeatureRequest req)
    {
        await _service.AddFeatureAsync(id, req);
        return ApiResponse<bool>.Ok(true);
    }

    [HttpDelete("{id}/feature/{featureCode}")]
    public async Task<ApiResponse<bool>> RemoveFeature(long id, string featureCode)
    {
        await _service.RemoveFeatureAsync(id, featureCode);
        return ApiResponse<bool>.Ok(true);
    }

    [HttpGet("compare")]
    public async Task<ApiResponse<PackageCompareDto>> Compare()
        => ApiResponse<PackageCompareDto>.Ok(await _service.CompareAsync());
}
