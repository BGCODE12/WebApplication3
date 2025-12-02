using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication3.Models;
using WebApplication3.Models.DTOs.ShiftDay;

[ApiController]
[Route("api/[controller]")]
public class ShiftDaysController : ControllerBase
{
    private readonly IShiftDayRepository _repo;

    public ShiftDaysController(IShiftDayRepository repo)
    {
        _repo = repo;
    }

    // ===============================
    // Helpers → لاستخراج الدور من JWT
    // ===============================
    private string? GetRole()
    {
        return User.FindFirstValue(ClaimTypes.Role);
    }

    // ===============================
    // GET ALL — الجميع يستطيع القراءة
    // ===============================
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _repo.GetAll());
    }

    // ===============================
    // GET ONE — الجميع يستطيع القراءة
    // ===============================
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get(int id)
    {
        var shiftDay = await _repo.GetById(id);
        return shiftDay == null ? NotFound() : Ok(shiftDay);
    }

    // ===============================
    // CREATE — SuperAdmin + Admin فقط
    // ===============================
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Create(ShiftDayCreateDto dto)
    {
        var model = new ShiftDay
        {
            ShiftID = dto.ShiftID,
            DayOfWeek = dto.DayOfWeek,
            IsWorkDay = dto.IsWorkDay
        };

        return (await _repo.Create(model)) > 0
            ? Ok("ShiftDay Created")
            : BadRequest();
    }

    // ===============================
    // UPDATE — SuperAdmin + Admin فقط
    // ===============================
    [HttpPut]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Update(ShiftDayUpdateDto dto)
    {
        var model = await _repo.GetById(dto.ShiftDayID);
        if (model == null) return NotFound();

        model.ShiftID = dto.ShiftID;
        model.DayOfWeek = dto.DayOfWeek;
        model.IsWorkDay = dto.IsWorkDay;

        return (await _repo.Update(model)) > 0
            ? Ok("ShiftDay Updated")
            : BadRequest();
    }

    // ===============================
    // DELETE — SuperAdmin فقط
    // ===============================
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        return (await _repo.Delete(id)) > 0
            ? Ok("ShiftDay Deleted")
            : NotFound();
    }
}
