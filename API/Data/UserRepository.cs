using System;
using API.DTOs;
using API.Entities;
using API.Helpers;
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

    public async Task<PagedList<MembersDto>> GetMembersAsync(UserParams userParams)
    {
        var query = context.Users.AsQueryable();

        query = query.Where(x => x.UserName != userParams.CurrentUsername);

        if (userParams.Gender != null)
        {
            query = query.Where(x => x.Gender == userParams.Gender);
        }

        var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
        var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

        query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);

        query = userParams.OrderBy switch
        {
            "created" => query.OrderByDescending(x => x.Created),
            _ => query.OrderByDescending(x => x.LastActive)
        };

        return await PagedList<MembersDto>.CreateAsync(query.ProjectTo<MembersDto>(mapper.ConfigurationProvider), userParams.PageNumber, userParams.PageSize);
    }

    public void UpdateUser(AppUser user)
    {
        context.Entry(user).State = EntityState.Modified;
    }

    public async Task<AppUser?> GetUserByUserIdAsync(int id)
    {
        return await context.Users.FindAsync(id);
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
