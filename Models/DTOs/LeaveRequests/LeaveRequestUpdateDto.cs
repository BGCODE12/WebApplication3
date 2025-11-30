namespace WebApplication3.Models.DTOs.LeaveRequests
{
    public class LeaveRequestUpdateDto
    {
        public int LeaveRequestID { get; set; }

        public int EmployeeID { get; set; }
        public int LeaveTypeID { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Status { get; set; }
        public int? ApprovedByEmployeeID { get; set; }
    }
}

