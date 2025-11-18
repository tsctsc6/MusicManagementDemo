using System.Text.RegularExpressions;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using static MusicManagementDemo.Application.Responses.ApiResult<MusicManagementDemo.Application.UseCase.Music.ReadAllMusicInfo.ReadAllMusicInfoQueryResponse>;

namespace MusicManagementDemo.Application.UseCase.Music.ReadAllMusicInfo;

internal sealed partial class ReadAllMusicInfoQueryHandler(
    IMusicAppDbContext dbContext,
    ILogger<ReadAllMusicInfoQueryHandler> logger
) : IRequestHandler<ReadAllMusicInfoQuery, ApiResult<ReadAllMusicInfoQueryResponse>>
{
    public async ValueTask<ApiResult<ReadAllMusicInfoQueryResponse>> Handle(
        ReadAllMusicInfoQuery request,
        CancellationToken cancellationToken
    )
    {
        var musicInfosToReadQuery = dbContext.MusicInfos.AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = WhiteSpaceRegex().Replace(request.SearchTerm.Trim(), "|");
            logger.LogInformation("Processed search term: \"{searchTerm}\"", searchTerm);
            musicInfosToReadQuery = musicInfosToReadQuery
                .Where(m =>
                    m.TitleTSV.Matches(
                        EF.Functions.ToTsQuery(NpgsqlValues.TsConfigSimple, searchTerm)
                    )
                )
                .OrderByDescending(m =>
                    m.TitleTSV.Rank(EF.Functions.ToTsQuery(NpgsqlValues.TsConfigSimple, searchTerm))
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
            .AsNoTracking()
            .Select(m => new ReadAllMusicInfoQueryResponseItem(m.Id, m.Title, m.Artist, m.Album))
            .ToArrayAsync(cancellationToken: cancellationToken);
        return Ok(new ReadAllMusicInfoQueryResponse(musicListsToRead));
    }

    [GeneratedRegex(@"\s+", RegexOptions.None, 3000)]
    private static partial Regex WhiteSpaceRegex();
}
