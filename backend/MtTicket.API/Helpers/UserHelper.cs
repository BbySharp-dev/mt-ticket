using System.Security.Claims;

namespace MtTicket.API.Helpers;

/// <summary>
/// Helper để lấy thông tin user từ JWT claims
/// </summary>
public static class UserHelper
{
    /// <summary>
    /// Lấy user id từ claims
    /// </summary>
    public static int GetUserId(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("Không thể xác định user");
        }

        return userId;
    }

    /// <summary>
    /// Lấy username từ claims
    /// </summary>
    public static string GetUsername(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Name)?.Value
               ?? throw new UnauthorizedAccessException("Không thể xác định username");
    }

    /// <summary>
    /// Lấy email từ claims
    /// </summary>
    public static string GetEmail(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Email)?.Value
               ?? throw new UnauthorizedAccessException("Không thể xác định email");
    }
}

