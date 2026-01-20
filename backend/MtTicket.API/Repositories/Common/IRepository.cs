using System.Linq.Expressions;

namespace MtTicket.API.Repositories.Common;

/// <summary>
/// Generic repository interface
/// </summary>
public interface IRepository<T> where T : class
{
    // Lấy tất cả
    Task<IEnumerable<T>> GetAllAsync();
    
    // Lấy theo ID
    Task<T?> GetByIdAsync(int id);
    
    // Tìm kiếm theo điều kiện
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    
    // Lấy một record theo điều kiện
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    
    // Kiểm tra tồn tại
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    
    // Đếm số lượng
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    
    // Thêm mới
    Task<T> AddAsync(T entity);
    
    // Thêm nhiều
    Task AddRangeAsync(IEnumerable<T> entities);
    
    // Cập nhật
    void Update(T entity);
    
    // Xóa
    void Remove(T entity);
    
    // Xóa nhiều
    void RemoveRange(IEnumerable<T> entities);
    
    // Lấy với include (eager loading)
    Task<IEnumerable<T>> GetAllWithIncludeAsync(params Expression<Func<T, object>>[] includes);
    
    // Tìm kiếm với include
    Task<IEnumerable<T>> FindWithIncludeAsync(
        Expression<Func<T, bool>> predicate,
        params Expression<Func<T, object>>[] includes);
}
