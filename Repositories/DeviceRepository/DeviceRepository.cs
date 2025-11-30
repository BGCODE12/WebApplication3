using Dapper;
using System.Data;
using WebApplication3.Context;
using WebApplication3.Models;
using WebApplication3.Repositories.DeviceRepository;

public class DeviceRepository : IDeviceRepository
{
    private readonly Db _db;

    public DeviceRepository(Db db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Device>> GetAll()
    {
        return await _db.CreateConnection().QueryAsync<Device>(
            "GetAllDevices",
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<Device?> GetById(int id)
    {
        return await _db.CreateConnection().QueryFirstOrDefaultAsync<Device>(
            "GetDeviceById",
            new { DeviceID = id },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> Create(Device device)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "CreateDevice",
            new { device.DeviceName, device.IPAddress },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> Update(Device device)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "UpdateDevice",
            new
            {
                device.DeviceID,
                device.DeviceName,
                device.IPAddress
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<int> Delete(int id)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "DeleteDevice",
            new { DeviceID = id },
            commandType: CommandType.StoredProcedure
        );
    }
}
