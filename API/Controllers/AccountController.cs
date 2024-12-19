using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace API.Controllers;

public class AccountController(DataContext context, ITokenInterface tokenService , IMapper mapper) : BaseApiController
{
    [HttpPost("resigter")]  //api/[controller] => account/(resigter = httpPost)

    public async Task<ActionResult<UserDto>> Resigter(ResigterDto resigterDTO)
    {

        if (await UserExists(resigterDTO.Username)) return BadRequest("Username Already Exists");
        using var hmac = new HMACSHA512();

        var user = mapper.Map<AppUser>(resigterDTO);
        user.UserName = resigterDTO.Username.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(resigterDTO.Password));
        user.PasswordSalt = hmac.Key;
    
        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new UserDto
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user),
            KnownAs = user.KnownAs
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        
        var user = await context.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == loginDto.Username.ToLower());

        if (user == null) return Unauthorized("Invalid User");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var ComputedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < ComputedHash.Length; i++)
        {
            if (ComputedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
        }

        return new UserDto
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            KnownAs = user.KnownAs
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }
}

