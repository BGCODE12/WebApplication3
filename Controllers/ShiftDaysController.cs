using Microsoft.AspNetCore.Mvc;
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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _repo.GetAll());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var shiftDay = await _repo.GetById(id);
        return shiftDay == null ? NotFound() : Ok(shiftDay);
    }

    [HttpPost]
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

    [HttpPut]
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        return (await _repo.Delete(id)) > 0
            ? Ok("ShiftDay Deleted")
            : NotFound();
    }
}
