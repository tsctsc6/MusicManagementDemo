using MediatR;
using MusicManagementDemo.SharedKernel;

namespace MusicManagementDemo.Application.UseCase.Management.ReadAllStorage;

public sealed record ReadAllStorageQuery(int Page, int PageSize) : IRequest<IServiceResult>;
