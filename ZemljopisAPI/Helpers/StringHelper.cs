using System.Security.Cryptography;
using ZemljopisAPI.Validators;

namespace ZemljopisAPI.Helpers;

public static class StringHelper
{
  private static readonly string randomInput = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890";
  public static string GenerateRoomCode()
  {
    return RandomNumberGenerator.GetString(randomInput, RoomConsts.ROOM_LENGTH);
  }
}