using WebApplication3.Models;

public interface IDepartmentRepository
{
    Task<IEnumerable<Department>> GetAll();
    Task<Department?> GetById(int id);
    Task<int> Create(Department dept);
    Task<int> Update(Department dept);
    Task<int> Delete(int id);
}
