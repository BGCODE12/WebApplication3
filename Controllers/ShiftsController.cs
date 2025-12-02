using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication3.Models.DTOs.Shift;
using WebApplication3.Services;

[ApiController]
[Route("api/[controller]")]
public class ShiftsController : ControllerBase
{
    private readonly IShiftService _service;

    public ShiftsController(IShiftService service)
    {
        _service = service;
    }

    // =============================================
    // Helpers → Read Claims from JWT
    // =============================================
    private string? GetRole()
    {
        return User.FindFirstValue(ClaimTypes.Role);
    }


    // =============================================
    // GET ALL SHIFTS
    // SuperAdmin + Admin + UnitAdmin + Employee
    // =============================================
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAll());
    }


    // =============================================
    // GET SHIFT BY ID
    // Everyone can read it
    // =============================================
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get(int id)
    {
        var shift = await _service.GetById(id);
        return shift == null ? NotFound() : Ok(shift);
    }


    // =============================================
    // CREATE SHIFT
    // Only SuperAdmin + Admin
    // =============================================
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Create(ShiftCreateDto dto)
    {
        var result = await _service.Create(dto);
        return result ? Ok("Shift Created") : BadRequest();
    }


    // =============================================
    // UPDATE SHIFT
    // Only SuperAdmin + Admin
    // =============================================
    [HttpPut]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Update(ShiftUpdateDto dto)
    {
        var result = await _service.Update(dto);
        return result ? Ok("Shift Updated") : NotFound();
    }


    // =============================================
    // DELETE SHIFT
    // Only SuperAdmin
    // =============================================
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.Delete(id);
        return result ? Ok("Shift Deleted") : NotFound();
    }
}
