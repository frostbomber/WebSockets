﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebSocketCommon.Services.Data;
using WebSocketCommon.Services.Timer;
using WebSocketServer.Model;

namespace WebSocketCommon.Services.Commands
{
    public class CommandService : ICommandService
    {
        ILogger _Logger { get; set; }
        IDataService _DataService { get; set; }
        public ITimerService _TimerService { get; set; }

        public CommandService(IServiceProvider serviceProvider)
        {
            _Logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<CommandService>();
            _DataService = serviceProvider.GetRequiredService<IDataService>();
            _TimerService = serviceProvider.GetRequiredService<ITimerService>();
        }

        public async Task<CommandResponse> InvokeCommandAsync(Command cmd)
        {
            CommandResponse cmdResponse = new CommandResponse();
            cmdResponse.CommandId = cmd.CommandId;
            // Quick and really dirty implementation
            if (string.Compare(cmd.CommandType, "GetNames", true) == 0)
            {
                var dbCommand = _DataService.CreateCommand("SELECT * FROM Names");
                List<object[]> values = await _DataService.ExecuteReaderAsync(dbCommand);
                cmdResponse.Data = values.Select(a => string.Join(" ", a.Skip(1))).ToArray();
            }
            else if (string.Compare(cmd.CommandType, "TimerTest", true) == 0)
            {
                _TimerService.TestTimer();
            }
            return cmdResponse;
        }
    }
}
