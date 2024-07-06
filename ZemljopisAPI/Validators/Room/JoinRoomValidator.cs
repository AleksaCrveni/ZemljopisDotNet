using FluentValidation;
using ZemljopisAPI.DTOs.Room;

namespace ZemljopisAPI.Validators.Room;

public class JoinRoomValidator : AbstractValidator<JoinRoomDto>
{
  public JoinRoomValidator()
  {
    RuleFor(x => x.Username)
      .NotEmpty()
      .MinimumLength(RoomConsts.MIN_USERNAME_LENGTH)
      .MaximumLength(RoomConsts.MAX_USERNAME_LENGTH);

    RuleFor(x => x.RoomCode)
      .NotEmpty()
      .Length(RoomConsts.ROOM_LENGTH);
  }
}