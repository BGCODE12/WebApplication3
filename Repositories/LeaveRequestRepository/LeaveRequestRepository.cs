using Dapper;
using System.Data;
using WebApplication3.Context;
using WebApplication3.Models;
using WebApplication3.Repositories.LeaveRequestRepository;

public class LeaveRequestRepository : ILeaveRequestRepository
{
    private readonly Db _db;

    public LeaveRequestRepository(Db db)
    {
        _db = db;
    }

    public async Task<IEnumerable<LeaveRequest>> GetAll()
    {
        return await _db.CreateConnection().QueryAsync<LeaveRequest>(
            "GetAllLeaveRequests",
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<LeaveRequest?> GetById(int id)
    {
        return await _db.CreateConnection().QueryFirstOrDefaultAsync<LeaveRequest>(
            "GetLeaveRequestById",
            new { LeaveRequestID = id },
            commandType: CommandType.StoredProcedure
        );
    }

    // 🔥 الطلبات حسب الموظف
    public async Task<IEnumerable<LeaveRequest>> GetByEmployee(int employeeId)
    {
        return await _db.CreateConnection().QueryAsync<LeaveRequest>(
            "GetLeaveRequestsByEmployee",
            new { EmployeeID = employeeId },
            commandType: CommandType.StoredProcedure
        );
    }

    // 🔥 الطلبات حسب القسم (لوحدة المسؤول)
    public async Task<IEnumerable<LeaveRequest>> GetByDepartment(int departmentId)
    {
        return await _db.CreateConnection().QueryAsync<LeaveRequest>(
            "GetLeaveRequestsByDepartment",
            new { DepartmentID = departmentId },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> Create(LeaveRequest model)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "CreateLeaveRequest",
            new
            {
                model.EmployeeID,
                model.LeaveTypeID,
                model.StartDate,
                model.EndDate,
                model.Status
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> Update(LeaveRequest model)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "UpdateLeaveRequest",
            new
            {
                model.LeaveRequestID,
                model.EmployeeID,
                model.LeaveTypeID,
                model.StartDate,
                model.EndDate,
                model.Status,
                model.ApprovedByEmployeeID
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> Delete(int id)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "DeleteLeaveRequest",
            new { LeaveRequestID = id },
            commandType: CommandType.StoredProcedure
        );
    }
}
