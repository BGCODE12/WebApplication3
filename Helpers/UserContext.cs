using System.Security.Claims;

namespace WebApplication3.Helpers
{
    public static class UserContext
    {
        public static int GetUserID(ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst("UserID")?.Value ?? "0");
        }

        public static int GetRole(ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst("Role")?.Value ?? "2"); 
        }

        public static int? GetEmployeeID(ClaimsPrincipal user)
        {
            string? value = user.FindFirst("EmployeeID")?.Value;
            return string.IsNullOrEmpty(value) ? null : int.Parse(value);
        }

        public static int? GetDepartmentID(ClaimsPrincipal user)
        {
            string? value = user.FindFirst("DepartmentID")?.Value;
            return string.IsNullOrEmpty(value) ? null : int.Parse(value);
        }
    }
}
