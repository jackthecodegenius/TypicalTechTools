﻿namespace TypicalTechTools.Models.DTOs
{
    public class LoginDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ReturnURL { get; set; } = string.Empty;
    }
}
