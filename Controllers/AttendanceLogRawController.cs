using Microsoft.AspNetCore.Mvc;
using WebApplication3.Repositories;
using WebApplication3.Repositories.AttendanceLogRawRepository;

[ApiController]
[Route("api/[controller]")]
public class AttendanceLogRawController : ControllerBase
{
    private readonly IAttendanceLogRawRepository _repo;

    public AttendanceLogRawController(IAttendanceLogRawRepository repo)
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
        var log = await _repo.GetById(id);
        return log == null ? NotFound() : Ok(log);
    }

    [HttpGet("user/{deviceUserID}")]
    public async Task<IActionResult> GetByUser(string deviceUserID)
    {
        return Ok(await _repo.GetByDeviceUserID(deviceUserID));
    }

    [HttpGet("unprocessed")]
    public async Task<IActionResult> GetUnprocessed()
    {
        return Ok(await _repo.GetUnprocessed());
    }

    [HttpPut("mark/{id}")]
    public async Task<IActionResult> MarkProcessed(long id)
    {
        return (await _repo.MarkProcessed(id)) > 0
            ? Ok("Marked as processed")
            : NotFound();
    }
}
