using MediatR;
using MusicManagementDemo.SharedKernel;

namespace MusicManagementDemo.Application.UseCase.Management.ReadAllStorage;

public record ReadAllStorageQuery(int Page, int PageSize) : IRequest<IServiceResult>;
