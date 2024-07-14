using Newtonsoft.Json;
using ZemljopisAPI.DI;
using ZemljopisAPI.DTOs.WS;
using ZemljopisAPI.Helpers;

namespace ZemljopisAPI.Services.Sockets;

public class SocketService(ILogger<SocketService> _logger, IDB _dbRepository) : ISocketService, ITransient
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

    object? response = await Dispatch(data);

    if (response is not null)
      return new SocketResult() { Evt = data.Evt, Response = response};

    return new SocketResult() { Evt = Events.ERR, Response = "", Success = false};
  }

  private async Task<object?> Dispatch(SocketData data)
  {
    // @TODO think about what to return
    object response = (data.Evt) switch
    {
      Events.GET_ROOM_DATA => await _dbRepository.RetrieveAllRoomData(data.RoomCode),
      Events.ERR => "",
      _ => null
    };
    return response;
  }
}