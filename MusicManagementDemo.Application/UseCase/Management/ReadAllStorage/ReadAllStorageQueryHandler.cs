using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;

namespace MusicManagementDemo.Application.UseCase.Management.ReadAllStorage;

internal sealed class ReadAllStorageQueryHandler(IManagementAppDbContext dbContext)
    : IRequestHandler<ReadAllStorageQuery, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        ReadAllStorageQuery request,
        CancellationToken cancellationToken
    )
    {
        var storagesToRead = dbContext.Storage.AsQueryable();
        if (request.Asc)
        {
            storagesToRead = storagesToRead.OrderBy(e => e.Id);
            if (request.ReferenceId is not null)
            {
                storagesToRead = storagesToRead.Where(e =>
                    e.Id > request.ReferenceId
                );
            }
        }
        else
        {
            storagesToRead = storagesToRead.OrderByDescending(e => e.Id);
            if (request.ReferenceId is not null)
            {
                storagesToRead = storagesToRead.Where(e =>
                    e.Id < request.ReferenceId
                );
            }
        }
        var musicListsToRead = await storagesToRead
            .Take(request.PageSize)
            .AsNoTracking()
            .ToArrayAsync(cancellationToken: cancellationToken);
        return ServiceResult.Ok(musicListsToRead);
    }
}
