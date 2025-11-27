namespace WebApplication3.Models.DTOs.AttendanceSessions
{
    public class AttendanceSessionUpdateDto
    {
        public DateTime WorkDate { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public int? DurationMinutes { get; set; }
    }
}

