using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApplication3.Context;
using WebApplication3.Models.DTOs.Devices;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevicesController : ControllerBase
    {
        private readonly Db _db;

        public DevicesController(Db db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceReadDto>>> GetAll()
        {
            using IDbConnection conn = _db.CreateConnection();

            var rows = await conn.QueryAsync<DeviceReadDto>(
                "SELECT DeviceID, DeviceName, IPAddress FROM Devices ORDER BY DeviceID");

            return Ok(rows);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DeviceReadDto>> GetById(int id)
        {
            using var conn = _db.CreateConnection();

            var device = await conn.QueryFirstOrDefaultAsync<DeviceReadDto>(
                "SELECT DeviceID, DeviceName, IPAddress FROM Devices WHERE DeviceID=@id", new { id });

            return device is null ? NotFound(new { message = "الجهاز غير موجود." }) : Ok(device);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DeviceCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.DeviceName))
                return BadRequest(new { message = "اسم الجهاز مطلوب." });

            using var conn = _db.CreateConnection();

            var id = await conn.ExecuteScalarAsync<int>(@"
                INSERT INTO Devices (DeviceName, IPAddress)
                VALUES (@DeviceName, @IPAddress);
                SELECT CAST(SCOPE_IDENTITY() AS INT);", dto);

            return CreatedAtAction(nameof(GetById), new { id }, new { DeviceID = id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, DeviceUpdateDto dto)
        {
            using var conn = _db.CreateConnection();

            var exists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Devices WHERE DeviceID=@id", new { id });

            if (exists == 0)
                return NotFound(new { message = "الجهاز غير موجود." });

            await conn.ExecuteAsync(@"
                UPDATE Devices
                SET DeviceName=@DeviceName,
                    IPAddress=@IPAddress
                WHERE DeviceID=@id", new { id, dto.DeviceName, dto.IPAddress });

            return Ok(new { message = "تم التحديث." });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            using var conn = _db.CreateConnection();

            var rows = await conn.ExecuteAsync("DELETE FROM Devices WHERE DeviceID=@id", new { id });

            return rows == 0 ? NotFound(new { message = "الجهاز غير موجود." })
                             : Ok(new { message = "تم الحذف." });
        }
    }
}

