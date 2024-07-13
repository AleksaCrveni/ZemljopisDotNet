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
      // @TODO Late on change from JSON to string buffer type
      data = JsonConvert.DeserializeObject<SocketData>(socketString);
    } catch(Exception e)
    {
      // this is basically 400 response
      return new SocketResult() { Evt = Events.ERR, Success = false };
    }
    if (data is null)
    {
      return new SocketResult() { Evt = Events.ERR, Success = false };
    }

    await Dispatch(data);
    return new SocketResult() { Evt = data.Evt, ResponseString = "OK"};
  }

  private async Task Dispatch(SocketData data)
  {
    // @TODO think about what to return
    var _ = (data.Evt) switch
    {
      Events.GET_ROOM_DATA => "",
      Events.ERR => "",
      _ => ""
    };
  }
}