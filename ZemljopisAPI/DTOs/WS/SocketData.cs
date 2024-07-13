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
  GET_ROOM_DATA = 0
}