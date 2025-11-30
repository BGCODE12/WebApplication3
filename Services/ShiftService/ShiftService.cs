using WebApplication3.Models;
using WebApplication3.Models.DTOs.Shift;

public class ShiftService : IShiftService
{
    private readonly IShiftRepository _repo;

    public ShiftService(IShiftRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Shift>> GetAll()
    {
        return await _repo.GetAll();
    }

    public async Task<Shift?> GetById(int id)
    {
        return await _repo.GetById(id);
    }

    public async Task<bool> Create(ShiftCreateDto dto)
    {
        var shift = new Shift
        {
            ShiftName = dto.ShiftName,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime
        };

        return await _repo.Create(shift) > 0;
    }

    public async Task<bool> Update(ShiftUpdateDto dto)
    {
        var shift = await _repo.GetById(dto.ShiftID);
        if (shift == null) return false;

        shift.ShiftName = dto.ShiftName;
        shift.StartTime = dto.StartTime;
        shift.EndTime = dto.EndTime;

        return await _repo.Update(shift) > 0;
    }

    public async Task<bool> Delete(int id)
    {
        return await _repo.Delete(id) > 0;
    }
}
