namespace WebApplication3.Models.DTOs.Auth
{
    public class AuthRegisterDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // SuperAdmin / GlobalAdmin / UnitAdmin / Employee
        public string Role { get; set; } = "Employee";

        // لو كان النوع Employee
        public int? EmployeeID { get; set; }

        // لو كان النوع UnitAdmin
        public int? DepartmentID { get; set; }
    }
}
