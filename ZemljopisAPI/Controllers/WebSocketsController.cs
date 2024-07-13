using System.Net.WebSockets;
using System.Text;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace ZemljopisAPI.DTOs.WS;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/ws")]
public class WebSocketsController : ControllerBase
{
  public WebSocketsController()
  {

  }

  [Route("")]
  public async Task Get()
  {
    if (HttpContext.WebSockets.IsWebSocketRequest)
    {
      /*
       * ReceiveAsync basically awaits result
       */
      using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
      var buffer = new byte[1024 * 4];
      WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
      var socketData = Encoding.Default.GetString(buffer, 0, receiveResult.Count);

      // events are encoded in first 3
      string evt = socketData.Substring(0, 3);
      while (!receiveResult.CloseStatus.HasValue)
      {
        /*
        await webSocket.SendAsync(
          new ArraySegment<byte>(buffer, 0, receiveResult.Count),
          receiveResult.MessageType,
          receiveResult.EndOfMessage,
          CancellationToken.None);
        */
        receiveResult = await webSocket.ReceiveAsync(
          new ArraySegment<byte>(buffer), CancellationToken.None);
        socketData = Encoding.Default.GetString(buffer);
      }

      if (receiveResult != null)
      {
        await webSocket.CloseAsync(
          receiveResult.CloseStatus.Value,
          receiveResult.CloseStatusDescription,
          CancellationToken.None);
      }
    }
    else
    {
      HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
  }

}