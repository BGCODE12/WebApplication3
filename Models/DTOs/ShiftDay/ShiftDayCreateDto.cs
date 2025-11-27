namespace WebApplication3.Models.DTOs.ShiftDay
{
    public class ShiftDayCreateDto
    {
        public int ShiftID { get; set; }
        public int DayOfWeek { get; set; } 
        public bool IsWorkDay { get; set; }
    }
}
