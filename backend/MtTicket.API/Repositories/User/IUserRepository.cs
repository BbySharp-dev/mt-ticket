using MtTicket.API.Models;
using MtTicket.API.Repositories.Common;

namespace MtTicket.API.Repositories.User;

/// <summary>
/// Repository interface cho User
/// </summary>
public interface IUserRepository : IRepository<Models.User>
{
    // Tìm user theo username
    Task<Models.User?> GetByUsernameAsync(string username);
    
    // Tìm user theo email
    Task<Models.User?> GetByEmailAsync(string email);
    
    // Tìm user theo username hoặc email
    Task<Models.User?> GetByUsernameOrEmailAsync(string usernameOrEmail);
    
    // Kiểm tra username đã tồn tại chưa
    Task<bool> UsernameExistsAsync(string username);
    
    // Kiểm tra email đã tồn tại chưa
    Task<bool> EmailExistsAsync(string email);
}
