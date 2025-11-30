
using WebApplication3.Models;

public interface IShiftDayRepository
{
    Task<IEnumerable<ShiftDay>> GetAll();
    Task<ShiftDay?> GetById(int id);
    Task<int> Create(ShiftDay shiftDay);
    Task<int> Update(ShiftDay shiftDay);
    Task<int> Delete(int id);
}
