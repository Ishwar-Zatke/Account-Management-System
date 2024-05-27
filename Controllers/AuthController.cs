using Account_Management.Data;
using Account_Management.Middleware;
using Account_Management.Models.Domain;
using Account_Management.Models.DTO;
using Account_Management.Repository.Interfaces;
using Account_Management.Repository.SqlCode;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Account_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IUserRepository userRepo;
        private readonly ITokenRepository tokenRepo;

        public AuthController(ApplicationDbContext dbContext, IUserRepository userRepo, ITokenRepository tokenRepo)
        {
            this.dbContext = dbContext;
            this.userRepo = userRepo;
            this.tokenRepo = tokenRepo;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequestDTO)
        {

            try
            {
                if (await userRepo.UserExistsAsync(registerRequestDTO.username))
                {
                    return BadRequest(new
                    {
                        displayMessage = "User Already Exists",
                        statusMessage = "FAIL"
                    });
                }
                var role = await userRepo.GetRoleByNameAsync(registerRequestDTO.Role);
                if (role == null)
                {
                    
                    return BadRequest(new
                    {
                        displayMessage = "Role does'nt exist",
                        statusMessage = "FAIL"
                    });
                }
                else
                {

                    var user = new User
                    {
                        Id = Guid.NewGuid(),
                        userId = registerRequestDTO.userId,
                        username = registerRequestDTO.username,
                        email = registerRequestDTO.email,
                        firstname = registerRequestDTO.firstname,
                        lastname = registerRequestDTO.lastname,
                        password = BCrypt.Net.BCrypt.HashPassword(registerRequestDTO.password),
                        roleType = role.Name,
                        roleId = role.Id,

                    };
                    await userRepo.CreateUserAsync(user);
                }
                return Ok(new
                {
                    displayMessage = "User Registered! Please Login",
                    statusMessage = "SUCCESS"
                });
            }
            catch (CustomerExceptionHandler ex)
            {
                Console.WriteLine(ex);
                return StatusCode(ex.StatusCode, new
                {
                    displayMessage = ex.StatusMessage,
                    statusMessage = "FAIL",
                    supportMessage = new
                    {
                        ex.Message,
                        ex.StackTrace
                    }

                });
            }
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginReqDTO loginRequestDTO)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    return BadRequest(new
                    {
                        displayMessage = "User is already logged in",
                        statusMessage = "FAIL"
                    });
                }
                var user = await userRepo.GetUserByUsernameAsync(loginRequestDTO.username);
                if (user == null)
                {
                    return Unauthorized(new
                    {
                        displayMessage = "Wrong User Credentials",
                        statusMessage = "UNAUTHORIZED"
                    });
                }
                if (user != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(loginRequestDTO.password, user.password))
                    {
                        var roles = new List<string> { user.roleType };

                        //var jwtToken = tokenRepo.CreateJwtToken(user, roles);

                        var claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.Role, user.username));
                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = false,
                            IssuedUtc = DateTime.UtcNow,
                        };
                        await HttpContext.SignInAsync(
                       CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                        //var loginResponse = new LoginResDTO
                        //{
                        //    JwtToken = jwtToken
                        //};
                        //return Ok(loginResponse);

                        return Ok(new
                        {
                            displayMessage = "Logged in successfully",
                            statusMessage = "SUCCESS"
                        });
                    }
                }
                return BadRequest(new
                {
                    displayMessage = "Username or password is incorrect",
                    statusMessage = "FAIL"
                });
            }
            catch(CustomerExceptionHandler ex)
            {
                Console.WriteLine(ex);
                return StatusCode(ex.StatusCode, new
                {
                    displayMessage = ex.StatusMessage,
                    statusMessage = "FAIL",
                    supportMessage = new
                    {
                        ex.Message,
                        ex.StackTrace
                    }
                });
            }
        }

        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            
            try
            {
                var ClaimUserId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                var ClaimSessionId = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
                if (ClaimUserId != null && ClaimSessionId != null)
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return Ok(new
                    {
                        displayMessage = "Logged-Out Successfully",
                        statusMessage = "SUCCESS",

                    });
                }
                else
                {
                    return Unauthorized(new
                    {
                        displayMessage = "User not found ",
                        statusMessage = "UNAUTHORIZED"

                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new
                {
                    displayMessage = "Something went wrong !",
                    statusMessage = "FAIL",
                    ResponseBody = ex.ToString()
                });

            }
        }
    }
}
