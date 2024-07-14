namespace ZemljopisAPI.DTOs.WS;

public class SocketData
{
  public Events Evt { get; set; }
  public string RoomCode { get; set; }
  public string Data { get; set; }
}

public enum Events
{
  ERR = -1,
  JOIN_ROOM = 0,
  GET_ROOM_DATA = 1, // Is this even needed?
}