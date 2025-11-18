using Mediator;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Domain.Entity.Music;
using static MusicManagementDemo.Application.Responses.ApiResult<MusicManagementDemo.Application.UseCase.Music.CreateMusicList.CreateMusicListCommandResponse>;

namespace MusicManagementDemo.Application.UseCase.Music.CreateMusicList;

internal sealed class CreateMusicListCommandHandler(
    IMusicAppDbContext dbContext,
    ILogger<CreateMusicListCommandHandler> logger
) : IRequestHandler<CreateMusicListCommand, ApiResult<CreateMusicListCommandResponse>>
{
    public async ValueTask<ApiResult<CreateMusicListCommandResponse>> Handle(
        CreateMusicListCommand request,
        CancellationToken cancellationToken
    )
    {
        var musicListToAdd = new MusicList { Name = request.Name, UserId = request.UserId };
        await dbContext.MusicLists.AddAsync(musicListToAdd, cancellationToken);
        try
        {
            if (await dbContext.SaveChangesAsync(cancellationToken) != 1)
            {
                logger.LogError("Create MusicList failed");
                return Err(503, "创建失败");
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Create MisicList failed");
            return Err(503, "创建失败");
        }
        return Ok(new CreateMusicListCommandResponse(musicListToAdd.Id));
    }
}
