using TypicalTechTools.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using TypicalTechTools.Models.Repository;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using TypicalTechTools.Models.DTOs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
namespace TypicalTools.Controllers
{
    public class AdminController : Controller
    {

        // <summary>
        /// The AdminController handles admin-specific functionalities and enforces strict role-based
        /// authorization to control access to sensitive actions and data.
        ///
        /// Authorisation:
        /// - Role-based authorisation is implemented using the [Authorize] attribute.
        /// - Methods such as SignUp are restricted to users with the "ADMIN" role.
        /// - The authorisation system relies on claims, which are generated during the login process
        ///   and include the user's role, ID, and username.
        /// - Unauthorised users attempting to access restricted methods are redirected to the AccessDenied view.
        ///
        /// Authentication:
        /// - During login, user credentials are validated against the database using the Authenticate method.
        /// - On successful authentication, claims are created and stored in a ClaimsPrincipal, which is then
        ///   used to sign the user in via cookie-based authentication.
        /// - The user's role and other identifiers are included in these claims to facilitate authorization checks.
        ///
        /// Login State Management:
        /// - Login sessions are maintained using cookies configured through the authentication middleware.
        /// - These cookies store user claims and are secured with properties such as HttpOnly and Secure
        ///   to prevent unauthorized access or tampering.
        ///
        /// Key Features:
        /// - Enforces access control for admin-only actions like user creation.
        /// - Uses claims-based authorisation to ensure flexibility and scalability for managing roles.
        /// - Provides an AccessDenied view for unauthorized access attempts, enhancing user feedback and security.
        ///
        /// </summary>

        private readonly IAuthenticationRepository _repository;
        HtmlSanitizer _sanitizer=new HtmlSanitizer();
        public AdminController(IAuthenticationRepository repository, HtmlSanitizer sanitizer)
        {
            _repository = repository;
            _sanitizer = sanitizer;
        }
        public IActionResult AdminLogin([FromQuery] string ReturnUrl)
        {
            LoginDTO loginDTO = new LoginDTO
            {
                ReturnURL = string.IsNullOrWhiteSpace(ReturnUrl) ? "/" : ReturnUrl
            };
            return View(loginDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        // Protects against cross-site request forgery (CSRF) attacks




        public IActionResult AdminLogin(LoginDTO loginDTO)
        {


            
            if (ModelState.IsValid == false)
            {

                return View(loginDTO);
            }
            loginDTO.UserName = _sanitizer.Sanitize(loginDTO.UserName);
            loginDTO.Password = _sanitizer.Sanitize(loginDTO.Password);
            var user = _repository.Authenticate(loginDTO);

            if (string.IsNullOrWhiteSpace(loginDTO.UserName) || string.IsNullOrWhiteSpace(loginDTO.Password))
            {
                ModelState.AddModelError(string.Empty, "Username and Password cannot be empty.");
                return View(loginDTO);
            }
            if (user == null)
            {
          
                ViewBag.LoginMessage = "Username or Password is invalid";
                return View(loginDTO);
            }

            var claims = new List<Claim>
            {
                
                new Claim(ClaimTypes.Role,user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
                new Claim(ClaimTypes.Name, user.UserName)

            };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

            
            var authProperties = new AuthenticationProperties
            {
                
                AllowRefresh = true,
               
                IsPersistent = true
            };

            
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);


            return Redirect(loginDTO.ReturnURL);



        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public IActionResult SignUp()
        {
          
           
                return View();
            
         
        }
        [HttpGet]
        
        public IActionResult CreateUser()
        {


            return View();


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(CreateUserDTO userDTO)
        {
            userDTO.UserName= _sanitizer.Sanitize(userDTO.UserName);
            userDTO.Password=_sanitizer.Sanitize(userDTO.Password);
            userDTO.PasswordConfirmation = _sanitizer.Sanitize(userDTO.PasswordConfirmation);

            if (string.IsNullOrWhiteSpace(userDTO.UserName) || string.IsNullOrWhiteSpace(userDTO.Password) || string.IsNullOrWhiteSpace(userDTO.PasswordConfirmation))
            {
                ModelState.AddModelError(string.Empty, "Username and Password cannot be empty.");
                return View(userDTO);
            }
            if (userDTO.Password.Equals(userDTO.PasswordConfirmation) == false)
            {
               
                ViewBag.CreateUserError = "Password and Confirmation do not match.";
                return View(userDTO);
            }
           
            if (ModelState.IsValid == false)
            {
                return View(userDTO);
            }
          
            var user = _repository.CreateUser(userDTO);
         
            if (user == null)
            {
       
                ViewBag.CreateUserError = "Username already exists. Please choose a different username.";
                return View(userDTO);
            }
       
            ViewBag.CreateUserConfirmation = "User Account Created";
            ModelState.Clear();
          
            return View();
           

            

           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(CreateUserDTO userDTO)
        {
            userDTO.UserName = _sanitizer.Sanitize(userDTO.UserName);
            userDTO.Password = _sanitizer.Sanitize(userDTO.Password);
            userDTO.PasswordConfirmation = _sanitizer.Sanitize(userDTO.PasswordConfirmation);
            if (string.IsNullOrWhiteSpace(userDTO.UserName) || string.IsNullOrWhiteSpace(userDTO.Password) || string.IsNullOrWhiteSpace(userDTO.PasswordConfirmation))
            {
                ModelState.AddModelError(string.Empty, "Username and Password cannot be empty.");
                return View(userDTO);
            }
            if (userDTO.Password.Equals(userDTO.PasswordConfirmation) == false)
            {

                ViewBag.CreateUserError = "Password and Confirmation do not match.";
                return View(userDTO);
            }

            if (ModelState.IsValid == false)
            {
                return View(userDTO);
            }

            var user = _repository.CreateUser(userDTO);

            if (user == null)
            {

                ViewBag.CreateUserError = "Username already exists. Please choose a different username.";
                return View(userDTO);
            }

            ViewBag.CreateUserConfirmation = "User Account Created";
            ModelState.Clear();

            return View();





        }
        public ActionResult AccessDenied()
        {
            return View();
        }
        public IActionResult Logoff()
        {
            HttpContext.SignOutAsync();

            // Redirect the user to the product index page
            return RedirectToAction("Index", "Product");
        }
    }
}
