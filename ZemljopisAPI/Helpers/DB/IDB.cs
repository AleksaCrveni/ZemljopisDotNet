using StackExchange.Redis;
using ZemljopisAPI.DTOs.Room;

namespace ZemljopisAPI.Helpers;

public interface IDB
{
  public Task<bool> SetHashAsync(RedisKey key, HashEntry[] entries);
  public Task<(bool, string?)> GetStringAsyncStack(string key);
  public Task<(bool isSuccessful, bool exists)> KeyExistsAsync(RedisKey key);
  public Task<bool> CreateRoomTrx(string roomCode, string username, HashEntry[] entries);
  public Task<bool> JoinRoomTrx(string roomCode, string username);
  public Task<AllRoomDataDto?> RetrieveAllRoomData(string roomCode);

}