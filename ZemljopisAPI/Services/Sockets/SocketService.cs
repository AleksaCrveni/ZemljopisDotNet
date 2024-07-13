using ZemljopisAPI.DI;

namespace ZemljopisAPI.Services.Sockets;

public class SocketService(ILogger<SocketService> _logger) : ISocketService, ITransient
{
  public Task<string> ProcessSocketData(string socketData)
  {
    throw new NotImplementedException();
  }
}