using TypicalTechTools.Models;
using TypicalTechTools.Models.DTOs;

namespace TypicalTechTools.Models.Repository
{
    public interface IAuthenticationRepository
    {
        AppUser Authenticate(LoginDTO loginDTO);
        AppUser CreateUser(CreateUserDTO userDTO);
    }
}