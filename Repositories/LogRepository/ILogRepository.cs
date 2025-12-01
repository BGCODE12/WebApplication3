namespace WebApplication3.Repositories.LogRepository
{
    public interface ILogRepository
    {
        Task<int> CreateLog(string action, string table, string recordId, string details, int? userId);
    }
}
