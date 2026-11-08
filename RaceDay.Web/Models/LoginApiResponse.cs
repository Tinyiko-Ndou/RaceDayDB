namespace RaceDay.Web.Models
{
    public class LoginApiResponse
    {
        public string? Message { get; set; }
        public string? Token { get; set; }
        public LoginApiUser? User { get; set; }
    }

    public class LoginApiUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}
