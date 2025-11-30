using WebApplication3.Models;

namespace WebApplication3.Repositories.LeaveTypeRepository
{
    public interface ILeaveTypeRepository
    {
        Task<IEnumerable<LeaveType>> GetAll();
        Task<LeaveType?> GetById(int id);
        Task<int> Create(LeaveType model);
        Task<int> Update(LeaveType model);
        Task<int> Delete(int id);
    }
}
