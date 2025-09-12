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
        var storagesToRead = await dbContext
            .Storage.Skip(request.Page * request.PageSize)
            .Take(request.PageSize)
            .AsNoTracking()
            .ToArrayAsync(cancellationToken: cancellationToken);

        return ServiceResult.Ok(storagesToRead);
    }
}
