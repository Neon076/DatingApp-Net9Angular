using API.Data;
using API.DTOs;
using API.Entities;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

// [ApiController]
// [Route("api/[controller]")]   // /api/users

// [controller] is replaces with 'Users' from UsersController
[Authorize]
public class UsersController(IUserRespository userRespository) : BaseApiController
{
    // private readonly DataContext _context = context;
    [HttpGet]

    public async Task<ActionResult<IEnumerable<MembersDto>>> GetUsers()
    {
        var users = await userRespository.GetMembersAsync();

        // var usersToReturn = mapper.Map<IEnumerable<MembersDto>>(users);

        return Ok(users);
    }

    // [HttpGet("{id:int}")] // /api/users/2

    // public async Task<ActionResult<AppUser>> GetUser(int id)
    // {
    //     var user = await userRespository.GetUserByIdAsync(id);

    //     if(user == null) return NotFound();

    //     return user;
    // }
    [HttpGet("{username}")] // /api/users/{name}

    public async Task<ActionResult<MembersDto>> GetUser(string username)
    {
        var user = await userRespository.GetMemberAsync(username);

        if(user == null) return NotFound();

        return user;
    }
}