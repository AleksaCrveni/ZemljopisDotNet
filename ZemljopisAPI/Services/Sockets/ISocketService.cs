using ZemljopisAPI.DTOs.WS;

namespace ZemljopisAPI.Services.Sockets;

public interface ISocketService
{
  // @TODO think about string or buffer return
  public Task<SocketResult> ProcessSocketData(string socketString);
}