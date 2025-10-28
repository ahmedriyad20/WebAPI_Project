using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Data.DTO;
using WebAPI.Data.Models;

namespace WebAPI_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration config;

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            config = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUser UserFromRequest)
        {
            ApplicationUser newUser = new ApplicationUser();
            newUser.UserName = UserFromRequest.Username;
            newUser.Email = UserFromRequest.Email;
            newUser.Address = UserFromRequest.Address;

            IdentityResult result = await _userManager.CreateAsync(newUser, UserFromRequest.Password);

            if(result.Succeeded)
            {
                //Assign Role to the User
                if(!await _roleManager.RoleExistsAsync(UserFromRequest.Role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(UserFromRequest.Role));
                }

                IdentityResult identityResult =  await _userManager.AddToRoleAsync(newUser, UserFromRequest.Role);

                if(identityResult.Succeeded)
                {
                    return Ok("User Registered Successfully");
                }
            }

            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO UserFromRequest)
        {
            ApplicationUser UserFromDB = await _userManager.FindByNameAsync(UserFromRequest.Username);

            if(UserFromDB != null)
            {
                bool isFound = await _userManager.CheckPasswordAsync(UserFromDB, UserFromRequest.Password);

                if(isFound)
                {
                    //Username and Password are valid
                    //Now Generate JWT Token

                    //First get User roles if exists to put them in the token claims
                    var UserRoles = await _userManager.GetRolesAsync(UserFromDB);

                    List<Claim> UserClaims = new List<Claim>();

                    UserClaims.Add(new Claim(ClaimTypes.NameIdentifier, UserFromDB.Id));
                    UserClaims.Add(new Claim(ClaimTypes.Name, UserFromDB.UserName));
                    
                    //Add all User Roles to the UserClaims
                    foreach(var claim in UserRoles)
                    {
                        UserClaims.Add(new Claim(ClaimTypes.Role, claim));
                    }

                    //Add the Token Generated id change (JWT Predefind Claims ) to generate new token every login
                    UserClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

                    //Now it's time for the Signature part in the token
                    SymmetricSecurityKey SecurityKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:SecurityKey"]));


                    SigningCredentials signingCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);

                    //Design Token
                    JwtSecurityToken MyToken = new JwtSecurityToken(
                        issuer: config["JWT:IssuerIP"],
                        audience: config["JWT:AudienceIP"], //Angular Localhost
                        claims: UserClaims,
                        expires: DateTime.Now.AddMinutes(30),
                        signingCredentials: signingCredentials
                        );

                    //Generate Token response
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(MyToken),
                        expiration = DateTime.Now.AddMinutes(30) //MyToken.ValidTo
                    });
                }
            }

            //If Username or password not right
            ModelState.AddModelError("Username", "Invalid Username or Password");

            return BadRequest(ModelState);
        }
    }
}
