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

    public async Task<IEnumerable<AttendanceLogRaw>> GetAll()
    {
        return await _db.CreateConnection().QueryAsync<AttendanceLogRaw>(
            "GetAllAttendanceLogs",
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<AttendanceLogRaw?> GetById(long id)
    {
        return await _db.CreateConnection().QueryFirstOrDefaultAsync<AttendanceLogRaw>(
            "GetAttendanceLogById",
            new { LogID = id },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<IEnumerable<AttendanceLogRaw>> GetByDeviceUserID(string deviceUserID)
    {
        return await _db.CreateConnection().QueryAsync<AttendanceLogRaw>(
            "GetAttendanceLogsByDeviceUserID",
            new { DeviceUserID = deviceUserID },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<IEnumerable<AttendanceLogRaw>> GetUnprocessed()
    {
        return await _db.CreateConnection().QueryAsync<AttendanceLogRaw>(
            "GetUnprocessedAttendanceLogs",
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> MarkProcessed(long id)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "MarkAttendanceLogProcessed",
            new { LogID = id },
            commandType: CommandType.StoredProcedure
        );
    }
}
