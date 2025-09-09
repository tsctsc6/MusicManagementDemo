﻿using FluentValidation;

namespace MusicManagementDemo.Application.UseCase.Management.ReadJob;

public class ReadJobQueryValidator : AbstractValidator<ReadJobQuery>
{
    public ReadJobQueryValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(e => e.Id).NotEmpty().GreaterThan(0);
    }
}
