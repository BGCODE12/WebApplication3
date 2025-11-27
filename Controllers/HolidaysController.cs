using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApplication3.Context;
using WebApplication3.Models.DTOs.Holidays;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HolidaysController : ControllerBase
    {
        private readonly Db _db;

        public HolidaysController(Db db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HolidayReadDto>>> GetAll()
        {
            using IDbConnection conn = _db.CreateConnection();

            var rows = await conn.QueryAsync<HolidayReadDto>(
                "SELECT HolidayID, HolidayName, HolidayDate FROM Holidays ORDER BY HolidayDate DESC");

            return Ok(rows);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<HolidayReadDto>> GetById(int id)
        {
            using var conn = _db.CreateConnection();

            var holiday = await conn.QueryFirstOrDefaultAsync<HolidayReadDto>(
                "SELECT HolidayID, HolidayName, HolidayDate FROM Holidays WHERE HolidayID=@id", new { id });

            return holiday is null ? NotFound(new { message = "العطلة غير موجودة." }) : Ok(holiday);
        }

        [HttpPost]
        public async Task<IActionResult> Create(HolidayCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.HolidayName))
                return BadRequest(new { message = "اسم العطلة مطلوب." });

            using var conn = _db.CreateConnection();

            var duplicate = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Holidays WHERE HolidayDate = @HolidayDate", new { dto.HolidayDate });

            if (duplicate > 0)
                return Conflict(new { message = "هناك عطلة في نفس التاريخ." });

            var id = await conn.ExecuteScalarAsync<int>(@"
                INSERT INTO Holidays (HolidayName, HolidayDate)
                VALUES (@HolidayName, @HolidayDate);
                SELECT CAST(SCOPE_IDENTITY() AS INT);", dto);

            return CreatedAtAction(nameof(GetById), new { id }, new { HolidayID = id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, HolidayUpdateDto dto)
        {
            using var conn = _db.CreateConnection();

            var exists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Holidays WHERE HolidayID=@id", new { id });

            if (exists == 0)
                return NotFound(new { message = "العطلة غير موجودة." });

            var conflict = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Holidays WHERE HolidayDate=@HolidayDate AND HolidayID<>@id",
                new { dto.HolidayDate, id });

            if (conflict > 0)
                return Conflict(new { message = "هناك عطلة أخرى في نفس التاريخ." });

            await conn.ExecuteAsync(@"
                UPDATE Holidays
                SET HolidayName=@HolidayName,
                    HolidayDate=@HolidayDate
                WHERE HolidayID=@id",
                new { id, dto.HolidayName, dto.HolidayDate });

            return Ok(new { message = "تم التحديث." });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            using var conn = _db.CreateConnection();

            var rows = await conn.ExecuteAsync("DELETE FROM Holidays WHERE HolidayID=@id", new { id });

            return rows == 0 ? NotFound(new { message = "العطلة غير موجودة." })
                             : Ok(new { message = "تم الحذف." });
        }
    }
}

