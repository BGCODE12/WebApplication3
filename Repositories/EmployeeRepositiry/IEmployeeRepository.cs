using WebApplication3.Models;
using WebApplication3.Models.DTOs.Employees;

namespace WebApplication3.Repositories.EmployeeRepositiry
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAll();
        Task<EmployeeFullDto?> GetFullProfile(int id);
        Task<Employee?> GetById(int id);
        Task<int> Create(Employee employee);
        Task<int> Update(Employee employee);
        Task<int> Delete(int id);
    }
}
