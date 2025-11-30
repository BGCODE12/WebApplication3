using Dapper;
using System.Data;
using WebApplication3.Context;
using WebApplication3.Models;
using WebApplication3.Models.DTOs.Employees;
using WebApplication3.Repositories.EmployeeRepositiry;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly Db _db;

    public EmployeeRepository(Db db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Employee>> GetAll()
    {
        var result = await _db.CreateConnection().QueryAsync<Employee>(
            "GetAllEmployees",
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    public async Task<Employee?> GetById(int id)
    {
        var result = await _db.CreateConnection().QueryFirstOrDefaultAsync<Employee>(
            "GetEmployeeById",
            new { EmployeeID = id },
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    public async Task<int> Create(Employee employee)
    {
        var result = await _db.CreateConnection().ExecuteAsync(
            "CreateEmployee",
            new
            {
                employee.FullName,
                employee.DeviceUserID,
                employee.UserDeviceSN,
                employee.DepartmentID,
                employee.ShiftID
            },
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    public async Task<int> Update(Employee employee)
    {
        var result = await _db.CreateConnection().ExecuteAsync(
            "UpdateEmployee",
            new
            {
                employee.EmployeeID,
                employee.FullName,
                employee.DeviceUserID,
                employee.UserDeviceSN,
                employee.DepartmentID,
                employee.ShiftID
            },
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    public async Task<int> Delete(int id)
    {
        var result = await _db.CreateConnection().ExecuteAsync(
            "DeleteEmployee",
            new { EmployeeID = id },
            commandType: CommandType.StoredProcedure
        );

        return result;
    }
    public async Task<EmployeeFullDto?> GetFullProfile(int id)
    {
        var result = await _db.CreateConnection().QueryFirstOrDefaultAsync<EmployeeFullDto>(
            "GetEmployeeFullProfile",
            new { EmployeeID = id },
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

}
