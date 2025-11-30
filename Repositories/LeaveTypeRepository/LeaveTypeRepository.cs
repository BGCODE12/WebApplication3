using Dapper;
using System.Data;
using WebApplication3.Context;
using WebApplication3.Models;
using WebApplication3.Repositories.LeaveTypeRepository;

public class LeaveTypeRepository : ILeaveTypeRepository
{
    private readonly Db _db;

    public LeaveTypeRepository(Db db)
    {
        _db = db;
    }

    public async Task<IEnumerable<LeaveType>> GetAll()
    {
        return await _db.CreateConnection().QueryAsync<LeaveType>(
            "GetAllLeaveTypes",
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<LeaveType?> GetById(int id)
    {
        return await _db.CreateConnection().QueryFirstOrDefaultAsync<LeaveType>(
            "GetLeaveTypeById",
            new { LeaveTypeID = id },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> Create(LeaveType model)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "CreateLeaveType",
            new { model.TypeName },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> Update(LeaveType model)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "UpdateLeaveType",
            new
            {
                model.LeaveTypeID,
                model.TypeName
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> Delete(int id)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "DeleteLeaveType",
            new { LeaveTypeID = id },
            commandType: CommandType.StoredProcedure
        );
    }
}
