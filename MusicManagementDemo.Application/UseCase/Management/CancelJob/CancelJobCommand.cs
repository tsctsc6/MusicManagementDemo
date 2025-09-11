using MediatR;
using MusicManagementDemo.Abstractions;

namespace MusicManagementDemo.Application.UseCase.Management.CancelJob;

public sealed record CancelJobCommand(long JobId) : IRequest<IServiceResult>;
