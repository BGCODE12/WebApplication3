using WebApplication3.Models;

namespace WebApplication3.Repositories.AttendanceLogRawRepository
{
    public interface IAttendanceLogRawRepository
    {
        Task<IEnumerable<AttendanceLogRaw>> GetAll();
        Task<AttendanceLogRaw?> GetById(long id);
        Task<IEnumerable<AttendanceLogRaw>> GetByDeviceUserID(string deviceUserID);
        Task<IEnumerable<AttendanceLogRaw>> GetUnprocessed();
        Task<int> MarkProcessed(long id);
        Task<IEnumerable<AttendanceLogRaw>> GetByDepartment(int departmentId);
        Task<IEnumerable<AttendanceLogRaw>> GetByEmployee(int employeeId);
        Task<IEnumerable<AttendanceLogRaw>> GetDeviceUserByDepartment(string deviceUserId, int departmentId);
        
        
       


    }
}
