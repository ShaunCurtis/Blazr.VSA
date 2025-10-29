/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;
using Blazr.Diode;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.Cadmium.Presentation;

public class EditUIBrokerFactory : IEditUIBrokerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public EditUIBrokerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<IEditUIBroker<TRecord, TMutor, TKey>> GetAsync<TRecord, TMutor, TKey>(TKey id)
        where TMutor : IRecordMutor<TRecord>
        where TKey : notnull, IEntityId
        where TRecord : class, new()
    {
        var presenter = _serviceProvider.GetRequiredService<IEditUIBroker<TRecord, TMutor, TKey>>();
        await presenter.LoadAsync(id);

        return presenter;
    }
}
