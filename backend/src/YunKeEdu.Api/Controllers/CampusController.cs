using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/campus")]
public class CampusController : ControllerBase
{
    private readonly CampusService _service;
    public CampusController(CampusService service) => _service = service;

    [HttpGet("list")]
    public async Task<ApiResponse<List<CampusDto>>> List()
        => ApiResponse<List<CampusDto>>.Ok(await _service.GetListAsync(GetUser().TenantId));

    [HttpGet("{id}")]
    public async Task<ApiResponse<CampusDto>> Get(long id)
        => ApiResponse<CampusDto>.Ok(await _service.GetByIdAsync(id, GetUser().TenantId));

    [HttpPost]
    public async Task<ApiResponse<long>> Create([FromBody] CreateCampusRequest req)
        => ApiResponse<long>.Ok(await _service.CreateAsync(req, GetUser()));

    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(long id, [FromBody] UpdateCampusRequest req)
    {
        await _service.UpdateAsync(id, req, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPut("{id}/status")]
    public async Task<ApiResponse<bool>> UpdateStatus(long id, [FromBody] UpdateStatusRequest req)
    {
        await _service.UpdateStatusAsync(id, req.Status, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new Exception("未登录");
}
