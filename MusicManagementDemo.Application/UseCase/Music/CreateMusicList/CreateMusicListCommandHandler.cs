using MediatR;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Domain.Entity.Music;

namespace MusicManagementDemo.Application.UseCase.Music.CreateMusicList;

public class CreateMusicListCommandHandler(IMusicAppDbContext dbContext)
    : IRequestHandler<CreateMusicListCommand, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        CreateMusicListCommand request,
        CancellationToken cancellationToken
    )
    {
        var musicListToAdd = new MusicList { Name = request.Name, UserId = request.UserId };
        await dbContext.MusicList.AddAsync(musicListToAdd, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResult.Ok(new { musicListToAdd.Id });
    }
}
