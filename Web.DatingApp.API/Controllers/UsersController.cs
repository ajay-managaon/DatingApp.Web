using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.DatingApp.API.Web.DatingApp.Dtos;
using Web.DatingApp.API.Web.DatingApp.Entities;
using Web.DatingApp.API.Web.DatingApp.Extensions;
using Web.DatingApp.API.Web.DatingApp.Helpers;
using Web.DatingApp.API.Web.DatingApp.Implenentations.Implementations;
using Web.DatingApp.API.Web.DatingApp.Interfaces.Repositories;

namespace Web.DatingApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api")]
    [ServiceFilter(typeof(LogUserActivity))]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly IPhotoService photoService;

        public UsersController(
            IUserRepository userRepository,
            IMapper mapper,
            IPhotoService photoService)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.photoService = photoService;
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            var currentUser = await userRepository.GetUserByNameAsync(User.GetUserName());
            userParams.CurrentUserName = currentUser.UserName;

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = currentUser.Gender;
            }

            var users = await userRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));
            return Ok(users);
        }

        [HttpGet("user/{name}", Name = "GetUser")]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUserByName(string name)
        {
            return Ok(await userRepository.GetMemberAsync(name));

        }

        [HttpPut("user/updateuser")]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.GetUserName();
            var user = await userRepository.GetUserByNameAsync(username);
            mapper.Map(memberUpdateDto, user);
            userRepository.Update(user);
            if (await userRepository.SaveAllChanges())
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("user/add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormCollection formCollection)
        {

            var user = await userRepository.GetUserByNameAsync(User.GetUserName());
            var result = await photoService.AddPhotoAsync(formCollection.Files.FirstOrDefault());
            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
            };
            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }
            user.Photos.Add(photo);
            if (await userRepository.SaveAllChanges())
            {
                return CreatedAtRoute("GetUser", new { name = user.UserName }, mapper.Map<PhotoDto>(photo));
            }
            return BadRequest("Problem adding photo");
        }



        [HttpPut("user/set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await userRepository.GetUserByNameAsync(User.GetUserName());
            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);
            if (photo.IsMain)
            {
                return BadRequest("The Photo is already a main photo");
            }
            var currentMainPhoto = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMainPhoto != null)
            {
                currentMainPhoto.IsMain = false;
            }
            photo.IsMain = true;
            if (await userRepository.SaveAllChanges())
            {
                return NoContent();
            }
            return BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<IActionResult> DeletePhoto(int photoId)
        {
            var user = await userRepository.GetUserByNameAsync(User.GetUserName());
            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);
            if (photo == null)
            {
                return NotFound();
            }
            if (photo.IsMain)
            {
                return BadRequest("Cannot delete main photo");
            }
            if (photo.PublicId != null)
            {
                var result = await photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null)
                {
                    return BadRequest(result.Error.Message);
                }
            }
            user.Photos.Remove(photo);
            if (await userRepository.SaveAllChanges())
            {
                return Ok();
            }
            return BadRequest("Failed to delete the image");
        }
    }
}
