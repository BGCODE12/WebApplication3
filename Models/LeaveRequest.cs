namespace WebApplication3.Models
{
    public class LeaveRequest
    {
        public int LeaveRequestID { get; set; }
        public int EmployeeID { get; set; }
        public int LeaveTypeID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? ApprovedByEmployeeID { get; set; }
    }
}
