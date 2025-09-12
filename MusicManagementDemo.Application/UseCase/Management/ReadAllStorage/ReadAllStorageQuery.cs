using MediatR;
using MusicManagementDemo.Abstractions;

namespace MusicManagementDemo.Application.UseCase.Management.ReadAllStorage;

public sealed record ReadAllStorageQuery(int? ReferenceId, int PageSize, bool Asc) : IRequest<IServiceResult>;
