using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Music.ReadAllMusicInfo;

internal sealed class ReadAllMusicInfoQueryHandler(IMusicAppDbContext dbContext)
    : IRequestHandler<ReadAllMusicInfoQuery, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        ReadAllMusicInfoQuery request,
        CancellationToken cancellationToken
    )
    {
        var musicInfosToReadQuery = dbContext.MusicInfo.AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            musicInfosToReadQuery = musicInfosToReadQuery
                .Where(m =>
                    m.TitleTSV.Matches(
                        EF.Functions.ToTsQuery(
                            NpgsqlValues.TsConfigSimple,
                            request.SearchTerm.Replace(" ", "|")
                        )
                    )
                )
                .OrderByDescending(m =>
                    m.TitleTSV.Rank(
                        EF.Functions.ToTsQuery(
                            NpgsqlValues.TsConfigSimple,
                            request.SearchTerm.Replace(" ", "|")
                        )
                    )
                );
        }
        else
        {
            if (request.Asc)
            {
                musicInfosToReadQuery = musicInfosToReadQuery.OrderBy(e => e.Id);
                if (request.ReferenceId is not null)
                {
                    musicInfosToReadQuery = musicInfosToReadQuery.Where(e =>
                        e.Id > request.ReferenceId
                    );
                }
            }
            else
            {
                musicInfosToReadQuery = musicInfosToReadQuery.OrderByDescending(e => e.Id);
                if (request.ReferenceId is not null)
                {
                    musicInfosToReadQuery = musicInfosToReadQuery.Where(e =>
                        e.Id < request.ReferenceId
                    );
                }
            }
        }
        var musicListsToRead = await musicInfosToReadQuery
            .Take(request.PageSize)
            .Select(m => new
            {
                m.Id,
                m.Title,
                m.Artist,
                m.Album,
            })
            .AsNoTracking()
            .ToArrayAsync(cancellationToken: cancellationToken);
        return ServiceResult.Ok(musicListsToRead);
    }
}
