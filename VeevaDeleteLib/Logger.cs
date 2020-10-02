//-----------------------------------------------------------------------
// <copyright file="Logger.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
// <summary>
// This file contains Logger class.
// </summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace VeevaDeleteApi
{
    /// <summary>
    /// Trace listener for log files
    /// </summary>
    public class Logger : IDisposable
    {
        /// <summary>
        /// standard log file extension
        /// </summary>
        public const string LogExtension = ".log";

        /// <summary>
        /// trace listener to write to log file
        /// </summary>
        private TextWriterTraceListener deleteListener;

        // secondary log file listener for internal copy of log
        // private TextWriterTraceListener deleteLog;

        /// <summary>
        /// flag indicates if dispose has already been called
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger" /> class.
        /// </summary>
        /// <param name="filePath">the log file</param>
        public Logger(string filePath)
        {
            // Instantiate listeners to log files.
            this.deleteListener = new TextWriterTraceListener(filePath + LogExtension);
            //// this.deleteLog = new TextWriterTraceListener("internal_" + filePath + LOG_EXT);

            // add to trace listeners list
            Trace.Listeners.Add(this.deleteListener);
        }

        /// <summary>
        /// Add a message to the current trace log
        /// </summary>
        /// <param name="message">the message to add</param>
        /// <param name="time">indicates whether to include a timestamp</param>
        public static void WriteMessage(string message, bool time = false)
        {
            SendTraceMessage(time, (sb) => sb.Append(message));
        }

        /// <summary>
        /// Add an exception message to the trace log
        /// </summary>
        /// <param name="message">the error message</param>
        /// <param name="ex">the exception</param>
        /// <param name="fatal">indicates whether the exception is fatal</param>
        /// <param name="time">indicates whether to include a timestamp</param>
        public static void WriteError(string message, Exception ex, bool fatal, bool time = false)
        {
            SendTraceMessage(
                time,
                (sb) =>
                {
                    if (fatal)
                    {
                        sb.AppendLine("****FATAL ERROR****");
                    }
                    else
                    {
                        sb.AppendLine("****ERROR****");
                    }

                    sb.AppendLine(message);
                    sb.AppendLine(ex.ToString());
                });
        }

        /// <summary>
        /// Close any trace logs
        /// </summary>
        public void Close()
        {
            // Close all log files.
            if (this.deleteListener != null)
            {
                this.deleteListener.Close();
                Trace.Listeners.Remove(this.deleteListener);
                this.deleteListener = null;
            }

            // if (this.deleteLog != null)
            //    this.deleteLog.Close();

            // Release resources.
            // Release whose resources? Everyone's?
            //// Trace.Listeners.Clear();
        }

        /// <summary>
        /// Public implementation of Dispose pattern callable by consumers.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of dispose pattern
        /// </summary>
        /// <param name="disposing">flag to indicate if we are disposing of managed resources, or called via finalize by the GC</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.Close();
            }

            // no umnanaged objects to free
            this.disposed = true;
        }

        /// <summary>
        /// Send message to trace log from a string builder, prepended by time stamp if required.
        /// </summary>
        /// <param name="time">indicate whether or not to prepend timestamp</param>
        /// <param name="filler">delegate to add the message to log into the string builder</param>
        private static void SendTraceMessage(bool time, Action<StringBuilder> filler)
        {
            if (!(filler != null))
            {
                throw new ArgumentNullException(nameof(filler));
            }

            Contract.EndContractBlock();

            StringBuilder sb = new StringBuilder(256);

            // Append time?
            if (time)
            {
                sb.Append(string.Format("[{0}] ", DateTime.Now.ToString()));
            }

            filler(sb);

            // write to log listeners
            Trace.WriteLine(sb.ToString());
            Trace.Flush();
        }
    }
}
