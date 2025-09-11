using MediatR;
using MusicManagementDemo.Application.Responses;
using MusicManagementDemo.Abstractions;
using MusicManagementDemo.Abstractions.IDbContext;

namespace MusicManagementDemo.Application.UseCase.Management.CreateStorage;

internal sealed class CreateStorageCommandHandler(IManagementAppDbContext dbContext)
    : IRequestHandler<CreateStorageCommand, IServiceResult>
{
    public async Task<IServiceResult> Handle(
        CreateStorageCommand request,
        CancellationToken cancellationToken
    )
    {
        await dbContext.Storage.AddAsync(
            new() { Name = request.Name, Path = request.Path },
            cancellationToken
        );
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception)
        {
            return ServiceResult.Err(503, ["内部错误"]);
        }
        return ServiceResult.Ok();
    }
}
