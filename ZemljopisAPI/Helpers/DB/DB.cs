using System.Reflection.Metadata.Ecma335;
using StackExchange.Redis;
using ZemljopisAPI.DI;
using ZemljopisAPI.DTOs.Room;
using ZemljopisAPI.Validators;

namespace ZemljopisAPI.Helpers;

public class DB  : IDB, ITransient
{
  private readonly IDatabase db;
  private readonly ILogger<DB> _logger;
  public DB(ILogger<DB> logger)
  {
    _logger = logger;
    db = DbHelper.Init.GetDatabase();
  }

  // Call only when you know result size
  public async Task<(bool, string?)> GetStringAsyncStack(string key)
  {
    try
    {
      return (true, (await db.StringGetAsync(key)).ToString());
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error in GetStringAsyncStack. ErrMsg: {Message}", ex.Message);
      return (false, null);
    }
  }

  public async Task<bool> SetHashAsync(RedisKey key, HashEntry[] entries)
  {
    try
    {
      await db.HashSetAsync(key, entries);
      return true;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error in GetStringAsyncStack. ErrMsg: {Message}", ex.Message);
      return false;
    }
  }

  public async Task<(bool isSuccessful, bool exists)> KeyExistsAsync(RedisKey key)
  {
    try
    {
      return (true, await db.KeyExistsAsync(key));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error in KeyExist. ErrMsg: {Message}", ex.Message);
      return (false, false);
    }
  }

  /*/
   * Put all redis actions here, that needed to be done on room creation
   * CreateRoom
   * CreateList of Players
   */
  public async Task<bool> CreateRoomTrx(string roomCode, string username, HashEntry[] entries)
  {
    try
    {
      var trx = db.CreateTransaction();
      trx.HashSetAsync($"room:{roomCode}", entries);
      trx.ListRightPushAsync($"players:{roomCode}", username);
      return await trx.ExecuteAsync();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error in CreateRoomTrx. ErrMsg: {Message}", ex.Message);
      return false;
    }
  }

  public async Task<bool> JoinRoomTrx(string roomCode, string username)
  {
    try
    {
      RedisKey playerKey = $"players:{roomCode}";
      RedisKey roomKey = $"room:{roomCode}";
      var trx = db.CreateTransaction();
      trx.AddCondition(Condition.KeyExists(playerKey));
      trx.AddCondition(Condition.ListLengthLessThan(playerKey, RoomConsts.MAX_PLAYERCOUNT));
      trx.AddCondition(Condition.KeyExists(roomKey));
      trx.ListRightPushAsync(playerKey, username);
      trx.HashIncrementAsync(roomKey, 1);
      return await trx.ExecuteAsync();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error in JoinRoomTrx. ErrMsg: {Message}", ex.Message);
      return false;
    }
  }

  public async Task<AllRoomDataDto?> RetrieveAllRoomData(string roomCode)
  {
    try
    {
      var playersDataTask = db.ListRangeAsync($"players:{roomCode}");
      var roomDataTask = db.HashGetAllAsync($"room:{roomCode}");
      await Task.WhenAll(playersDataTask, roomDataTask);

      RedisValue[] playersData = await playersDataTask;
      HashEntry[] roomData = await roomDataTask;

      AllRoomDataDto data = new AllRoomDataDto()
      {
        PlayerList = playersData,
        RoomData = roomData,
      };
      return data;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error in JoinRoomTrx. ErrMsg: {Message}", ex.Message);
      return null;
    }
  }
}