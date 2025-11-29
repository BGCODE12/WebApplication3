namespace WebApplication3.Models.DTOs.Auth
{
    public class AuthResultDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? EmployeeID { get; set; }
        public int? DepartmentID { get; set; }
    }
}
