using StackExchange.Redis;

namespace ZemljopisAPI.DTOs.Room;

public class AllRoomData
{
  public RedisValue[] PlayerList { get; set; }
  public HashEntry[] RoomData { get; set; }
}