namespace WebApplication3.Models
{
    public class DailyAttendanceSummary
    {
        public long SummaryID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime WorkDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? TotalWorkMinutes { get; set; }
        public int? LateMinutes { get; set; }
        public int? OvertimeMinutes { get; set; }
    }
}
