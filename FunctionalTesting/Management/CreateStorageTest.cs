﻿using System.Text;
using MusicManagementDemo.Application.UseCase.Management.CreateStorage;

namespace FunctionalTesting.Management;

public class CreateStorageTest : BaseTestingClass
{
    [Fact]
    public async Task Normal()
    {
        var result = await mediator.Send(
            new CreateStorageCommand("Test", "X:\\storage1"),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
    }

    [Fact]
    public async Task Repeatedly()
    {
        await mediator.Send(
            new CreateStorageCommand("Test", "X:\\storage1"),
            TestContext.Current.CancellationToken
        );
        var result = await mediator.Send(
            new CreateStorageCommand("Test", "X:\\storage1"),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }

    [Fact]
    public async Task InvalidName()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 10; i++)
        {
            sb.Append("abdefghi123");
        }
        var result = await mediator.Send(
            new CreateStorageCommand(sb.ToString(), "X:\\storage1"),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }

    [Fact]
    public async Task InvalidPath()
    {
        var sb = new StringBuilder("X:\\");
        for (int i = 0; i < 22; i++)
        {
            sb.Append("abdefghi123\\");
        }
        var result = await mediator.Send(
            new CreateStorageCommand("Test", sb.ToString()),
            TestContext.Current.CancellationToken
        );
        Assert.NotNull(result);
        Assert.NotEqual(200, result.Code);
    }
}
