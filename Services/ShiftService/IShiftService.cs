using WebApplication3.Models;
using WebApplication3.Models.DTOs.Shift;

public interface IShiftService
{
    Task<IEnumerable<Shift>> GetAll();
    Task<Shift?> GetById(int id);
    Task<bool> Create(ShiftCreateDto dto);
    Task<bool> Update(ShiftUpdateDto dto);
    Task<bool> Delete(int id);
}
