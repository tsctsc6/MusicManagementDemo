﻿using MediatR;
using MusicManagementDemo.Abstractions;

namespace MusicManagementDemo.Application.UseCase.Management.ReadJob;

public sealed record ReadJobQuery(long Id) : IRequest<IServiceResult>;
