namespace WebApplication3.Models
{
    public class AttendanceLogRaw
    {
        public long LogID { get; set; }
        public string DeviceUserID { get; set; } = string.Empty;
        public DateTime RecordTime { get; set; }
        public string DeviceIP { get; set; } = string.Empty;
        public bool Processed { get; set; }
        public int EmployeeID { get; set; }        // مربوط من جدول Employees
        public int EmployeeDeptID { get; set; }    // قسم الموظف

    }
}
