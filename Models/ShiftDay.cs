namespace WebApplication3.Models
{
    public class ShiftDay
    {
        public int ShiftDayID { get; set; }
        public int ShiftID { get; set; }
        public int DayOfWeek { get; set; }   
        public bool IsWorkDay { get; set; }
    }
}
