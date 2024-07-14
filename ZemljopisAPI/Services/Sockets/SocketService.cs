using System.Net.Sockets;
using System.Net.WebSockets;
using Newtonsoft.Json;
using ZemljopisAPI.DI;
using ZemljopisAPI.DTOs.Room;
using ZemljopisAPI.DTOs.WS;
using ZemljopisAPI.Helpers;

namespace ZemljopisAPI.Services.Sockets;

public class SocketService(ILogger<SocketService> _logger, IDB _dbRepository) : ISocketService, ITransient
{
  public async Task<SocketResult> ProcessSocketData(string socketString, WebSocket socket)
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

    object? response = await Dispatch(data, socket);

    if (response is null)
      return new SocketResult() { Evt = data.Evt, Response = "", Success = false};

    return new SocketResult() { Evt = data.Evt, Response = response};
  }

  private async Task<object?> Dispatch(SocketData data, WebSocket socket)
  {
    // @TODO think about what to return
    object response = (data.Evt) switch
    {
      Events.JOIN_ROOM => await JoinRoom(data.RoomCode, data.Data, socket),
      Events.GET_ROOM_DATA => await _dbRepository.RetrieveAllRoomData(data.RoomCode),
      Events.ERR => null,
      _ => null
    };
    return response;
  }

  /*
   * @NOTE this method is just adding socket to room manager and retrieving data
   * Inserting username and stuff is done over normal HTTP request in RoomService
   * null indicates error, empty class indicates some data logic is flaws, like player not registed
   */
  private async Task<AllRoomData?> JoinRoom(string roomCode, string dataString, WebSocket socket)
  {
    JoinRoomData? data;
    try
    {
      data = JsonConvert.DeserializeObject<JoinRoomData>(dataString);
    }
    catch (Exception ex)
    {
      return null;
    }

    if (data is null)
      return null;
    var result = await _dbRepository.RetrieveAllRoomData(roomCode);

    // @TODO Refactor to check th is on db side?
    if (result is not null && result.RoomData.Length > 0 && result.PlayerList.Length > 0)
      return null;

    bool playerRegistered = false;
    // @TODO Think to see if I care about this to be case sensitive or not
    // @TODO Think about if I want to allow only certain characters as username
    //  - for example, allow only Cyrilic and Latinic Serbian letters
    var playerExists = result.PlayerList.ToList().Contains(data.Username);
    if (!playerExists)
      return null;

    // This is not atomic but it's ok
    // Normal user won't get in this race condition, it can happen only for someone trying to exploit
    // In which case, older socket might be kept which is fine
    // Point is that only one socket per player should exist.
    // @TODO don't allow same player name as the room!!!
    string key = $"{roomCode}:{data.Username}";
    var added = SocketManager.PlayerSockets.AddOrUpdate(key, socket, (key, oldSocket) =>
    {
      // @TODO Sync is ok?
      CloseSocketConnection(oldSocket);
      return socket;
    });
    // @TODO Have one socket manager with username and socket pair because I have to iterate twice otherwise
    // @TODO Pass new connection parameter to know if its new socket or old one
    // @TODO have generic method for this??

    return result;
  }

  private void CloseSocketConnection(WebSocket socket)
  {
    socket.CloseAsync(
      WebSocketCloseStatus.NormalClosure,
      "",
      CancellationToken.None);
  }
}