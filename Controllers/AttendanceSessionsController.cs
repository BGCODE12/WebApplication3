using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApplication3.Context;
using WebApplication3.Models;
using WebApplication3.Models.DTOs.AttendanceSessions;
using WebApplication3.Repositories.AttendanceSessionRepository;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceSessionsController : ControllerBase
    {
        private readonly IAttendanceSessionRepository _repo;

        public AttendanceSessionsController(IAttendanceSessionRepository repo)
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
        public async Task<IActionResult> Create(AttendanceSessionCreateDto dto)
        {
            var model = new AttendanceSession
            {
                EmployeeID = dto.EmployeeID,
                CheckInTime = dto.CheckInTime,
                CheckOutTime = dto.CheckOutTime,
                WorkDate = dto.CheckInTime.Date,
                DurationMinutes = (int)(dto.CheckOutTime - dto.CheckInTime).TotalMinutes
            };

            return (await _repo.Create(model)) > 0
                ? Ok("Session created")
                : BadRequest();
        }

        [HttpPut]
        public async Task<IActionResult> Update(AttendanceSessionUpdateDto dto)
        {
            var model = await _repo.GetById(dto.SessionID);
            if (model == null) return NotFound();

            model.EmployeeID = dto.EmployeeID;
            model.CheckInTime = dto.CheckInTime;
            model.CheckOutTime = dto.CheckOutTime;
            model.WorkDate = dto.CheckInTime.Date;
            model.DurationMinutes = (int)(dto.CheckOutTime - dto.CheckInTime).TotalMinutes;

            return (await _repo.Update(model)) > 0
                ? Ok("Session updated")
                : BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            return (await _repo.Delete(id)) > 0
                ? Ok("Session deleted")
                : NotFound();
        }
    }

}


