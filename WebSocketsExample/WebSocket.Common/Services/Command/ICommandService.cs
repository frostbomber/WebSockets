using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebSocketServer.Model;

namespace WebSocketCommon.Services.Commands
{
    public interface ICommandService
    {
        Task<CommandResponse> InvokeCommandAsync(Command cmd);
    }
}
