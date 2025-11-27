using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApplication3.Context;
using WebApplication3.Models;
using WebApplication3.Context;
using WebApplication3.Models.DTOs.Shift;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShiftsController : ControllerBase
    {
        private readonly Db _db;

        public ShiftsController(Db db)
        {
            _db = db;
        }

        // GET: api/shifts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShiftDto>>> GetAll()
        {
            using IDbConnection conn = _db.CreateConnection();
            var sql = @"SELECT ShiftID, ShiftName, StartTime, EndTime, GracePeriodMinutes FROM Shifts";
            var items = await conn.QueryAsync<ShiftDto>(sql);
            return Ok(items);
        }

        // GET: api/shifts/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ShiftDto>> GetById(int id)
        {
            using IDbConnection conn = _db.CreateConnection();

            var sql = @"SELECT ShiftID, ShiftName, StartTime, EndTime, GracePeriodMinutes
                        FROM Shifts WHERE ShiftID = @id";

            var item = await conn.QueryFirstOrDefaultAsync<ShiftDto>(sql, new { id });
            return item is null ? NotFound(new { message = "Shift not found" }) : Ok(item);
        }

        // POST: api/shifts
        [HttpPost]
        public async Task<IActionResult> Create(ShiftCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ShiftName))
                return BadRequest(new { message = "ShiftName is required." });

            using IDbConnection conn = _db.CreateConnection();

            var sql = @"
                INSERT INTO Shifts (ShiftName, StartTime, EndTime, GracePeriodMinutes)
                VALUES (@ShiftName, @StartTime, @EndTime, @GracePeriodMinutes);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var id = await conn.ExecuteScalarAsync<int>(sql, dto);

            return CreatedAtAction(nameof(GetById), new { id }, new { ShiftID = id, dto.ShiftName });
        }

        // PUT: api/shifts/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, ShiftUpdateDto dto)
        {
            using IDbConnection conn = _db.CreateConnection();

            var exists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Shifts WHERE ShiftID=@id", new { id });

            if (exists == 0)
                return NotFound(new { message = "Shift not found." });

            var sql = @"
                UPDATE Shifts
                SET ShiftName = @ShiftName,
                    StartTime = @StartTime,
                    EndTime = @EndTime,
                    GracePeriodMinutes = @GracePeriodMinutes
                WHERE ShiftID = @id";

            await conn.ExecuteAsync(sql, new
            {
                id,
                dto.ShiftName,
                dto.StartTime,
                dto.EndTime,
                dto.GracePeriodMinutes
            });

            return Ok(new { message = "Shift updated successfully." });
        }

        // DELETE: api/shifts/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            using IDbConnection conn = _db.CreateConnection();

            // هل هناك موظفين مرتبطين بهذه الوردية؟
            var emp = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Employees WHERE ShiftID=@id", new { id });

            if (emp > 0)
                return Conflict(new { message = "Cannot delete shift because employees are assigned to it." });

            var sql = "DELETE FROM Shifts WHERE ShiftID=@id";
            var rows = await conn.ExecuteAsync(sql, new { id });

            return rows == 0 ? NotFound(new { message = "Shift not found." })
                             : Ok(new { message = "Shift deleted successfully." });
        }
    }
}
