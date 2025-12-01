using Dapper;
using Newtonsoft.Json;
using WebApplication3.Context;
using WebApplication3.Repositories.LogRepository;

public class LogRepository : ILogRepository
{
    private readonly Db _db;

    public LogRepository(Db db)
    {
        _db = db;
    }

    public async Task<int> CreateLog(string action, string table, string recordId, string details, int? userId)
    {
        return await _db.CreateConnection().ExecuteAsync(
            "CreateLog",
            new
            {
                UserID = userId,
                ActionName = action,
                TableName = table,
                RecordID = recordId,
                Details = details
            },
            commandType: System.Data.CommandType.StoredProcedure
        );
    }
}
// how logs every action in conttrolers
//await _logRepo.CreateLog(
//action: "Create",
   // table: "Employees",
   // recordId: createdEmployeeId.ToString(),
  //  details: JsonConvert.SerializeObject(dto),
   // userId: currentUserId
//);
