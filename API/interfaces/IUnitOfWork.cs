namespace API.interfaces;

public interface IUnitOfWork
{
    IUserRespository UserRespository { get; }
    IMessageRepository MessageRepository { get; }
    ILikesRepository LikesRepository { get; }
    IPhotoRepository PhotoRepository {get;}
    Task<bool> Complete();
    bool HasChanges();
}
