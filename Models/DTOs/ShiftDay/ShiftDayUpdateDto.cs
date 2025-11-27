namespace WebApplication3.Models.DTOs.ShiftDay
{
    public class ShiftDayUpdateDto
    {
        public int DayOfWeek { get; set; } // 0-6
        public bool IsWorkDay { get; set; }
    }
}
