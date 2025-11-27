namespace WebApplication3.Models.DTOs.Devices
{
    public class DeviceReadDto
    {
        public int DeviceID { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;
    }
}

