using System;
using System.IO;

namespace MigrationUtils
{
	// This allows for more flexible logging
	public delegate void OutputDelegate(string message);
	
	/// <summary>
	/// Summary description for Logger.
	/// </summary>
	public class Logger
	{
		private static string _logFileName = null;

		/// <summary>
		/// Returns the log file name
		/// </summary>
		public static string LogFileName
		{
			get
			{
				if (Logger._logFileName == null)
					throw new Exception("Log File Name not set. Logger not intialized.");
				return Logger._logFileName;
			}
		}

		/// <summary>
		///  Needs to now the file name and location
		///  The fileName should be the full filename including the path
		/// </summary>
		/// <param name="fileName"></param>
		public static void Initialize(string fileName)
		{
			//if (Logger._logFileName != null)
			//	throw new Exception("Logger already initialized");
			Logger._logFileName = fileName;
		}

		/// <summary>
		/// Write out debug messages
		/// </summary>
		/// <param name="message"></param>
		public static void WriteDebugMessage(string message)
		{
			Output("DEBUG: " + DateTime.Now + " " + message);
		}

		/// <summary>
		/// Write out messages
		/// </summary>
		/// <param name="message"></param>
		public static void WriteMessage(string message)
		{
			Output(message);
		}

		/// <summary>
		/// This does the actual output to the file
		/// </summary>
		/// <param name="message"></param>
		public static void Output(string message)
		{
			StreamWriter sw = new StreamWriter(Logger.LogFileName, true);
			sw.WriteLine(message);
			sw.Close();
		}

		/// <summary>
		/// Writes to the specified file
		/// </summary>
		/// <param name="message"></param>
		public static void WriteToFile(string message, string fileName)
		{
			StreamWriter sw = new StreamWriter(fileName, true);
			sw.WriteLine("DETAIL: " + DateTime.Now + " " + message);
			sw.Close();
		}

	
	}
}
