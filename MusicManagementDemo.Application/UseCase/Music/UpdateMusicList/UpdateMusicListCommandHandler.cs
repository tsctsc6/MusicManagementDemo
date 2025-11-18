using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using static MusicManagementDemo.Application.Responses.ApiResult<MusicManagementDemo.Application.UseCase.Music.UpdateMusicList.UpdateMusicListCommandResponse>;

namespace MusicManagementDemo.Application.UseCase.Music.UpdateMusicList;

internal sealed class UpdateMusicListCommandHandler(
    IMusicAppDbContext dbContext,
    ILogger<UpdateMusicListCommandHandler> logger
) : IRequestHandler<UpdateMusicListCommand, ApiResult<UpdateMusicListCommandResponse>>
{
    public async ValueTask<ApiResult<UpdateMusicListCommandResponse>> Handle(
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
            return Err(404, "请求的 MusicListId 不存在");
        }
        musicListToUpdate.Name = request.Name;
        try
        {
            if (await dbContext.SaveChangesAsync(cancellationToken) != 1)
            {
                logger.LogError("Can't update MusicList {@MusicList}", musicListToUpdate);
                return Err(503, "内部错误");
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Can't update MusicList {@MusicList}", musicListToUpdate);
            return Err(503, "内部错误");
        }
        return Ok(new UpdateMusicListCommandResponse());
    }
}
