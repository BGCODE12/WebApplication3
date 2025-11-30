using WebApplication3.Models;
using WebApplication3.Models.DTOs.Employees;
using WebApplication3.Repositories.EmployeeRepositiry;
using WebApplication3.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repo;

    public EmployeeService(IEmployeeRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Employee>> GetAll()
    {
        return await _repo.GetAll();
    }

    public async Task<Employee?> GetById(int id)
    {
        return await _repo.GetById(id);
    }

    public async Task<bool> Create(EmployeeCreateDto dto)
    {
        var emp = new Employee
        {
            FullName = dto.FullName,
            DeviceUserID = dto.DeviceUserID,
            UserDeviceSN = dto.UserDeviceSN,
            DepartmentID = dto.DepartmentID,
            ShiftID = dto.ShiftID
        };

        return await _repo.Create(emp) > 0;
    }

    public async Task<bool> Update(EmployeeUpdateDto dto)
    {
        var emp = await _repo.GetById(dto.EmployeeID);
        if (emp == null) return false;

        emp.FullName = dto.FullName;
        emp.DeviceUserID = dto.DeviceUserID;
        emp.UserDeviceSN = dto.UserDeviceSN;
        emp.DepartmentID = dto.DepartmentID;
        emp.ShiftID = dto.ShiftID;

        return await _repo.Update(emp) > 0;
    }

    public async Task<bool> Delete(int id)
    {
        return await _repo.Delete(id) > 0;
    }
    public async Task<EmployeeFullDto?> GetFullProfile(int id)
    {
        return await _repo.GetFullProfile(id);
    }

}
