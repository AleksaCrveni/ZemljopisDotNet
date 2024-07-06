using System.Net;
using StackExchange.Redis;
using ZemljopisAPI.DI;
using ZemljopisAPI.DTOs.Room;
using ZemljopisAPI.Helpers;

namespace ZemljopisAPI.Services.Room;

public class RoomService : IRoomService, IScoped
{
  private readonly ILogger<RoomService> _logger;
  private readonly IDB _dbRepository;
  public RoomService(ILogger<RoomService> logger, IDB dbRepository)
  {
    _logger = logger;
    _dbRepository = dbRepository;
  }

  public async Task<(int code, string? resBody)> CreateRoom(CreateRoomDto data)
  {
    var (code, roomCode) = (StatusCodes.Status200OK, "");
    roomCode = StringHelper.GenerateRoomCode();
    var (okKeyExist, exists) = await _dbRepository.KeyExistsAsync(roomCode);
    if (!okKeyExist)
      return (StatusCodes.Status500InternalServerError, null);

    int i = 0;
    // TODO @Wick: See if this can be in transaction as well
    while (i < Config.ROOM_CREATION_RETRY && exists)
    {
      roomCode = StringHelper.GenerateRoomCode();
      (okKeyExist, exists) = await _dbRepository.KeyExistsAsync(roomCode);
      if (!exists)
        break;
      i++;
    }

    if (exists)
    {
      // Send notification to me for this!
      _logger.LogError("No names available!");
      return (StatusCodes.Status500InternalServerError, null);
    }
    // Put this thing in transcation -> https://stackexchange.github.io/StackExchange.Redis/Transactions.html
    // Prep room data
    HashEntry[] roomData = new HashEntry[4];
    roomData[0] = new HashEntry("playerCount", data.PlayerCount);
    roomData[1] = new HashEntry("roomCode", roomCode);
    roomData[2] = new HashEntry("playersReady", 0);
    roomData[3] = new HashEntry("createdAt", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
    bool ok = await _dbRepository.CreateRoomTrx(roomCode, data.Username, roomData);
    if (!ok)
      return (StatusCodes.Status500InternalServerError, null);

    return (code, roomCode);
  }

  public async Task<(int code, string? resBody)> JoinRoom(JoinRoomDto data)
  {
    var (code, resBody) = (StatusCodes.Status200OK, "");

    bool ok = await _dbRepository.JoinRoomTrx(data.RoomCode, data.Username);
    if (!ok)
      return (StatusCodes.Status500InternalServerError, null);
    return (code, resBody);
  }
}

