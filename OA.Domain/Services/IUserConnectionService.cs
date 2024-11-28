using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Core.Services
{
    public interface IUserConnectionService
    {
        // Thêm ConnectionId cho một người dùng
        Task AddConnectionAsync(string userId, string connectionId);

        // Xóa ConnectionId của người dùng khi ngắt kết nối
        Task RemoveConnectionAsync(string userId, string connectionId);

        // Lấy danh sách tất cả ConnectionIds của một người dùng
        Task<List<string>> GetConnectionsForUserAsync(string userId);
    }
}
