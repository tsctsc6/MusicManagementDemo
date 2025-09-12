using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Music.AddMusicInfoToMusicList;

internal sealed class AddMusicInfoToMusicListCommandHandler(IMusicAppDbContext dbContext)
    : IRequestHandler<AddMusicInfoToMusicListCommand, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        AddMusicInfoToMusicListCommand request,
        CancellationToken cancellationToken
    )
    {
        var musicListWhichAddMusicInfo = await dbContext.MusicList.SingleOrDefaultAsync(
            e => e.Id == request.MusicInfoId && e.UserId == request.UserId,
            cancellationToken: cancellationToken
        );
        if (musicListWhichAddMusicInfo is null)
        {
            return ServiceResult.Err(404, ["没有找到歌单"]);
        }
        await dbContext.MusicInfoMusicListMap.AddAsync(
            new() { MusicListId = request.MusicListId, MusicInfoId = request.MusicInfoId },
            cancellationToken
        );
        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResult.Ok();
    }
}
