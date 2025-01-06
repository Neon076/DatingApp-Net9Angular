using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

// [ApiController]
// [Route("api/[controller]")]   // /api/users

// [controller] is replaces with 'Users' from UsersController
[Authorize]
public class UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService
) : BaseApiController
{
    // private readonly DataContext _context = context;
    [HttpGet]

    public async Task<ActionResult<IEnumerable<MembersDto>>> GetUsers([FromQuery] UserParams userParams)
    {
        userParams.CurrentUsername = User.GetUsername();
        var users = await unitOfWork.UserRespository.GetMembersAsync(userParams);

        // var usersToReturn = mapper.Map<IEnumerable<MembersDto>>(users);

        Response.AddPaginationHeader(users);

        return Ok(users);
    }

    [HttpGet("{id:int}")] // /api/users/2

    public async Task<ActionResult<MembersDto>> GetUser(int id)
    {
        var user = await unitOfWork.UserRespository.GetMemberByIdAsync(id);

        if (user == null) return NotFound();

        return Ok(user);
    }
    [HttpGet("{username}")] // /api/users/{name}

    public async Task<ActionResult<MembersDto>> GetUser(string username)
    {
        var user = await unitOfWork.UserRespository.GetMemberAsync(username);

        if (user == null) return NotFound();

        return Ok(user);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {

        var user = await unitOfWork.UserRespository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null) return BadRequest("No username found");

        mapper.Map(memberUpdateDto, user);
        if (await unitOfWork.Complete()) return NoContent();

        return BadRequest("Failed to update the user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {

        var user = await unitOfWork.UserRespository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null) return BadRequest("No user Found");

        var result = await photoService.AddPhotoAsync(file);

        if (result.Error != null) return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if (user.Photos.Count == 0) photo.IsMain = true;
        user.Photos.Add(photo);

        if (await unitOfWork.Complete()) return CreatedAtAction(nameof(GetUser), new { username = user.UserName }, mapper.Map<PhotoDto>(photo));

        return BadRequest("Problem Adding photo");
    }

    [HttpPut("set-main-photo/{photoId:int}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {

        var user = await unitOfWork.UserRespository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null) return BadRequest("User Not Found");

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null || photo.IsMain) return BadRequest("Cannot set this as main photo");

        var currentMain = user.Photos.SingleOrDefault(x => x.IsMain);

        if (currentMain != null) currentMain.IsMain = false;

        photo.IsMain = true;

        if (await unitOfWork.Complete()) return NoContent();

        return BadRequest("Problem setting main Photo");
    }

    [HttpDelete("delete-photo/{photoId:int}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await unitOfWork.UserRespository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null) return BadRequest("User not found");

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null || photo.IsMain) return BadRequest("This photo cannot be deleted");

        if (photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);

        if (await unitOfWork.Complete()) return Ok();

        return BadRequest("Problem deleting Photo");
    }
}