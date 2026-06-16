using Microsoft.AspNetCore.Mvc;
using YunKeEdu.Core.Models;
using YunKeEdu.Core.Models.DTOs;
using YunKeEdu.Application.Services;

namespace YunKeEdu.Api.Controllers;

[ApiController]
[Route("api/course-package")]
public class CoursePackageController : ControllerBase
{
    private readonly CoursePackageService _service;
    public CoursePackageController(CoursePackageService service) => _service = service;

    [HttpGet("page")]
    public async Task<ApiResponse<PagedResult<CoursePackageDto>>> Page([FromQuery] PageRequest req)
        => ApiResponse<PagedResult<CoursePackageDto>>.Ok(await _service.GetPageAsync(req, GetUser()));

    [HttpGet("{id}")]
    public async Task<ApiResponse<CoursePackageDto>> Get(long id)
        => ApiResponse<CoursePackageDto>.Ok(await _service.GetByIdAsync(id, GetUser()));

    [HttpPost]
    public async Task<ApiResponse<long>> Create([FromBody] CreateCoursePackageRequest req)
        => ApiResponse<long>.Ok(await _service.CreateAsync(req, GetUser()));

    [HttpPut("{id}")]
    public async Task<ApiResponse<bool>> Update(long id, [FromBody] UpdateCoursePackageRequest req)
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

    [HttpPost("{id}/publish")]
    public async Task<ApiResponse<bool>> Publish(long id)
    {
        await _service.PublishAsync(id, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPost("{id}/offline")]
    public async Task<ApiResponse<bool>> Offline(long id)
    {
        await _service.OfflineAsync(id, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpPost("{id}/add-course")]
    public async Task<ApiResponse<bool>> AddCourse(long id, [FromQuery] long courseId)
    {
        await _service.AddCourseAsync(id, courseId, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpDelete("{id}/remove-course/{courseId}")]
    public async Task<ApiResponse<bool>> RemoveCourse(long id, long courseId)
    {
        await _service.RemoveCourseAsync(id, courseId, GetUser());
        return ApiResponse<bool>.Ok(true);
    }

    [HttpGet("available-courses")]
    public async Task<ApiResponse<List<CourseDto>>> AvailableCourses()
        => ApiResponse<List<CourseDto>>.Ok(await _service.GetAvailableCoursesAsync(GetUser()));

    [HttpPost("{id}/purchase")]
    public async Task<ApiResponse<bool>> Purchase(long id)
    {
        // TODO: Payment integration
        return ApiResponse<bool>.Ok(true);
    }

    [HttpGet("my-packages")]
    public async Task<ApiResponse<List<MyCoursePackageDto>>> MyPackages()
    {
        // TODO: Query purchased packages
        return ApiResponse<List<MyCoursePackageDto>>.Ok(new List<MyCoursePackageDto>());
    }

    [HttpGet("purchase/{id}/detail")]
    public async Task<ApiResponse<MyCoursePackageDto>> PurchaseDetail(long id)
    {
        var pkg = await _service.GetByIdAsync(id, GetUser());
        return ApiResponse<MyCoursePackageDto>.Ok(new MyCoursePackageDto
        {
            PackageId = pkg.Id, PackageName = pkg.PackageName, CoverImage = pkg.CoverImage,
            TotalPrice = pkg.TotalPrice, CourseCount = pkg.CourseCount,
            EnrolledCourseIds = pkg.Items.Select(i => i.CourseId).ToList(),
        });
    }

    private CurrentUser GetUser() => HttpContext.Items["CurrentUser"] as CurrentUser ?? throw new Exception("未登录");
}
