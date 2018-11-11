using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]    
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthRepository authRepo, IConfiguration configuration)
        {
            _authRepo = authRepo;
            _configuration = configuration;
        }

        [HttpPost("register")] // specifying that this http post is for register
        public async Task<IActionResult> Register(UserForRegisterDTO userForRegisterDTO) // if we don't add ApiController annotation here, then for validating the request, 
        // we have to add [FromBody] before an actions's parameter list, and also add the 
        // if(!ModelState.IsValid) condition first thing in the body of the action method (like in MVC controller course). Section 3 Lecture 31
        {
            userForRegisterDTO.Username = userForRegisterDTO.Username.ToLower(); // for consistency

            if(await _authRepo.UserExists(userForRegisterDTO.Username))
                return BadRequest("User name already exists");

            var userToCreate = new User
            {
                Name = userForRegisterDTO.Username
            };

            var createdUser = await _authRepo.Register(userToCreate, userForRegisterDTO.Password);

            return StatusCode(201);
        }

        // https://en.wikipedia.org/wiki/JSON_Web_Token - also Section 3 Lecture 32-33
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDTO userForLoginDTO)
        {
            var userFromRepo = await _authRepo.Login(userForLoginDTO.Username.ToLower(), userForLoginDTO.Password);

            if(userFromRepo == null)
                return Unauthorized(); // just generic message for security purposes
            
            // token to contain two claims
            var claims = new[]
            {
                // checking the id and the user name
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.ID.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Name)
            };

            // making sure that the token that comes back from client is a valid token, we need to sign it
            // key to sign the token, which will be hashed
            // also we have to store this key in appsettings, which is why we need the configuration/appsettings part
            var key = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            // signing credentials with the key created above
            // encrypting the key with SHA512
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // creating a security token descriptor, which contain claims, expiry date of token and signing credentials
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            // write the token created as a response we are sending back to client
            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}