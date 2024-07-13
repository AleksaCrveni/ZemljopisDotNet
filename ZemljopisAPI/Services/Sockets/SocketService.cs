using Newtonsoft.Json;
using ZemljopisAPI.DI;
using ZemljopisAPI.DTOs.WS;

namespace ZemljopisAPI.Services.Sockets;

public class SocketService(ILogger<SocketService> _logger) : ISocketService, ITransient
{
  public async Task<SocketResult> ProcessSocketData(string socketString)
  {
    SocketData? data;
    try
    {
      data = JsonConvert.DeserializeObject<SocketData>(socketString);
    } catch(Exception e)
    {
      // this is basically 400 response
      return new SocketResult() { Evt = Events.ERR };
    }

    if (data is null)
    {
      return new SocketResult() { Evt = Events.ERR };
    }

    return new SocketResult() { Evt = data.Evt, ResponseString = "OK"};
  }
}