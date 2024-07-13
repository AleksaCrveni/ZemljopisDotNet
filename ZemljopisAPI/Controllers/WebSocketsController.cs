using System.Net.WebSockets;
using System.Text;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ZemljopisAPI.Services.Sockets;

namespace ZemljopisAPI.DTOs.WS;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/ws")]
public class WebSocketsController(ILogger<WebSocketsController> _logger, ISocketService _socketService) : ControllerBase
{

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

      string socketData = Encoding.Default.GetString(buffer, 0, receiveResult.Count);
      SocketResult result = new SocketResult();
      string responseString = "";

      while (!receiveResult.CloseStatus.HasValue)
      {
        result = await _socketService.ProcessSocketData(socketData);
        responseString = JsonConvert.SerializeObject(result!);
        // @TODO refactor to reuse buffer, for now keep going till I built something
        var outBuffer = Encoding.Default.GetBytes(responseString);
        await webSocket.SendAsync(new ArraySegment<byte>(outBuffer, 0, outBuffer.Length),
          receiveResult.MessageType,
          receiveResult.EndOfMessage,
          CancellationToken.None);

        receiveResult = await webSocket.ReceiveAsync(
          new ArraySegment<byte>(buffer), CancellationToken.None);
        socketData = Encoding.Default.GetString(buffer, 0, receiveResult.Count);
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