using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using ZemljopisAPI.DTOs.Room;
using ZemljopisAPI.Helpers;
using ZemljopisAPI.Services;
using ZemljopisAPI.Validators.Room;

namespace ZemljopisAPI.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/Room")]
public class RoomController(IRoomService _service) : ControllerBase
{

  [HttpPost("Create")]
  public async Task<ObjectResult> CreateRoom(CreateRoomDto roomData)
  {
    CreateRoomValidator validator = new();
    if (!validator.Validate(roomData).IsValid)
      return StatusCode(400, null);

    var (code, body) = await _service.CreateRoom(roomData);
    return StatusCode(code,body);
  }

  [HttpPost("Join")]
  public async Task<ObjectResult> JoinRoom(JoinRoomDto joinData)
  {
    JoinRoomValidator validator = new();
    if (!validator.Validate(joinData).IsValid)
      return StatusCode(400, null);

    var (code, body) = await _service.JoinRoom(joinData);
    return StatusCode(code, body);
  }

}