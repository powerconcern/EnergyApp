using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace EnergyApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://*:5000", "https://*:5001")
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                        {
                            logging.ClearProviders();
                            logging.AddConsole(c =>
                            {
                                c.IncludeScopes=true;
                                //From 3.0 c.TimestampFormat = "[HH:mm:ss] ";
                            });
                        });
    }
}
