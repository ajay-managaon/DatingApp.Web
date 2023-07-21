using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Web.DatingApp.API.Web.DatingApp.Database;
using Web.DatingApp.API.Web.DatingApp.Dtos;
using Web.DatingApp.API.Web.DatingApp.Entities;
using Web.DatingApp.API.Web.DatingApp.Interfaces;

namespace Web.DatingApp.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class AccountController : ControllerBase
    {
        private readonly DatingAppDbContext datingAppDbContext;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public AccountController(DatingAppDbContext datingAppDbContext, ITokenService tokenService, IMapper mapper)
        {
            this.datingAppDbContext = datingAppDbContext;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        [HttpPost("account/register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
        {
            if (await UserExists(registerDto.UserName))
            {
                return BadRequest("Username already exists");
            }
            var user = mapper.Map<AppUser>(registerDto);
            using var hmac = new HMACSHA512();
            user.UserName = registerDto.UserName;
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;
            datingAppDbContext.tbl_User.Add(user);
            await datingAppDbContext.SaveChangesAsync();
            return new UserDto
            {
                Username = registerDto.UserName,
                Token = tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("account/login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto logindto)
        {
            var user = await datingAppDbContext.tbl_User.Include(p=>p.Photos).SingleOrDefaultAsync(u => u.UserName == logindto.UserName);
            if (user == null)
            {
                return Unauthorized("Invalid User Name");
            }
            using (var hmac = new HMACSHA512(user.PasswordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(logindto.Password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != user.PasswordHash[i])
                    {
                        return Unauthorized("Password Incorrect");
                    }
                }
            }
            return new UserDto
            {
                Username = user.UserName,
                Token = tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }












        #region Private methods
        private async Task<bool> UserExists(string username)
        {
            return await datingAppDbContext.tbl_User.AnyAsync(u => u.UserName == username);
        }
        #endregion
    }
}
