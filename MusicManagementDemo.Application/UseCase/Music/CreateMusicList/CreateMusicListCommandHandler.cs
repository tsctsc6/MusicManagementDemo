using MediatR;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Domain.Entity.Music;

namespace MusicManagementDemo.Application.UseCase.Music.CreateMusicList;

internal sealed class CreateMusicListCommandHandler(
    IMusicAppDbContext dbContext,
    ILogger<CreateMusicListCommandHandler> logger
) : IRequestHandler<CreateMusicListCommand, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        CreateMusicListCommand request,
        CancellationToken cancellationToken
    )
    {
        var musicListToAdd = new MusicList { Name = request.Name, UserId = request.UserId };
        await dbContext.MusicList.AddAsync(musicListToAdd, cancellationToken);
        try
        {
            if (await dbContext.SaveChangesAsync(cancellationToken) != 1)
            {
                logger.LogError("Create MusicList failed");
                return ServiceResult.Err(503, ["创建失败"]);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Create MisicList failed");
            return ServiceResult.Err(503, ["创建失败"]);
        }
        return ServiceResult.Ok(new { musicListToAdd.Id });
    }
}
