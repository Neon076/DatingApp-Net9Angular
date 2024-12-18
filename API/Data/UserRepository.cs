using System;
using API.DTOs;
using API.Entities;
using API.interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserRepository(DataContext context, IMapper mapper) : IUserRespository
{
    public async Task<IEnumerable<AppUser>> GetAppUsersAsync()
    {
        return await context.Users
        .Include(x => x.Photos)
        .ToListAsync();
    }

    public async Task<MembersDto?> GetMemberAsync(string username)
    {
        return await context.Users
        .Where(x => x.UserName == username)
        .ProjectTo<MembersDto>(mapper.ConfigurationProvider)
        .SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<MembersDto>> GetMembersAsync()
    {
        return await context.Users
                    .ProjectTo<MembersDto>(mapper.ConfigurationProvider)
                    .ToListAsync();
    }


    public async Task<MembersDto?> GetMembersAsync(string username)
    {
        return await context.Users
        .Where(x =>x.UserName == username)
        .ProjectTo<MembersDto>(mapper.ConfigurationProvider)
        .SingleOrDefaultAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void UpdateUser(AppUser user)
    {
        context.Entry(user).State = EntityState.Modified;
    }

    public async Task<AppUser?> GetUserByUsernameAsync(string username)
    {
        return await context.Users
        .Include(x => x.Photos)
        .SingleOrDefaultAsync(x => x.UserName == username);
    }

    public async Task<MembersDto?> GetMemberByIdAsync(int id)
    {
        return await context.Users
        .Where(x => x.Id == id)
        .ProjectTo<MembersDto>(mapper.ConfigurationProvider)
        .SingleOrDefaultAsync();
    }
}
