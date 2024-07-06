using FluentValidation;
using ZemljopisAPI.DTOs.Room;

namespace ZemljopisAPI.Validators.Room;

public class CreateRoomValidator : AbstractValidator<CreateRoomDto>
{
  public CreateRoomValidator()
  {
    RuleFor(x => x.Username)
      .NotEmpty()
      .MinimumLength(RoomConsts.MIN_USERNAME_LENGTH)
      .MaximumLength(RoomConsts.MAX_USERNAME_LENGTH);

    RuleFor(x => x.PlayerCount)
      .NotEmpty()
      .LessThanOrEqualTo(RoomConsts.MAX_PLAYERCOUNT)
      .GreaterThanOrEqualTo(RoomConsts.MIN_PLAYERCOUNT);
  }
}