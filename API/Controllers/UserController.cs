using API.DTOs;
using API.Entities;
using API.Extensions;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

// [ApiController]
// [Route("api/[controller]")]   // /api/users

// [controller] is replaces with 'Users' from UsersController
[Authorize]
public class UsersController(IUserRespository userRespository , IMapper mapper , IPhotoService photoService
) : BaseApiController
{
    // private readonly DataContext _context = context;
    [HttpGet]

    public async Task<ActionResult<IEnumerable<MembersDto>>> GetUsers()
    {
        var users = await userRespository.GetMembersAsync();

        // var usersToReturn = mapper.Map<IEnumerable<MembersDto>>(users);

        return Ok(users);
    }

    [HttpGet("{id:int}")] // /api/users/2

    public async Task<ActionResult<MembersDto>> GetUser(int id)
    {
        var user = await userRespository.GetMemberByIdAsync(id);

        if(user == null) return NotFound();

        return user;
    }
    [HttpGet("{username}")] // /api/users/{name}

    public async Task<ActionResult<MembersDto>> GetUser(string username)
    {
        var user = await userRespository.GetMemberAsync(username);

        if(user == null) return NotFound();

        return user;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto){

        var user = await userRespository.GetUserByUsernameAsync(User.GetUsername());

        if(user == null) return BadRequest("No username found");
        
        mapper.Map(memberUpdateDto , user);
        if(await userRespository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update the user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file){
       
        var user = await userRespository.GetUserByUsernameAsync(User.GetUsername());

        if(user == null) return BadRequest("No user Found");

        var result = await photoService.AddPhotoAsync(file);

        if(result.Error != null) return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        user.Photos.Add(photo);

        if(await userRespository.SaveAllAsync()) return CreatedAtAction(nameof(GetUser) , new {username = user.UserName} , mapper.Map<PhotoDto>(photo));

        return BadRequest("Problem Adding photo");
    }

    [HttpPut("set-main-photo/{photoId:int}")]
    public async Task<ActionResult> SetMainPhoto(int photoId){
        
        var user = await userRespository.GetUserByUsernameAsync(User.GetUsername());

        if(user == null) return BadRequest("User Not Found");

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if(photo == null || photo.IsMain) return BadRequest("Cannot set this as main photo");    

        var currentMain = user.Photos.SingleOrDefault(x => x.IsMain);

        if(currentMain != null) currentMain.IsMain = false;

        photo.IsMain = true;

        if(await userRespository.SaveAllAsync()) return NoContent();

        return  BadRequest("Problem setting main Photo");
    }

    [HttpDelete("delete-photo/{photoId:int}")]
    public async Task<ActionResult> DeletePhoto(int photoId){
        var user = await userRespository.GetUserByUsernameAsync(User.GetUsername());

        if(user == null) return BadRequest("User not found");

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if(photo == null || photo.IsMain) return BadRequest("This photo cannot be deleted");

        if(photo.PublicId != null) {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if(result.Error != null) return BadRequest(result.Error.Message);   
        }        

        user.Photos.Remove(photo);

        if(await userRespository.SaveAllAsync()) return Ok();

        return BadRequest("Problem deleting Photo");
    }
}