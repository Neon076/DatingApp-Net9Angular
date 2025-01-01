using System;
using API.Entities;

namespace API.interfaces;

public interface ITokenInterface
{
    public  Task<string> CreateToken(AppUser user);
}
