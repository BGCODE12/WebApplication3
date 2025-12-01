using WebApplication3.Models;

namespace WebApplication3.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<User?> GetByUsername(string username);
        Task<bool> Create(User user);
        Task<IEnumerable<User>> GetAll();
        Task<bool> Update(User user);
        Task<bool> Delete(int id);
        Task<User?> GetById(int id);

    }
}
