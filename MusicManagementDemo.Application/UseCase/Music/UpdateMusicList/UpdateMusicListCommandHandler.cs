using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Music.UpdateMusicList;

internal sealed class UpdateMusicListCommandHandler(
    IMusicAppDbContext dbContext,
    ILogger<UpdateMusicListCommandHandler> logger
) : IRequestHandler<UpdateMusicListCommand, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        UpdateMusicListCommand request,
        CancellationToken cancellationToken
    )
    {
        var musicListToUpdate = await dbContext.MusicLists.SingleOrDefaultAsync(
            e => e.Id == request.MusicListId && e.UserId == request.UserId,
            cancellationToken: cancellationToken
        );
        if (musicListToUpdate is null)
        {
            logger.LogError("MusicList {MusicListId} not found", request.MusicListId);
            return ServiceResult.Err(404, ["请求的 MusicListId 不存在"]);
        }
        musicListToUpdate.Name = request.Name;
        try
        {
            if (await dbContext.SaveChangesAsync(cancellationToken) != 1)
            {
                logger.LogError("Can't update MusicList {@MusicList}", musicListToUpdate);
                return ServiceResult.Err(503, ["内部错误"]);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Can't update MusicList {@MusicList}", musicListToUpdate);
            return ServiceResult.Err(503, ["内部错误"]);
        }
        return ServiceResult.Ok();
    }
}
