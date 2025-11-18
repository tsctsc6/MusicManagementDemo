using Mediator;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Abstractions.IDbContext;
using MusicManagementDemo.Application.Responses;
using static MusicManagementDemo.Application.Responses.ApiResult<MusicManagementDemo.Application.UseCase.Management.ReadAllStorage.ReadAllStorageQueryResponse>;

namespace MusicManagementDemo.Application.UseCase.Management.ReadAllStorage;

internal sealed class ReadAllStorageQueryHandler(IManagementAppDbContext dbContext)
    : IRequestHandler<ReadAllStorageQuery, ApiResult<ReadAllStorageQueryResponse>>
{
    public async ValueTask<ApiResult<ReadAllStorageQueryResponse>> Handle(
        ReadAllStorageQuery request,
        CancellationToken cancellationToken
    )
    {
        var storagesQueryable = dbContext.Storages.AsQueryable();
        if (request.Asc)
        {
            storagesQueryable = storagesQueryable.OrderBy(e => e.Id);
            if (request.ReferenceId is not null)
            {
                storagesQueryable = storagesQueryable.Where(e => e.Id > request.ReferenceId);
            }
        }
        else
        {
            storagesQueryable = storagesQueryable.OrderByDescending(e => e.Id);
            if (request.ReferenceId is not null)
            {
                storagesQueryable = storagesQueryable.Where(e => e.Id < request.ReferenceId);
            }
        }
        var storagesToRead = await storagesQueryable
            .Take(request.PageSize)
            .AsNoTracking()
            .Select(e => new ReadAllStorageQueryResponseItem(e.Id, e.Name, e.Path))
            .ToArrayAsync(cancellationToken: cancellationToken);
        return Ok(new ReadAllStorageQueryResponse(storagesToRead));
    }
}
