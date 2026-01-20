namespace MtTicket.API.Services;

/// <summary>
/// Service interface cho file upload
/// </summary>
public interface IFileService
{
    Task<string> UploadImageAsync(IFormFile file);
    Task<bool> DeleteImageAsync(string imageUrl);
}