using Dapper;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Context;
using WebApplication3.Models.DTOs.ShiftDay;

[ApiController]
[Route("api/[controller]")]
public class ShiftDaysController : ControllerBase
{
    private readonly Db _db;

    public ShiftDaysController(Db db)
    {
        _db = db;
    }

    [HttpGet("{shiftId:int}")]
    public async Task<IActionResult> GetByShift(int shiftId)
    {
        using var conn = _db.CreateConnection();
        var sql = "SELECT * FROM ShiftDays WHERE ShiftID=@shiftId";
        var days = await conn.QueryAsync<ShiftDayDto>(sql, new { shiftId });

        return Ok(days);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ShiftDayCreateDto dto)
    {
        using var conn = _db.CreateConnection();

        var id = await conn.ExecuteScalarAsync<int>(
            @"INSERT INTO ShiftDays (ShiftID, DayOfWeek, IsWorkDay)
              VALUES (@ShiftID, @DayOfWeek, @IsWorkDay);
              SELECT CAST(SCOPE_IDENTITY() AS INT);",
            dto
        );

        return Ok(new { ShiftDayID = id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ShiftDayUpdateDto dto)
    {
        using var conn = _db.CreateConnection();

        var exists = await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM ShiftDays WHERE ShiftDayID=@id",
            new { id });

        if (exists == 0)
            return NotFound("اليوم غير موجود");

        await conn.ExecuteAsync(
            @"UPDATE ShiftDays 
              SET  DayOfWeek=@DayOfWeek, IsWorkDay=@IsWorkDay
              WHERE ShiftDayID=@id",
            new { dto.DayOfWeek, dto.IsWorkDay, id }
        );

        return Ok("تم التحديث");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        using var conn = _db.CreateConnection();

        await conn.ExecuteAsync("DELETE FROM ShiftDays WHERE ShiftDayID=@id", new { id });

        return Ok("تم الحذف");
    }
}
