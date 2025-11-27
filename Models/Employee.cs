namespace WebApplication3.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string DeviceUserID { get; set; } = string.Empty;
        public int? UserDeviceSN { get; set; }
        public int DepartmentID { get; set; }
        public int ShiftID { get; set; }
    }
}
