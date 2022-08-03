using Discord.Interactions;
using Discord.WebSocket;

namespace OracleCommands;

public class ScopedSocketInteractionContext : SocketInteractionContext, IDisposable
{
    private bool disposedValue;
    public IServiceScope ServiceScope { get; }

    public ScopedSocketInteractionContext(DiscordSocketClient client, SocketInteraction interaction, IServiceProvider services) : base(client, interaction)
    {
        ServiceScope = services.CreateScope();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                ServiceScope.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

public class ScopedSocketInteractionContext<TInteraction> : SocketInteractionContext<TInteraction>, IDisposable where TInteraction : SocketInteraction
{
    private bool disposedValue;
    public IServiceScope ServiceScope { get; }

    public ScopedSocketInteractionContext(DiscordSocketClient client, TInteraction interaction, IServiceProvider services) : base(client, interaction)
    {
        ServiceScope = services.CreateScope();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                ServiceScope.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
