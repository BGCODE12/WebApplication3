using Dapper;
using System.Data;
using WebApplication3.Context;
using WebApplication3.Models;
using WebApplication3.Repositories.UserRepository;

public class UserRepository : IUserRepository
{
    private readonly Db _db;

    public UserRepository(Db db)
    {
        _db = db;
    }

    public async Task<User?> GetByUsername(string username)
    {
        return await _db.CreateConnection().QueryFirstOrDefaultAsync<User>(
            "GetUserByUsername",
            new { Username = username },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> Create(User user)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "CreateUser",
            new
            {
                user.Username,
                user.PasswordHash,
                user.Role,
                user.EmployeeID,
                user.DepartmentID,
                user.IsActive
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        return await _db.CreateConnection().QueryAsync<User>(
            "GetAllUsers",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> Update(User user)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "UpdateUser",
            new
            {
                user.UserID,
                user.Username,
                user.Role,
                user.EmployeeID,
                user.DepartmentID,
                user.IsActive
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> Delete(int id)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "DeleteUser",
            new { UserID = id },
            commandType: CommandType.StoredProcedure);
    }
}
