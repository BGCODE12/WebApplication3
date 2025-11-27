namespace WebApplication3.Models.DTOs.Holidays
{
    public class HolidayReadDto
    {
        public int HolidayID { get; set; }
        public string HolidayName { get; set; } = string.Empty;
        public DateTime HolidayDate { get; set; }
    }
}

