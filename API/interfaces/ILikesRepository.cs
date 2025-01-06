using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.interfaces;

public interface ILikesRepository
{
    Task<UserLike?> GetUserLike(int sourceUserId , int targetUserId);
    Task<PagedList<MembersDto>> GetUserLikes (LikesParams likesParams);
    Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId);
    void DeleteLike(UserLike like);
    void AddLike(UserLike like);
}
