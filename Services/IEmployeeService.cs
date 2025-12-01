using WebApplication3.Models;
using WebApplication3.Models.DTOs.Employees;

namespace WebApplication3.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAll();
        Task<EmployeeFullDto?> GetFullProfile(int id);
        Task<Employee?> GetById(int id);
        Task<bool> Create(EmployeeCreateDto dto);
        Task<bool> Update(EmployeeUpdateDto dto);
        Task<bool> Delete(int id);
        Task<IEnumerable<Employee>> GetByDepartment(int departmentId);


    }
}
