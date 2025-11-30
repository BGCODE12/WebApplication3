using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using WebApplication3.Models.DTOs.Devices;
using WebApplication3.Repositories;
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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _repo.GetAll());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var device = await _repo.GetById(id);
        return device == null ? NotFound() : Ok(device);
    }

    [HttpPost]
    public async Task<IActionResult> Create(DeviceCreateDto dto)
    {
        var model = new Device
        {
            DeviceName = dto.DeviceName,
            IPAddress = dto.IPAddress
        };

        return (await _repo.Create(model)) > 0
            ? Ok("Device created")
            : BadRequest();
    }

    [HttpPut]
    public async Task<IActionResult> Update(DeviceUpdateDto dto)
    {
        var model = await _repo.GetById(dto.DeviceID);
        if (model == null) return NotFound();

        model.DeviceName = dto.DeviceName;
        model.IPAddress = dto.IPAddress;

        return (await _repo.Update(model)) > 0
            ? Ok("Device updated")
            : BadRequest();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        return (await _repo.Delete(id)) > 0
            ? Ok("Device deleted")
            : NotFound();
    }
}
