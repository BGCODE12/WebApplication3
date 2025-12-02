using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication3.Models;
using WebApplication3.Models.DTOs.Devices;
using WebApplication3.Repositories.DeviceRepository;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceRepository _repo;

    public DevicesController(IDeviceRepository repo)
    {
        _repo = repo;
    }

    // ======================
    // 🔥 Helpers for Role, DeptID
    // ======================
    private string? GetRole() =>
        User.FindFirstValue(ClaimTypes.Role);  // SuperAdmin / Admin / UnitAdmin / Employee

    private int? GetDeptId() =>
        int.TryParse(User.FindFirstValue("DepartmentID"), out var id) ? id : null;


    // =========================================================
    // GET ALL DEVICES
    // SuperAdmin + Admin + UnitAdmin  → مسموح
    // Employee → ممنوع
    // =========================================================
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var role = GetRole();

        if (role == "SuperAdmin" || role == "Admin" || role == "UnitAdmin")
            return Ok(await _repo.GetAll());

        return Forbid("Employees cannot view devices.");
    }


    // =========================================================
    // GET BY ID
    // SuperAdmin + Admin + UnitAdmin
    // =========================================================
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get(int id)
    {
        var role = GetRole();

        if (role == "SuperAdmin" || role == "Admin" || role == "UnitAdmin")
        {
            var device = await _repo.GetById(id);
            return device == null ? NotFound() : Ok(device);
        }

        return Forbid("Employees cannot view devices.");
    }


    // =========================================================
    // CREATE DEVICE
    // SuperAdmin ONLY
    // =========================================================
    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Create(DeviceCreateDto dto)
    {
        var model = new Device
        {
            DeviceName = dto.DeviceName,
            IPAddress = dto.IPAddress
        };

        return (await _repo.Create(model)) > 0
            ? Ok("Device created")
            : BadRequest("Failed to create device");
    }


    // =========================================================
    // UPDATE DEVICE
    // SuperAdmin ONLY
    // =========================================================
    [HttpPut]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Update(DeviceUpdateDto dto)
    {
        var model = await _repo.GetById(dto.DeviceID);
        if (model == null) return NotFound();

        model.DeviceName = dto.DeviceName;
        model.IPAddress = dto.IPAddress;

        return (await _repo.Update(model)) > 0
            ? Ok("Device updated")
            : BadRequest("Failed to update");
    }


    // =========================================================
    // DELETE DEVICE
    // SuperAdmin ONLY
    // =========================================================
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        return (await _repo.Delete(id)) > 0
            ? Ok("Device deleted")
            : NotFound();
    }
}
