using WebApplication3.Models;

namespace WebApplication3.Repositories.LeaveRequestRepository
{
    public interface ILeaveRequestRepository
    {
        Task<IEnumerable<LeaveRequest>> GetAll();
        Task<LeaveRequest?> GetById(int id);
        Task<int> Create(LeaveRequest model);
        Task<int> Update(LeaveRequest model);
        Task<int> Delete(int id);
        Task<IEnumerable<LeaveRequest>> GetByEmployee(int employeeId);
        Task<IEnumerable<LeaveRequest>> GetByDepartment(int departmentId);
    }
}
