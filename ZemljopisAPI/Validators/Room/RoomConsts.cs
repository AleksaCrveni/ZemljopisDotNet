namespace ZemljopisAPI.Validators;

public static class RoomConsts
{
  public static int MIN_USERNAME_LENGTH { get; } = 3;
  public static int MAX_USERNAME_LENGTH { get; } = 24;
  public static int MAX_PLAYERCOUNT { get; } = 24;
  public static int MIN_PLAYERCOUNT { get; } = 1;
  public static int ROOM_LENGTH { get; } = 8;
}