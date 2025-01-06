using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Authorize]
public class LikesController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpPost("{targetUserId:int}")]
    public async Task<ActionResult> ToggleLike(int targetUserId)
    {
        var sourceUserId = User.GetUserId();

        if (sourceUserId == targetUserId) return BadRequest("You cannot like yourself");

        var existingLike = await unitOfWork.LikesRepository.GetUserLike(sourceUserId, targetUserId);

        if (existingLike == null)
        {
            var like = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = targetUserId
            };

            unitOfWork.LikesRepository.AddLike(like);
        }
        else
        {
            unitOfWork.LikesRepository.DeleteLike(existingLike);
        }

        if (await unitOfWork.Complete()) return Ok();

        return BadRequest("Failed to updated like");
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
    {
        return Ok(await unitOfWork.LikesRepository.GetCurrentUserLikeIds(User.GetUserId()));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MembersDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
    {

        likesParams.UserId = User.GetUserId();
        var users = await unitOfWork.LikesRepository.GetUserLikes(likesParams);

        Response.AddPaginationHeader(users);
        return Ok(users);
    }
}

// The User property is populated by the ASP.NET Core authentication middleware, which extracts claims from the 
// authenticated user's token or session. This process usually depends on the authentication scheme 
// (e.g., JWT, cookies, etc.).
// When a request is made, the authentication middleware validates the user's credentials 
// (e.g., from a JWT token in the Authorization header).
// If successful, the user's claims are attached to the HttpContext.User property, which the
//  ControllerBase.User property accesses.