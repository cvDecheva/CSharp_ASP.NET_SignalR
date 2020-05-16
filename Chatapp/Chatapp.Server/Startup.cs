using Microsoft.Owin;
using Owin;
using Microsoft.Extensions.Configuration;
using Chatapp.Server.Entities;
using System;
using Chatapp.Server.EntitiesData;

[assembly: OwinStartup(typeof(Chatapp.Server.Startup))]

namespace Chatapp.Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            string connString = new ConfigurationBuilder()
                           .AddJsonFile("serversettings.json", optional: false, reloadOnChange: true)
                           .Build()
                           .GetConnectionString("defaultConnection");

            try
            {
                Connection.Open(connString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
        }
    }
}
