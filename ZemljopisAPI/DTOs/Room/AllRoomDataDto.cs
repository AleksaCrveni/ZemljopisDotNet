using StackExchange.Redis;

namespace ZemljopisAPI.DTOs.Room;

public class AllRoomDataDto
{
  public RedisValue[] PlayerList { get; set; }
  public HashEntry[] RoomData { get; set; }
}