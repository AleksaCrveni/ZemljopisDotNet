using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace ZemljopisAPI.Helpers;

public class SocketManager
{
  public static readonly ConcurrentDictionary<string, List<WebSocket>> Rooms =
    new ConcurrentDictionary<string, List<WebSocket>>();


  public static readonly ConcurrentDictionary<string, WebSocket> PlayerSockets =
    new ConcurrentDictionary<string, WebSocket>();
}