using Dapper;
using System.Data;
using WebApplication3.Context;
using WebApplication3.Models;
using WebApplication3.Repositories.AttendanceSessionRepository;

public class AttendanceSessionRepository : IAttendanceSessionRepository
{
    private readonly Db _db;

    public AttendanceSessionRepository(Db db)
    {
        _db = db;
    }

    public async Task<IEnumerable<AttendanceSession>> GetAll()
    {
        return await _db.CreateConnection().QueryAsync<AttendanceSession>(
            "GetAllAttendanceSessions",
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<AttendanceSession?> GetById(long id)
    {
        return await _db.CreateConnection().QueryFirstOrDefaultAsync<AttendanceSession>(
            "GetAttendanceSessionById",
            new { SessionID = id },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> Create(AttendanceSession model)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "CreateAttendanceSession",
            new
            {
                model.EmployeeID,
                model.WorkDate,
                model.CheckInTime,
                model.CheckOutTime,
                model.DurationMinutes
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> Update(AttendanceSession model)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "UpdateAttendanceSession",
            new
            {
                model.SessionID,
                model.EmployeeID,
                model.WorkDate,
                model.CheckInTime,
                model.CheckOutTime,
                model.DurationMinutes
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> Delete(long id)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "DeleteAttendanceSession",
            new { SessionID = id },
            commandType: CommandType.StoredProcedure
        );
    }
}
