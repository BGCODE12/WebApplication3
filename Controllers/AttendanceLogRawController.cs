using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApplication3.Context;
using WebApplication3.Models.DTOs.AttendanceLogRaw;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceLogRawController : ControllerBase
    {
        private readonly Db _db;

        public AttendanceLogRawController(Db db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttendanceLogRawDto>>> GetAll([FromQuery] bool? processed)
        {
            using IDbConnection conn = _db.CreateConnection();

            var sql = @"
                SELECT LogID, DeviceUserID, RecordTime, DeviceIP, Processed
                FROM AttendanceLog_Raw
                WHERE (@processed IS NULL OR Processed = @processed)
                ORDER BY RecordTime DESC";

            var rows = await conn.QueryAsync<AttendanceLogRawDto>(sql, new { processed });
            return Ok(rows);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<AttendanceLogRawDto>> GetById(long id)
        {
            using var conn = _db.CreateConnection();

            var row = await conn.QueryFirstOrDefaultAsync<AttendanceLogRawDto>(
                @"SELECT LogID, DeviceUserID, RecordTime, DeviceIP, Processed
                  FROM AttendanceLog_Raw WHERE LogID=@id", new { id });

            return row is null ? NotFound(new { message = "السجل غير موجود." }) : Ok(row);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AttendanceLogRawCreateDto dto)
        {
            if (dto.LogID <= 0)
                return BadRequest(new { message = "LogID مطلوب." });

            using var conn = _db.CreateConnection();

            var exists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM AttendanceLog_Raw WHERE LogID=@LogID",
                new { dto.LogID });

            if (exists > 0)
                return Conflict(new { message = "LogID مستخدم سابقاً." });

            var sql = @"
                INSERT INTO AttendanceLog_Raw (LogID, DeviceUserID, RecordTime, DeviceIP, Processed)
                VALUES (@LogID, @DeviceUserID, @RecordTime, @DeviceIP, @Processed);";

            await conn.ExecuteAsync(sql, dto);

            return CreatedAtAction(nameof(GetById), new { id = dto.LogID }, new { dto.LogID });
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, AttendanceLogRawUpdateDto dto)
        {
            if (dto.LogID != id)
                return BadRequest(new { message = "يجب أن يتطابق LogID مع قيمة المسار." });

            using var conn = _db.CreateConnection();

            var exists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM AttendanceLog_Raw WHERE LogID=@id", new { id });

            if (exists == 0)
                return NotFound(new { message = "السجل غير موجود." });

            var sql = @"
                UPDATE AttendanceLog_Raw
                SET DeviceUserID=@DeviceUserID,
                    RecordTime=@RecordTime,
                    DeviceIP=@DeviceIP,
                    Processed=@Processed
                WHERE LogID=@LogID";

            await conn.ExecuteAsync(sql, dto);

            return Ok(new { message = "تم التحديث." });
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            using var conn = _db.CreateConnection();

            var rows = await conn.ExecuteAsync("DELETE FROM AttendanceLog_Raw WHERE LogID=@id", new { id });

            return rows == 0 ? NotFound(new { message = "السجل غير موجود." })
                             : Ok(new { message = "تم الحذف." });
        }
    }
}

