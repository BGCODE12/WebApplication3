namespace WebApplication3.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // SuperAdmin / GlobalAdmin / UnitAdmin / Employee
        public int? EmployeeID { get; set; }
        public int? DepartmentID { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
