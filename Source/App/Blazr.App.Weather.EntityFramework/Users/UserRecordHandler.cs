/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Weather.EntityFramework;

/// <summary>
/// Mediatr Handler for executing record requests to get a WeatherForecast Entity
/// </summary>
public sealed class UserRecordHandler : IRequestHandler<UserRecordRequest, Result<DmoUser>>
{
    private IDbContextFactory<InMemoryWeatherTestDbContext> _factory;

    public UserRecordHandler(IDbContextFactory<InMemoryWeatherTestDbContext> dbContextFactory)
    {
        _factory = dbContextFactory;
    }

    public async Task<Result<DmoUser>> Handle(UserRecordRequest request, CancellationToken cancellationToken)
    {
        using var dbContext = _factory.CreateDbContext();

        Expression<Func<DvoUser, bool>> findExpression = (item) =>
            item.UserID == request.Id.Value;

        var query = new RecordQueryRequest<DvoUser>(findExpression);

        var result = await dbContext.GetRecordAsync<DvoUser>(query);

        if (!result.HasSucceeded(out DvoUser? record))
            return result.ConvertFail<DmoUser>();

        var returnItem = UserMap.Map(record);

        return Result<DmoUser>.Success(returnItem);
    }
}
