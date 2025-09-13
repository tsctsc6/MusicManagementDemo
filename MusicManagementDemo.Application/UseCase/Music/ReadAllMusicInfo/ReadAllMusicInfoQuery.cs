﻿using MediatR;
using MusicManagementDemo.Abstractions;

namespace MusicManagementDemo.Application.UseCase.Music.ReadAllMusicInfo;

public sealed record ReadAllMusicInfoQuery(
    Guid? ReferenceId,
    int PageSize,
    bool Asc,
    string SearchTerm
) : IRequest<IServiceResult>;
