using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.interfaces;

public interface IUserRespository
{
    void UpdateUser(AppUser user);
    Task<bool> SaveAllAsync();
    Task<IEnumerable<AppUser>> GetAppUsersAsync();
    Task<MembersDto?> GetMemberByIdAsync(int id);
    Task<AppUser?> GetUserByUsernameAsync(string username);
    Task<AppUser?> GetUserByUserIdAsync(int id);
    Task<PagedList<MembersDto>> GetMembersAsync(UserParams userParams);
    Task<MembersDto?> GetMemberAsync(string username);
}
