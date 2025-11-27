namespace WebApplication3.Models.DTOs.Holidays
{
    public class HolidayCreateDto
    {
        public string HolidayName { get; set; } = string.Empty;
        public DateTime HolidayDate { get; set; }
    }
}

