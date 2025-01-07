using API.DTOs;
using API.Entities;
using API.interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class PhotoRepository(DataContext context) : IPhotoRepository
{
    public async Task<Photo?> GetPhotoById(int Id)
    {
        return await context.Photos
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == Id);
    }

    public async Task<IEnumerable<PhotoForApprovalDto>> GetUnApprovedPhotos()
    {
        return await context.Photos
            .IgnoreQueryFilters()
            .Where(x=>!x.IsApproved)
            .Select(u => new PhotoForApprovalDto
            {
                Id = u.Id,
                Username = u.AppUser.UserName,
                IsApproved = u.IsApproved,
                Url = u.Url
            })
            .ToListAsync();
    }

    public void RemovePhoto(Photo photo)
    {
        context.Photos.Remove(photo);
    }
}
