using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/course-category")]
public class CourseCategoryController : ControllerBase
{
    private readonly CourseCategoryService _service;
    public CourseCategoryController(CourseCategoryService service) => _service = service;

    [HttpGet("tree")]
    public async Task<ApiResponse<List<CategoryTreeNode>>> Tree()
        => ApiResponse<List<CategoryTreeNode>>.Ok(await _service.GetTreeAsync(GetUser().TenantId));

    [HttpPost]
    public async Task<ApiResponse<long>> Create([FromBody] CreateCategoryRequest req)
        => ApiResponse<long>.Ok(await _service.CreateAsync(req, GetUser()));

    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(long id, [FromBody] UpdateCategoryRequest req)
    {
        await _service.UpdateAsync(id, req, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse<bool>> Delete(long id)
    {
        await _service.DeleteAsync(id, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new Exception("未登录");
}
