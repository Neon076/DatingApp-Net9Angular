using API.DTOs;
using API.Entities;

namespace API.interfaces;

public interface IPhotoRepository
{
    public Task<IEnumerable<PhotoForApprovalDto>> GetUnApprovedPhotos();
    public Task<Photo?> GetPhotoById(int Id);
    void RemovePhoto(Photo photo);
}
