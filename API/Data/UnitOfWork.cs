using API.interfaces;

namespace API.Data;

public class UnitOfWork(DataContext context, IUserRespository userRespository, IMessageRepository messageRepository, ILikesRepository likesRepository, IPhotoRepository photoRepository) : IUnitOfWork
{
    public IUserRespository UserRespository => userRespository;

    public IMessageRepository MessageRepository => messageRepository;

    public ILikesRepository LikesRepository => likesRepository;

    public IPhotoRepository PhotoRepository { get; } = photoRepository;

    public async Task<bool> Complete()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return context.ChangeTracker.HasChanges();
    }
}
