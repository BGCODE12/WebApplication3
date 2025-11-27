namespace WebApplication3.Models.DTOs.DailyAttendanceSummaries
{
    public class DailyAttendanceSummaryUpdateDto
    {
        public string Status { get; set; } = string.Empty;
        public int? TotalWorkMinutes { get; set; }
        public int? LateMinutes { get; set; }
        public int? OvertimeMinutes { get; set; }
    }
}

