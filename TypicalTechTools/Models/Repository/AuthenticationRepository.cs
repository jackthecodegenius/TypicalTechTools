using BCrypt.Net;

using TypicalTechTools.Models.DTOs;
using TypicalTechTools.Models.Repository;
using TypicalTechTools.Models;
using TypicalTechTools.Models.Data;
using Ganss.Xss;

namespace TypicalTechTools.Models.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        //Readonly variable to store a reference to our context class
        private readonly TypicalTechToolsDBContext _context;
        //Request the context from the dependency injection by naming it in the constructor
       
        public AuthenticationRepository(TypicalTechToolsDBContext context)
        {
            _context = context;
        
        }

        public AppUser Authenticate(LoginDTO loginDTO)
        {
            var userDetails = _context.AppUsers.Where(u => u.UserName.Equals(loginDTO.UserName)).FirstOrDefault();
            //If no user was found, return null to let the caller know that the login failed.
            if (userDetails == null)
            {
                return null;
            }
            //Use bcrypt to check the password provided in the user DTO agains the hashed password stored in the 
            //user account we just retrieved.
            if (BCrypt.Net.BCrypt.EnhancedVerify(loginDTO.Password, userDetails.Password))
            {
                //If they match, return the user details to the caller to let them know it worked
                return userDetails;
            }
            //If the check failed, return null to let the caller know that the login failed.
            return null;
        }

        public AppUser CreateUser(CreateUserDTO userDTO)
        {
            //Find the user that has the same username as the one provided in the login DTO
            
            var userDetails = _context.AppUsers.Where(u => u.UserName.Equals(userDTO.UserName)).FirstOrDefault();
            //If the username returns a record, meaning the username is already taken.
            if (userDetails != null)
            {
                //Retuyrn null to the caller to let them know the account couldn't be created.
                return null;
            }

            var user = new AppUser
            {
               
                UserName = userDTO.UserName,
                Password = BCrypt.Net.BCrypt.EnhancedHashPassword(userDTO.Password),
                Role = userDTO.Role
            };
            //Add the user to the context class then save the changes to the database
            _context.AppUsers.Add(user);
            _context.SaveChanges();
            //Return the user details to the caller to confirm it worked.
            return user;
        }
    }
}
