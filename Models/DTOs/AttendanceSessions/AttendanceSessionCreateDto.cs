namespace WebApplication3.Models.DTOs.AttendanceSessions
{
    public class AttendanceSessionCreateDto
    {
        public int EmployeeID { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
    }
}

