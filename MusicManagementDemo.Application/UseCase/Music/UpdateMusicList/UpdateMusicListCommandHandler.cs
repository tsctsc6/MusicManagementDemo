using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Music.UpdateMusicList;

internal sealed class UpdateMusicListCommandHandler(IMusicAppDbContext dbContext)
    : IRequestHandler<UpdateMusicListCommand, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        UpdateMusicListCommand request,
        CancellationToken cancellationToken
    )
    {
        var musicListToUpdate = await dbContext.MusicList.SingleOrDefaultAsync(
            e => e.Id == request.MusicListId,
            cancellationToken: cancellationToken
        );
        if (musicListToUpdate is null)
        {
            return ServiceResult.Err(404, ["请求的 MusicListId 不存在"]);
        }
        musicListToUpdate.Name = request.Name;
        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResult.Ok();
    }
}
