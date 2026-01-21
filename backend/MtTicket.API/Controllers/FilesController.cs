using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MtTicket.API.DTOs.Common;
using MtTicket.API.Services;

namespace MtTicket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;
    private readonly ILogger<FilesController> _logger;

    public FilesController(IFileService fileService, ILogger<FilesController> logger)
    {
        _fileService = fileService;
        _logger = logger;
    }

    [HttpPost("upload-image")]
    public async Task<ActionResult<ApiResponse<string>>> UploadImage(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("File không được để trống"));
            }

            var imageUrl = await _fileService.UploadImageAsync(file);
            return Ok(ApiResponse<string>.SuccessResponse(imageUrl, "Upload ảnh thành công"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<string>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi upload ảnh");
            return StatusCode(500, ApiResponse<string>.ErrorResponse("Lỗi server"));
        }
    }

    [HttpDelete("delete-image")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteImage([FromQuery] string imageUrl)
    {
        try
        {
            var result = await _fileService.DeleteImageAsync(imageUrl);
            return Ok(ApiResponse<bool>.SuccessResponse(result, result ? "Xóa ảnh thành công" : "Không tìm thấy ảnh"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xóa ảnh");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("Lỗi server"));
        }
    }
}
