using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Music.ReadAllMusicInfo;

internal sealed partial class ReadAllMusicInfoQueryHandler(
    IMusicAppDbContext dbContext,
    ILogger<ReadAllMusicInfoQueryHandler> logger
) : IRequestHandler<ReadAllMusicInfoQuery, IServiceResult>
{
    public async ValueTask<IServiceResult> Handle(
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
            .Select(m => new ReadAllMusicInfoQueryResponse(m.Id, m.Title, m.Artist, m.Album))
            .ToArrayAsync(cancellationToken: cancellationToken);
        return ApiResult<>.Ok(
            new ReadOnlyCollection<ReadAllMusicInfoQueryResponse>(musicListsToRead)
        );
    }

    [GeneratedRegex(@"\s+", RegexOptions.None, 3000)]
    private static partial Regex WhiteSpaceRegex();
}
