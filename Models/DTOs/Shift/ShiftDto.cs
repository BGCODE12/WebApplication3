namespace WebApplication3.Models.DTOs.Shift
{
    public class ShiftDto
    {
        public int ShiftID { get; set; }
        public string ShiftName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int GracePeriodMinutes { get; set; }
    }
}
