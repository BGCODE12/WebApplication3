namespace WebApplication3.Models.DTOs.DailyAttendanceSummaries
{
    public class DailyAttendanceSummaryCreateDto
    {
        public int EmployeeID { get; set; }
        public DateTime WorkDate { get; set; }
        public string Status { get; set; }
        public int TotalWorkMinutes { get; set; }
        public int LateMinutes { get; set; }
        public int OvertimeMinutes { get; set; }
    }
}

