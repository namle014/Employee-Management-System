using OA.Core.Services;

namespace OA.Service
{
    public class UserConnectionService : IUserConnectionService
    {
        private readonly Dictionary<string, HashSet<string>> _userConnections = new();

        public Task AddConnectionAsync(string userId, string connectionId)
        {
            if (!_userConnections.ContainsKey(userId))
            {
                _userConnections[userId] = new HashSet<string>();
            }
            _userConnections[userId].Add(connectionId);
            return Task.CompletedTask;
        }

        public Task RemoveConnectionAsync(string userId, string connectionId)
        {
            if (_userConnections.ContainsKey(userId))
            {
                _userConnections[userId].Remove(connectionId);
                if (_userConnections[userId].Count == 0)
                {
                    _userConnections.Remove(userId);
                }
            }
            return Task.CompletedTask;
        }

        public Task<List<string>> GetConnectionsForUserAsync(string userId)
        {
            if (_userConnections.ContainsKey(userId))
            {
                return Task.FromResult(_userConnections[userId].ToList());
            }
            return Task.FromResult(new List<string>());
        }
    }
}
