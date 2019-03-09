using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebSocketServer.Model;

namespace WebSocketCommon.Services.Socket
{
    public interface ISocketService
    {
        Task<string> RecieveMessageAsync(HttpContext context, System.Net.WebSockets.WebSocket socket, CancellationToken token);
        Task<bool> SendMessageAsync(string message, System.Net.WebSockets.WebSocket socket, CancellationToken token);
        Task<Command> RecieveCommandAsync(HttpContext context, System.Net.WebSockets.WebSocket socket, CancellationToken token);
        Task<bool> SendCommandAsync(Command message, System.Net.WebSockets.WebSocket socket, CancellationToken token);
    }
}
