using WebApplication3.Models;

namespace WebApplication3.Repositories.DailyAttendanceSummaryRepository
{
    public interface IDailyAttendanceSummaryRepository
    {
        Task<IEnumerable<DailyAttendanceSummary>> GetAll();
        Task<DailyAttendanceSummary?> GetById(long id);
        Task<int> Create(DailyAttendanceSummary model);
        Task<int> Update(DailyAttendanceSummary model);
        Task<int> Delete(long id);
    }
}
