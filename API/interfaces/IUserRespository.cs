using API.DTOs;
using API.Entities;

namespace API.interfaces;

public interface IUserRespository
{
    void UpdateUser(AppUser user);
    Task<bool> SaveAllAsync();
    Task<IEnumerable<AppUser>> GetAppUsersAsync();
    Task<AppUser?> GetUserByIdAsync(int id);
    Task<AppUser?> GetUserByUsernameAsync(string username);
    Task<IEnumerable<MembersDto>> GetMembersAsync();
    Task<MembersDto?> GetMemberAsync(string username);
}
