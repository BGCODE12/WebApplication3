using WebApplication3.Models;

namespace WebApplication3.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<User?> GetByUsername(string username);
        Task<int> Create(User user);
        Task<IEnumerable<User>> GetAll();
        Task<int> Update(User user);
        Task<int> Delete(int id);
    }
}
