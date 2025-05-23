using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
using Microsoft.Extensions.Logging;
using static Blazr.Antimony.Tests.MediatorTests;
using Blazr.Antimony.Mediator;

namespace Blazr.Antimony.Tests;

public record UserDto(int Id, string Name);

public record GetUserQuery(int Id) : IRequest<Result<UserDto>>;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<UserDto>>
{
    public Task<Result<UserDto>> HandleAsync(GetUserQuery request, CancellationToken cancellationToken)
    {
        // Simulated data
        return Task.FromResult(Result<UserDto>.Success(new UserDto(
            Id: request.Id,
            Name: $"Test User {request.Id}"
        )));
    }
}

public class MediatorTests
{

    public MediatorTests()
    {
    }

    private ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddScoped<IMediatorBroker, MediatorBroker>();
        services.AddTransient<IRequestHandler<GetUserQuery, Result<UserDto>>, GetUserQueryHandler>();
        //services.AddLogging(builder => builder.AddDebug());

        var provider = services.BuildServiceProvider();

        return provider!;
    }

    [Fact]
    public async Task TestHandler()
    {
        var service = GetServiceProvider();
        var mediatorBroker = service.GetRequiredService<IMediatorBroker>();
        int id = 1;
        var expectedUser = new UserDto(
            Id: id,
            Name: $"Test User {id}"
        );
        UserDto? resultUser = null;

        var result = await mediatorBroker.DispatchAsync(new GetUserQuery(id));

        Assert.True(result.HasSucceeded(out resultUser));

        Assert.Equal(resultUser, expectedUser);
    }
}
