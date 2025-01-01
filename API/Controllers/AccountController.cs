using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace API.Controllers;

public class AccountController(UserManager<AppUser> userManager, ITokenInterface tokenService, IMapper mapper) : BaseApiController
{
    [HttpPost("resigter")]  //api/[controller] => account/(resigter = httpPost)

    public async Task<ActionResult<UserDto>> Resigter(ResigterDto resigterDTO)
    {

        if (await UserExists(resigterDTO.Username)) return BadRequest("Username Already Exists");
        // using var hmac = new HMACSHA512();

        var user = mapper.Map<AppUser>(resigterDTO);
        user.UserName = resigterDTO.Username.ToLower();
        // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(resigterDTO.Password));
        // user.PasswordSalt = hmac.Key;

        var result = await userManager.Creat    eAsync(user, resigterDTO.Password);

        if (!result.Succeeded) return BadRequest(result.Errors);

        return new UserDto
        {
            Username = user.UserName,
            Token =await tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {

        var user = await userManager.Users
                    .Include(x => x.Photos)
                    .FirstOrDefaultAsync(x => x.NormalizedUserName == loginDto.Username.ToUpper());

        if (user == null || user.UserName == null) return Unauthorized("Invalid User");

        var result = await userManager.CheckPasswordAsync(user , loginDto.Password);

        if(!result) return Unauthorized("Incorrect Password");

        // using var hmac = new HMACSHA512(user.PasswordSalt);

        // var ComputedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        // for (int i = 0; i < ComputedHash.Length; i++)
        // {
        //     if (ComputedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
        // }

        return new UserDto
        {
            Username = user.UserName,
            Token = await tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await userManager.Users.AnyAsync(x => x.NormalizedUserName == username.ToUpper());
    }
}

