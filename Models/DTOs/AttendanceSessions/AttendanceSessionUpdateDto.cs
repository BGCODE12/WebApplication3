namespace WebApplication3.Models.DTOs.AttendanceSessions
{
    public class AttendanceSessionUpdateDto
    {
        public long SessionID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime WorkDate { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public int? DurationMinutes { get; set; }
    }
}

