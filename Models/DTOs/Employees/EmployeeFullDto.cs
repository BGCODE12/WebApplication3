using System;
using System.Collections.Generic;
using WebApplication3.Models.DTOs.ShiftDay;

namespace WebApplication3.Models.DTOs.Employees
{
    public class EmployeeFullDto
    {
        public int EmployeeID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? DeviceUserID { get; set; }
        public int? UserDeviceSN { get; set; }
        public EmployeeDepartmentDto Department { get; set; } = new();
        public EmployeeShiftDto Shift { get; set; } = new();
        public List<ShiftDayDto> ShiftDays { get; set; } = new();
        public List<EmployeeSessionDto> Sessions { get; set; } = new();
        public List<EmployeeDailySummaryDto> DailySummaries { get; set; } = new();
        public List<EmployeeLeaveRequestDto> LeaveRequests { get; set; } = new();
    }

    public class EmployeeDepartmentDto
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
    }

    public class EmployeeShiftDto
    {
        public int ShiftID { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int GracePeriodMinutes { get; set; }
    }

    public class EmployeeSessionDto
    {
        public long SessionID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime WorkDate { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public int? DurationMinutes { get; set; }
    }

    public class EmployeeDailySummaryDto
    {
        public long SummaryID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime WorkDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? TotalWorkMinutes { get; set; }
        public int? LateMinutes { get; set; }
        public int? OvertimeMinutes { get; set; }
    }

    public class EmployeeLeaveRequestDto
    {
        public int LeaveRequestID { get; set; }
        public int EmployeeID { get; set; }
        public int LeaveTypeID { get; set; }
        public string LeaveType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? ApprovedByEmployeeID { get; set; }
        public string? ApproverName { get; set; }
    }
}

