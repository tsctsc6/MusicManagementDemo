﻿using MediatR;
using MusicManagementDemo.SharedKernel;

namespace MusicManagementDemo.Application.UseCase.Identity.Register;

public sealed record RegisterCommand(string Email, string UserName, string Password)
    : IRequest<IServiceResult>;
