namespace TypicalTechTools.Models.DTOs
{
    public class CreateUserDTO
    {
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PasswordConfirmation { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

    }
}
