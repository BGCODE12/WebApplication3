namespace WebApplication3.Models.DTOs.Users
{
    public class UserRegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public int? EmployeeID { get; set; }
        public int? DepartmentID { get; set; }
    }
}
