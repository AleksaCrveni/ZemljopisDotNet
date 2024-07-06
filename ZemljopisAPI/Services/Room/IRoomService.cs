using System.Net;
using ZemljopisAPI.DTOs.Room;

namespace ZemljopisAPI.Services;

public interface IRoomService
{
  public Task<(int code, string? resBody)> CreateRoom(CreateRoomDto roomData);
  public Task<(int code, string? resBody)> JoinRoom(JoinRoomDto joinData);
}