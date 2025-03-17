using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Channels;

namespace Modmail.NET.Abstract;

public abstract class BaseQueue<TKey, TValue> where TKey : notnull
{
  private readonly TimeSpan _idleTimeout;
  private readonly ConcurrentDictionary<TKey, DateTime> _lastActiveTime = new();
  private readonly ConcurrentDictionary<TKey, Channel<TValue>> _queues = new();

  protected BaseQueue(TimeSpan idleTimeout) {
    _idleTimeout = idleTimeout;
  }

  public async Task EnqueueMessage(TKey key, TValue message) {
    _lastActiveTime[key] = DateTime.UtcNow;
    if (!_queues.TryGetValue(key, out var channel)) {
      // Create a new channel for the key and start processing
      channel = Channel.CreateUnbounded<TValue>();
      _queues[key] = channel;

      // Create a new cancellation token source for this key
      _ = ProcessQueue(key, channel);
      _ = ProcessTimeouts(key, channel);
    }

    await channel.Writer.WriteAsync(message);
  }

  private async Task ProcessTimeouts(TKey key, Channel<TValue> channel) {
    while (true) {
      Debug.WriteLine("Waiting for queue timeout");
      await Task.Delay(_idleTimeout);
      if (!_lastActiveTime.TryGetValue(key, out var value)) {
        Debug.WriteLine("Cancelled not exists");
        channel.Writer.Complete();
        break;
      }

      if (DateTime.UtcNow - value >= _idleTimeout) {
        Debug.WriteLine("Cancelled timeout");
        channel.Writer.Complete();
        return;
      }
    }
  }

  private async Task ProcessQueue(TKey key, Channel<TValue> channel) {
    await foreach (var message in channel.Reader.ReadAllAsync()) {
      Debug.WriteLine("handling message");
      await HandleMessageAsync(key, message);
    }

    Debug.WriteLine("removing queue");
    _queues.TryRemove(key, out _);
    _lastActiveTime.TryRemove(key, out _);
  }

  protected abstract Task HandleMessageAsync(TKey key, TValue message);

  public int GetChannelCount() {
    return _queues.Count;
  }
}