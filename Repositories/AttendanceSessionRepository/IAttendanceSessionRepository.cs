using WebApplication3.Models;

namespace WebApplication3.Repositories.AttendanceSessionRepository
{
    public interface IAttendanceSessionRepository
    {
        Task<IEnumerable<AttendanceSession>> GetAll();
        Task<AttendanceSession?> GetById(long id);
        Task<int> Create(AttendanceSession model);
        Task<int> Update(AttendanceSession model);
        Task<int> Delete(long id);
        Task<IEnumerable<AttendanceSession>> GetByDepartment(int deptId);
        Task<IEnumerable<AttendanceSession>> GetByEmployee(int empId);

    }
}
