using WebApplication3.Models;
using WebApplication3.Models.DTOs.Departments;

public interface IDepartmentService
{
    Task<IEnumerable<Department>> GetAll();
    Task<Department?> GetById(int id);
    Task<bool> Create(DepartmentCreateDto dto);
    Task<bool> Update(DepartmentUpdateDto dto);
    Task<bool> Delete(int id);
}
