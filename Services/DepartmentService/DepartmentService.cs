using WebApplication3.Models;
using WebApplication3.Models.DTOs.Departments;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _repo;

    public DepartmentService(IDepartmentRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Department>> GetAll()
    {
        return await _repo.GetAll();
    }

    public async Task<Department?> GetById(int id)
    {
        return await _repo.GetById(id);
    }

    public async Task<bool> Create(DepartmentCreateDto dto)
    {
        var dept = new Department
        {
            DepartmentName = dto.DepartmentName
        };

        return await _repo.Create(dept) > 0;
    }

    public async Task<bool> Update(DepartmentUpdateDto dto)
    {
        var dept = await _repo.GetById(dto.DepartmentID);
        if (dept == null) return false;

        dept.DepartmentName = dto.DepartmentName;

        return await _repo.Update(dept) > 0;
    }

    public async Task<bool> Delete(int id)
    {
        return await _repo.Delete(id) > 0;
    }
}
