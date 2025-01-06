using API.DTOs;
using API.Entities;
using API.Helpers;
using API.interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class LikesRepository(DataContext context, IMapper mapper) : ILikesRepository
{
    public void AddLike(UserLike like)
    {
        context.Likes.Add(like);
    }

    public void DeleteLike(UserLike like)
    {
        context.Likes.Remove(like);
    }

    public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
    {
        return await context.Likes
            .Where(x => x.SourceUserId == currentUserId)
            .Select(x => x.TargetUserId)
            .ToListAsync();
    }

    public async Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await context.Likes.FindAsync(sourceUserId, targetUserId);
    }

    public async Task<PagedList<MembersDto>> GetUserLikes(LikesParams likeParams)
    {
        var likes = context.Likes.AsQueryable();
        IQueryable<MembersDto> query;
        switch (likeParams.Predicate)
        {
            case "liked":
                query = likes.Where(x => x.SourceUserId == likeParams.UserId)
                        .Select(x => x.TargetUser)
                        .ProjectTo<MembersDto>(mapper.ConfigurationProvider);
                break;
            case "likedBy":
                query = likes.Where(x => x.TargetUserId == likeParams.UserId)
                        .Select(x => x.SourceUser)
                        .ProjectTo<MembersDto>(mapper.ConfigurationProvider);
                break;
            default:
                var likeIds = await GetCurrentUserLikeIds(likeParams.UserId);

                query = likes
                        .Where(x => x.TargetUserId == likeParams.UserId && likeIds.Contains(x.SourceUserId))
                        .Select(x => x.SourceUser)
                        .ProjectTo<MembersDto>(mapper.ConfigurationProvider);
                break;
        }

        return await PagedList<MembersDto>.CreateAsync(query, likeParams.PageNumber, likeParams.PageSize);
    }
}
