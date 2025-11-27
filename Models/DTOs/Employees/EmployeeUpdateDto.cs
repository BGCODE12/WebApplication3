namespace WebApplication3.Models.DTOs.Employees
{
    public class EmployeeUpdateDto
    {
        public string FullName { get; set; }
        public string DeviceUserID { get; set; }
        public int DepartmentID { get; set; }
        public int ShiftID { get; set; }
        public int? UserDeviceSN { get; set; }
    }
}
