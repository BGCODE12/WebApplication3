
using Dapper;
using System.Data;
using WebApplication3.Context;
using WebApplication3.Models;

public class ShiftDayRepository : IShiftDayRepository
{
    private readonly Db _db;

    public ShiftDayRepository(Db db)
    {
        _db = db;
    }

    public async Task<IEnumerable<ShiftDay>> GetAll()
    {
        return await _db.CreateConnection().QueryAsync<ShiftDay>(
            "GetAllShiftDays",
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<ShiftDay?> GetById(int id)
    {
        return await _db.CreateConnection().QueryFirstOrDefaultAsync<ShiftDay>(
            "GetShiftDayById",
            new { ShiftDayID = id },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> Create(ShiftDay shiftDay)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "CreateShiftDay",
            new
            {
                shiftDay.ShiftID,
                shiftDay.DayOfWeek,
                shiftDay.IsWorkDay
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> Update(ShiftDay shiftDay)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "UpdateShiftDay",
            new
            {
                shiftDay.ShiftDayID,
                shiftDay.ShiftID,
                shiftDay.DayOfWeek,
                shiftDay.IsWorkDay
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> Delete(int id)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "DeleteShiftDay",
            new { ShiftDayID = id },
            commandType: CommandType.StoredProcedure
        );
    }
}
