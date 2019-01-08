using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _datingRepo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository datingRepo, IMapper mapper)
        {
            _mapper = mapper;
            _datingRepo = datingRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _datingRepo.GetUsers();
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDTO>>(users);
            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name="GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _datingRepo.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailedDTO>(user);
            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDTO userForUpdateDTO)
        {
            // if the current user that is passed to our server matches the id
            // FindFirst - Retrieves the first claim with the specified claim type.
            // the ClaimType is connected to the ones declared in AuthController
            // https://docs.microsoft.com/en-us/dotnet/api/system.security.claims.claimsidentity.findfirst?view=netframework-4.7.2#System_Security_Claims_ClaimsIdentity_FindFirst_System_String_
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _datingRepo.GetUser(id);

            _mapper.Map(userForUpdateDTO, userFromRepo);

            // if save is successful then return no content
            if(await _datingRepo.SaveAll())
                return NoContent();
            
            // $ - string interpolation - i.e. instead of the usual '{0}...id we do in string formatting,
            // we can directly interpolate id as below
            // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/interpolated
            throw new Exception($"Updating user {id} failed on save");
        }
    }
}