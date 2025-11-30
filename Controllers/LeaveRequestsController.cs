using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using WebApplication3.Models.DTOs.LeaveRequests;
using WebApplication3.Repositories;
using WebApplication3.Repositories.LeaveRequestRepository;

[ApiController]
[Route("api/[controller]")]
public class LeaveRequestsController : ControllerBase
{
    private readonly ILeaveRequestRepository _repo;

    public LeaveRequestsController(ILeaveRequestRepository repo)
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
        var item = await _repo.GetById(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(LeaveRequestCreateDto dto)
    {
        var model = new LeaveRequest
        {
            EmployeeID = dto.EmployeeID,
            LeaveTypeID = dto.LeaveTypeID,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = "Pending",
            ApprovedByEmployeeID = null
        };

        return (await _repo.Create(model)) > 0
            ? Ok("Leave request created")
            : BadRequest();
    }

    [HttpPut]
    public async Task<IActionResult> Update(LeaveRequestUpdateDto dto)
    {
        var model = await _repo.GetById(dto.LeaveRequestID);
        if (model == null) return NotFound();

        model.EmployeeID = dto.EmployeeID;
        model.LeaveTypeID = dto.LeaveTypeID;
        model.StartDate = dto.StartDate;
        model.EndDate = dto.EndDate;
        model.Status = dto.Status;
        model.ApprovedByEmployeeID = dto.ApprovedByEmployeeID;

        return (await _repo.Update(model)) > 0
            ? Ok("Leave request updated")
            : BadRequest();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        return (await _repo.Delete(id)) > 0
            ? Ok("Leave request deleted")
            : NotFound();
    }
}
