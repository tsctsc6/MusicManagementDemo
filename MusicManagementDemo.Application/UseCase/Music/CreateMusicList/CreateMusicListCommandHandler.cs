using MediatR;
using MusicManagementDemo.Abstractions;

namespace MusicManagementDemo.Application.UseCase.Music.CreateMusicList;

public class CreateMusicListCommandHandler : IRequestHandler<CreateMusicListCommand, IServiceResult>
{
    public Task<IServiceResult> Handle(
        CreateMusicListCommand request,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }
}
