using Dapper;
using System.Data;
using WebApplication3.Context;
using WebApplication3.Models;
using WebApplication3.Repositories.DailyAttendanceSummaryRepository;

public class DailyAttendanceSummaryRepository : IDailyAttendanceSummaryRepository
{
    private readonly Db _db;

    public DailyAttendanceSummaryRepository(Db db)
    {
        _db = db;
    }

    public async Task<IEnumerable<DailyAttendanceSummary>> GetAll()
    {
        return await _db.CreateConnection().QueryAsync<DailyAttendanceSummary>(
            "GetAllDailyAttendanceSummary",
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<DailyAttendanceSummary?> GetById(long id)
    {
        return await _db.CreateConnection().QueryFirstOrDefaultAsync<DailyAttendanceSummary>(
            "SELECT s.*, e.DepartmentID AS EmployeeDeptID " +
            "FROM DailyAttendanceSummaries s " +
            "JOIN Employees e ON s.EmployeeID = e.EmployeeID " +
            "WHERE SummaryID = @Id",
            new { Id = id }
        );
    }

    public async Task<int> Create(DailyAttendanceSummary model)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "CreateDailyAttendanceSummary",
            new
            {
                model.EmployeeID,
                model.WorkDate,
                model.Status,
                model.TotalWorkMinutes,
                model.LateMinutes,
                model.OvertimeMinutes
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> Update(DailyAttendanceSummary model)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "UpdateDailyAttendanceSummary",
            new
            {
                model.SummaryID,
                model.EmployeeID,
                model.WorkDate,
                model.Status,
                model.TotalWorkMinutes,
                model.LateMinutes,
                model.OvertimeMinutes
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> Delete(long id)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "DeleteDailyAttendanceSummary",
            new { SummaryID = id },
            commandType: CommandType.StoredProcedure
        );
    }
    public async Task<IEnumerable<DailyAttendanceSummary>> GetByDepartment(int departmentId)
    {
        return await _db.CreateConnection().QueryAsync<DailyAttendanceSummary>(
            "SELECT s.* FROM DailyAttendanceSummaries s " +
            "JOIN Employees e ON s.EmployeeID = e.EmployeeID " +
            "WHERE e.DepartmentID = @DeptID",
            new { DeptID = departmentId }
        );
    }
    public async Task<IEnumerable<DailyAttendanceSummary>> GetByEmployee(int employeeId)
    {
        return await _db.CreateConnection().QueryAsync<DailyAttendanceSummary>(
            "SELECT * FROM DailyAttendanceSummaries WHERE EmployeeID = @EmpID",
            new { EmpID = employeeId }
        );
    }
   


}
