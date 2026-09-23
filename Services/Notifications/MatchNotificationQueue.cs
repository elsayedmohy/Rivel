using System.Threading.Channels;

namespace RiverLine.Api.Services.Notifications;

public class MatchNotificationQueue
{
    private readonly Channel<Guid> _channel =
        Channel.CreateBounded<Guid>(new BoundedChannelOptions(1_000)
        {
            FullMode = BoundedChannelFullMode.DropWrite 
        });
 
    public ValueTask EnqueueAsync(Guid requestId) => _channel.Writer.WriteAsync(requestId);
    public IAsyncEnumerable<Guid> ReadAllAsync(CancellationToken ct) =>
        _channel.Reader.ReadAllAsync(ct);
}