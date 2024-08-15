using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.ListenAnyIP(5001, listenOptions =>
                    {
                        listenOptions.UseHttps("path/to/certificate.pfx", "yourpassword");
                    });
                });
                webBuilder.UseStartup<Startup>();
            });
}

public class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        app.Run(async context =>
        {
            if (context.Request.Path == "/start-node")
            {
                // Execute Node.js script here
                await context.Response.WriteAsync("Node script started.");
            }
            else
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Not found.");
            }
        });
    }
}
