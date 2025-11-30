
using WebApplication3.Models;

public interface IShiftRepository
{
    Task<IEnumerable<Shift>> GetAll();
    Task<Shift?> GetById(int id);
    Task<int> Create(Shift shift);
    Task<int> Update(Shift shift);
    Task<int> Delete(int id);
}
