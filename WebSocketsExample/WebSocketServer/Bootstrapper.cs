using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using WebSocketCommon.Services.Data;

namespace WebSocketServer
{
    public class Bootstrapper
    {
        static void Main(string[] args)
        {
            // Start the Kestrel Server
            var host = new WebHostBuilder()
                       .UseKestrel()
                       .UseStartup<Startup>()
                       .Build();
            host.Start();

            // Initialize Application
            Application myApp = new Application(host.Services);
            myApp.Run();

            // Press any key to continue
            Console.WriteLine("Press any key to continue...");
            Console.Read();
        }
    }
    
    public sealed class Application
    {
        ILogger _Logger { get; set; }
        IServiceProvider _Services { get; set; }

        public Application(IServiceProvider serviceProvider)
        {
            _Services = serviceProvider;
            _Logger = _Services.GetService<ILoggerFactory>().CreateLogger<Application>();
        }

        public void Run()
        {
            // Configure Data Service
            _Services.GetRequiredService<IDataService>().SetConnectionString(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Northwind;Integrated Security=True");
        }
    }
}
