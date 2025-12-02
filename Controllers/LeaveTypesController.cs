using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication3.Models;
using WebApplication3.Models.DTOs.LeaveTypes;
using WebApplication3.Repositories.LeaveTypeRepository;

[ApiController]
[Route("api/[controller]")]
public class LeaveTypesController : ControllerBase
{
    private readonly ILeaveTypeRepository _repo;

    public LeaveTypesController(ILeaveTypeRepository repo)
    {
        _repo = repo;
    }

    // ============================
    // Helpers (Role)
    // ============================
    private string? GetRole() =>
        User.FindFirstValue(ClaimTypes.Role);   // SuperAdmin / Admin / UnitAdmin / Employee



    // ================================================================
    // GET ALL LEAVE TYPES
    // SuperAdmin + Admin + UnitAdmin + Employee
    // (????? ???? ??? ????? ??????? ??????? ????)
    // ================================================================
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _repo.GetAll());
    }


    // ================================================================
    // GET BY ID (Everyone Allowed)
    // ================================================================
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var item = await _repo.GetById(id);
        return item == null ? NotFound() : Ok(item);
    }


    // ================================================================
    // CREATE (SuperAdmin + Admin)
    // ================================================================
    [Authorize(Roles = "SuperAdmin,Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(LeaveTypeCreateDto dto)
    {
        var model = new LeaveType
        {
            TypeName = dto.TypeName
        };

        return (await _repo.Create(model)) > 0
            ? Ok("Leave type created")
            : BadRequest("Failed");
    }


    // ================================================================
    // UPDATE (SuperAdmin + Admin)
    // ================================================================
    [Authorize(Roles = "SuperAdmin,Admin")]
    [HttpPut]
    public async Task<IActionResult> Update(LeaveTypeUpdateDto dto)
    {
        var model = await _repo.GetById(dto.LeaveTypeID);
        if (model == null) return NotFound();

        model.TypeName = dto.TypeName;

        return (await _repo.Update(model)) > 0
            ? Ok("Leave type updated")
            : BadRequest("Failed");
    }


    // ================================================================
    // DELETE (SuperAdmin ONLY)
    // ================================================================
    [Authorize(Roles = "SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        return (await _repo.Delete(id)) > 0
            ? Ok("Leave type deleted")
            : NotFound();
    }
}
