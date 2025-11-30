
using Dapper;
using System.Data;
using WebApplication3.Context;
using WebApplication3.Models;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly Db _db;

    public DepartmentRepository(Db db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Department>> GetAll()
    {
        return await _db.CreateConnection().QueryAsync<Department>(
            "GetAllDepartments",
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<Department?> GetById(int id)
    {
        return await _db.CreateConnection().QueryFirstOrDefaultAsync<Department>(
            "GetDepartmentById",
            new { DepartmentID = id },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> Create(Department dept)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "CreateDepartment",
            new { dept.DepartmentName },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> Update(Department dept)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "UpdateDepartment",
            new { dept.DepartmentID, dept.DepartmentName },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> Delete(int id)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "DeleteDepartment",
            new { DepartmentID = id },
            commandType: CommandType.StoredProcedure
        );
    }
}
