using System;
using API.Entities;

namespace API.interfaces;

public interface ITokenInterface
{
    public string CreateToken(AppUser user);
}
