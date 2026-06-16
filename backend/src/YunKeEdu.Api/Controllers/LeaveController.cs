using YunKeEdu.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/leave")]
public class LeaveController : ControllerBase
{
    private readonly LeaveService _service;
    public LeaveController(LeaveService service) => _service = service;

    [HttpPost]
    public async Task<ApiResponse<long>> Create([FromBody] CreateLeaveRequest req)
        => ApiResponse<long>.Ok(await _service.CreateAsync(req, GetUser()));

    [HttpGet("my-list")]
    public async Task<ApiResponse<List<LeaveRequestDto>>> MyList()
        => ApiResponse<List<LeaveRequestDto>>.Ok(await _service.GetMyListAsync(GetUser()));

    [HttpPut("{id}/pre-review")]
    public async Task<ApiResponse<bool>> PreReview(long id, [FromBody] PreReviewRequest req)
    {
        await _service.PreReviewAsync(id, req, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPut("{id}/approve")]
    public async Task<ApiResponse<bool>> Approve(long id, [FromBody] ApproveLeaveRequest req)
    {
        await _service.ApproveAsync(id, req, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpGet("page")]
    public async Task<ApiResponse<PagedResult<LeaveRequestDto>>> Page([FromQuery] PageRequest req)
        => ApiResponse<PagedResult<LeaveRequestDto>>.Ok(await _service.GetPageAsync(req, GetUser()));

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new BizException("未登录");
}
