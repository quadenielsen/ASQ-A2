/*
 * File:		Logger.cs
 * Project:		HiLoGameService
 * By:			Quade Nielsen
 * Date:		November 25, 2021
 * Description:	This file contains the Logger class for the HiLoGame service. .
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace HiLoService
{

	/* 
     * Name         :   Server
     * Description  :   This class is a logger for the HiLoGame service. The logger will append to a log file
	 *					in the same directory as the executable
     */
	public static class Logger
	{
		private static Mutex permissionToLog = new Mutex(); //mutex to prevent calls from multiple threads to append to the same file simultaneously



		/*
         * Function     :   Log
         * Description  :   Appends to a log file in the same directory as the executable, or creates one if none exists.
         * 
         * Parameters   :   string message	:	the message that will be written in the log to the developer
         * Returns      :   None
         */
		public static void Log(string message)
		{
			permissionToLog.WaitOne();
			StreamWriter logWriter = File.AppendText(AppDomain.CurrentDomain.BaseDirectory + "HiLoService.log");
			logWriter.WriteLine(DateTime.Now + " | " + message);
			logWriter.Close();
			permissionToLog.ReleaseMutex();
		}



		/*
         * Function     :   ClearLog
         * Description  :   If a log file exists, it is deleted.
         * 
         * Parameters   :   None
         * Returns      :   None
         */
		public static void ClearLog()
		{
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "HiLoService.log"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "HiLoService.log");
			}
		}

	}
}
