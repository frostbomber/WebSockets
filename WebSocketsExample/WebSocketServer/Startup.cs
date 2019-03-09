using System;
using System.Net.WebSockets;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebSocketCommon.Services.Commands;
using WebSocketCommon.Services.Data;
using WebSocketCommon.Services.Socket;

namespace WebSocketServer
{
    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/websockets?view=aspnetcore-2.2
    //
    public class Startup
    {
        // Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure Services
            services.AddSingleton<IDataService, DataService>()
                    .AddSingleton<ICommandService, CommandService>()
                    .AddTransient<ISocketService, SocketService>()
                    .AddLogging(options =>
                    {
                        options.AddConsole();
                        options.SetMinimumLevel(LogLevel.Debug);
                    });
        }

        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };
            app.UseWebSockets(webSocketOptions)
                .Use(async (context, next) =>
                {
                    if (context.Request.Path == "/ws")
                    {
                        if (context.WebSockets.IsWebSocketRequest)
                        {
                            System.Net.WebSockets.WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                            var command = await app.ApplicationServices.GetRequiredService<ISocketService>().RecieveCommandAsync(context, webSocket, new CancellationToken());
                            var commandResponse = await app.ApplicationServices.GetRequiredService<ICommandService>().InvokeCommandAsync(command);
                            await app.ApplicationServices.GetRequiredService<ISocketService>().SendCommandAsync(commandResponse, webSocket, new CancellationToken());
                        }
                        else
                        {
                            context.Response.StatusCode = 400;
                        }
                    }
                    else
                    {
                        await next();
                    }
                });
        }
    }
}
