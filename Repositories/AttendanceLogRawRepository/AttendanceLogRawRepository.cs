using Dapper;
using System.Data;
using WebApplication3.Context;
using WebApplication3.Models;
using WebApplication3.Repositories.AttendanceLogRawRepository;

public class AttendanceLogRawRepository : IAttendanceLogRawRepository
{
    private readonly Db _db;

    public AttendanceLogRawRepository(Db db)
    {
        _db = db;
    }

    // ================================
    // GET ALL
    // ================================
    public async Task<IEnumerable<AttendanceLogRaw>> GetAll()
    {
        var sql = @"
        SELECT r.*, e.EmployeeID, e.DepartmentID AS EmployeeDeptID
        FROM AttendanceLogRaw r
        JOIN Employees e ON r.DeviceUserID = e.DeviceUserID";

        return await _db.CreateConnection().QueryAsync<AttendanceLogRaw>(sql);
    }

    // ================================
    // GET BY ID
    // ================================
    public async Task<AttendanceLogRaw?> GetById(long id)
    {
        var sql = @"
        SELECT r.*, e.EmployeeID, e.DepartmentID AS EmployeeDeptID
        FROM AttendanceLogRaw r
        JOIN Employees e ON r.DeviceUserID = e.DeviceUserID
        WHERE r.LogID = @Id";

        return await _db.CreateConnection().QueryFirstOrDefaultAsync<AttendanceLogRaw>(sql, new { Id = id });
    }

    // ================================
    // GET BY DeviceUserID
    // ================================
    public async Task<IEnumerable<AttendanceLogRaw>> GetByDeviceUserID(string deviceUserID)
    {
        var sql = @"
        SELECT r.*, e.EmployeeID, e.DepartmentID AS EmployeeDeptID
        FROM AttendanceLogRaw r
        JOIN Employees e ON r.DeviceUserID = e.DeviceUserID
        WHERE r.DeviceUserID = @DeviceUserID";

        return await _db.CreateConnection().QueryAsync<AttendanceLogRaw>(sql, new { DeviceUserID = deviceUserID });
    }

    // ================================
    // GET BY DEPARTMENT
    // ================================
    public async Task<IEnumerable<AttendanceLogRaw>> GetByDepartment(int departmentId)
    {
        var sql = @"
        SELECT r.*, e.EmployeeID, e.DepartmentID AS EmployeeDeptID
        FROM AttendanceLogRaw r
        JOIN Employees e ON r.DeviceUserID = e.DeviceUserID
        WHERE e.DepartmentID = @DeptID";

        return await _db.CreateConnection().QueryAsync<AttendanceLogRaw>(sql, new { DeptID = departmentId });
    }

    // ================================
    // GET BY EMPLOYEE
    // ================================
    public async Task<IEnumerable<AttendanceLogRaw>> GetByEmployee(int employeeId)
    {
        var sql = @"
        SELECT r.*, e.EmployeeID, e.DepartmentID AS EmployeeDeptID
        FROM AttendanceLogRaw r
        JOIN Employees e ON r.DeviceUserID = e.DeviceUserID
        WHERE e.EmployeeID = @EmpID";

        return await _db.CreateConnection().QueryAsync<AttendanceLogRaw>(sql, new { EmpID = employeeId });
    }

    // ================================
    // GET DEVICE USER BY DEPARTMENT
    // ================================
    public async Task<IEnumerable<AttendanceLogRaw>> GetDeviceUserByDepartment(string deviceUserID, int departmentId)
    {
        var sql = @"
        SELECT r.*, e.EmployeeID, e.DepartmentID AS EmployeeDeptID
        FROM AttendanceLogRaw r
        JOIN Employees e ON r.DeviceUserID = e.DeviceUserID
        WHERE r.DeviceUserID = @UserID AND e.DepartmentID = @DeptID";

        return await _db.CreateConnection().QueryAsync<AttendanceLogRaw>(
            sql,
            new { UserID = deviceUserID, DeptID = departmentId }
        );
    }

    // ================================
    // GET UNPROCESSED
    // ================================
    public async Task<IEnumerable<AttendanceLogRaw>> GetUnprocessed()
    {
        var sql = @"
        SELECT r.*, e.EmployeeID, e.DepartmentID AS EmployeeDeptID
        FROM AttendanceLogRaw r
        JOIN Employees e ON r.DeviceUserID = e.DeviceUserID
        WHERE r.IsProcessed = 0";

        return await _db.CreateConnection().QueryAsync<AttendanceLogRaw>(sql);
    }

    // ================================
    // MARK PROCESSED
    // ================================
    public async Task<int> MarkProcessed(long id)
    {
        var sql = "UPDATE AttendanceLogRaw SET IsProcessed = 1 WHERE LogID = @Id";

        return await _db.CreateConnection().ExecuteAsync(sql, new { Id = id });
    }
}
