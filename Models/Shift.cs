namespace WebApplication3.Models
{
    public class Shift
    {
        public int ShiftID { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int GracePeriodMinutes { get; set; }
    }
}
