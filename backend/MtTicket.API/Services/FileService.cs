namespace MtTicket.API.Services;

/// <summary>
/// Service implementation cho file upload
/// </summary>
public class FileService : IFileService
{
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;
    private readonly string _uploadPath;

    public FileService(IWebHostEnvironment environment, IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
        _uploadPath = Path.Combine(_environment.WebRootPath ?? "wwwroot", "uploads", "images");
        
        // Tạo thư mục nếu chưa có
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        // Validate file
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File không hợp lệ");
        }

        // Validate file type
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        if (!allowedExtensions.Contains(fileExtension))
        {
            throw new ArgumentException("Chỉ chấp nhận file ảnh (jpg, jpeg, png, gif)");
        }

        // Validate file size (max 5MB)
        const long maxFileSize = 5 * 1024 * 1024; // 5MB
        if (file.Length > maxFileSize)
        {
            throw new ArgumentException("File không được vượt quá 5MB");
        }

        // Tạo tên file unique
        var fileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(_uploadPath, fileName);

        // Lưu file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Trả về URL
        var baseUrl = _configuration["BaseUrl"] ?? "http://localhost:5000";
        return $"{baseUrl}/uploads/images/{fileName}";
    }

    public Task<bool> DeleteImageAsync(string imageUrl)
    {
        try
        {
            if (string.IsNullOrEmpty(imageUrl))
                return Task.FromResult(false);

            var fileName = Path.GetFileName(imageUrl);
            var filePath = Path.Combine(_uploadPath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}