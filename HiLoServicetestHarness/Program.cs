

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using HiLoService;
using System.Net;
using System.Net.Sockets;

namespace HiLoServicetestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Logger.ClearLog();
            Server server = new Server("127.0.0.1", 11000);
            Thread serverThread = new Thread(new ThreadStart(server.StartServer));
            serverThread.Start();
            Logger.Log("Server was started.");

            Thread.Sleep(2000);

            Logger.Log("Attempting to stop server.");
            server.ShutDown();
            Logger.Log("Closed the listening socket.");

            serverThread.Join();
            Logger.Log("Server was stopped.");
            
        }
    }
}
