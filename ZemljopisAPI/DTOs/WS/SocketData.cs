namespace ZemljopisAPI.DTOs.WS;

public class SocketData<T>
{
  public Events Evt { get; set; }
  public string RoomCode { get; set; }
  public T Data { get; set; }
}

public enum Events
{
  GET_ROOM_DATA = 0
}