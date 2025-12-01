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

    // ========== GET BY USERNAME ==========
    public async Task<User?> GetByUsername(string username)
    {
        string query = "SELECT * FROM Users WHERE Username = @Username";
        return await _db.CreateConnection()
            .QueryFirstOrDefaultAsync<User>(query, new { Username = username });
    }

    // ========== CREATE USER (returns bool) ==========
    public async Task<bool> Create(User user)
    {
        int rows = await _db.CreateConnection().ExecuteAsync(
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

        return rows > 0;
    }

    // ========== GET ALL USERS ==========
    public async Task<IEnumerable<User>> GetAll()
    {
        return await _db.CreateConnection().QueryAsync<User>(
            "GetAllUsers",
            commandType: CommandType.StoredProcedure);
    }

    // ========== UPDATE USER (returns bool) ==========
    public async Task<bool> Update(User user)
    {
        int rows = await _db.CreateConnection().ExecuteAsync(
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

        return rows > 0;
    }

    // ========== DELETE USER (returns bool) ==========
    public async Task<bool> Delete(int id)
    {
        int rows = await _db.CreateConnection().ExecuteAsync(
            "DeleteUser",
            new { UserID = id },
            commandType: CommandType.StoredProcedure);

        return rows > 0;
    }

    // ========== GET USER BY ID ==========
    public async Task<User?> GetById(int id)
    {
        string query = "SELECT * FROM Users WHERE UserID = @Id";
        return await _db.CreateConnection()
            .QueryFirstOrDefaultAsync<User>(query, new { Id = id });
    }
}
