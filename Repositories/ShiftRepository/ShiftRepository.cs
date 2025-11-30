
using Dapper;
using System.Data;
using WebApplication3.Context;
using WebApplication3.Models;

public class ShiftRepository : IShiftRepository
{
    private readonly Db _db;

    public ShiftRepository(Db db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Shift>> GetAll()
    {
        return await _db.CreateConnection().QueryAsync<Shift>(
            "GetAllShifts",
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<Shift?> GetById(int id)
    {
        return await _db.CreateConnection().QueryFirstOrDefaultAsync<Shift>(
            "GetShiftById",
            new { ShiftID = id },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> Create(Shift shift)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "CreateShift",
            new
            {
                shift.ShiftName,
                shift.StartTime,
                shift.EndTime
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> Update(Shift shift)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "UpdateShift",
            new
            {
                shift.ShiftID,
                shift.ShiftName,
                shift.StartTime,
                shift.EndTime
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> Delete(int id)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "DeleteShift",
            new { ShiftID = id },
            commandType: CommandType.StoredProcedure
        );
    }
}
