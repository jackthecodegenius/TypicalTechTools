namespace TypicalTechTools.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "USER";
    }
}
