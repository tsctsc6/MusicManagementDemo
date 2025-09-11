using MediatR;
using Microsoft.EntityFrameworkCore;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;

namespace MusicManagementDemo.Application.UseCase.Management.ReadAllStorage;

internal sealed class ReadAllStorageQueryHandler(IManagementAppDbContext dbContext)
    : IRequestHandler<ReadAllStorageQuery, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        ReadAllStorageQuery request,
        CancellationToken cancellationToken
    )
    {
        var storagesToRead = await dbContext
            .Storage.AsNoTracking()
            .Skip(request.Page * request.PageSize)
            .Take(request.PageSize)
            .ToArrayAsync(cancellationToken: cancellationToken);

        return ServiceResult.Ok(storagesToRead);
    }
}
