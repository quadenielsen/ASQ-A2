/*
 * File:		HiLoGameService.cs
 * Project:		HiLoGameService
 * By:			Quade Nielsen
 * Date:		November 25, 2021
 * Description:	This file contains the constructor, OnStart and OnStop methods for the HiLoGame service.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Configuration;

namespace HiLoService
{
    public partial class HiLoGameService : ServiceBase
    {

        private Thread serverThread; //thread which the server will run on
        private Server server;  //server object which listens for players

        /*
         * Function     :   HiLoGameService
         * Description  :   Initializes the HiLoGameService.
         * 
         * Parameters   :   None
         * Returns      :   None
         */
        public HiLoGameService()
        {
            InitializeComponent();
        }



        /*
         * Function     :   OnStart
         * Description  :   Creates and starts a server for the Hi Lo game based on the settings in the config file.
         * 
         * Parameters   :   None
         * Returns      :   None
         */
        protected override void OnStart(string[] args)
        {

            try
            {
                server = new Server(ConfigurationManager.AppSettings["IPAddress"], int.Parse(ConfigurationManager.AppSettings["PortNumber"]));
                serverThread = new Thread(new ThreadStart(server.StartServer));
                serverThread.Start();
                Logger.Log("Server was started.");
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
            }
        }



        /*
         * Function     :   OnStop
         * Description  :   Shuts down the Hi Lo game server.
         * 
         * Parameters   :   None
         * Returns      :   None
         */
        protected override void OnStop()
        {
            try
            {
                server.ShutDown();
                serverThread.Join();
                Logger.Log("Server was stopped.");
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
            }
            
        }
    }
}
