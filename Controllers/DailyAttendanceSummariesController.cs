using Microsoft.AspNetCore.Mvc;

using WebApplication3.Models;
using WebApplication3.Models.DTOs.DailyAttendanceSummaries;
using WebApplication3.Repositories;
using WebApplication3.Repositories.DailyAttendanceSummaryRepository;

[ApiController]
[Route("api/[controller]")]
public class DailyAttendanceSummaryController : ControllerBase
{
    private readonly IDailyAttendanceSummaryRepository _repo;

    public DailyAttendanceSummaryController(IDailyAttendanceSummaryRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _repo.GetAll());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(long id)
    {
        var item = await _repo.GetById(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(DailyAttendanceSummaryCreateDto dto)
    {
        var model = new DailyAttendanceSummary
        {
            EmployeeID = dto.EmployeeID,
            WorkDate = dto.WorkDate,
            Status = dto.Status,
            TotalWorkMinutes = dto.TotalWorkMinutes,
            LateMinutes = dto.LateMinutes,
            OvertimeMinutes = dto.OvertimeMinutes
        };

        return (await _repo.Create(model)) > 0
            ? Ok("Daily summary created")
            : BadRequest();
    }

    [HttpPut]
    public async Task<IActionResult> Update(DailyAttendanceSummaryUpdateDto dto)
    {
        var model = await _repo.GetById(dto.SummaryID);
        if (model == null) return NotFound();

        model.EmployeeID = dto.EmployeeID;
        model.WorkDate = dto.WorkDate;
        model.Status = dto.Status;
        model.TotalWorkMinutes = dto.TotalWorkMinutes;
        model.LateMinutes = dto.LateMinutes;
        model.OvertimeMinutes = dto.OvertimeMinutes;

        return (await _repo.Update(model)) > 0
            ? Ok("Daily summary updated")
            : BadRequest();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        return (await _repo.Delete(id)) > 0
            ? Ok("Daily summary deleted")
            : NotFound();
    }
}
