using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace VeevaDelete
{
    class Logger
    {
        public const string LOG_EXT = ".log";
        private TextWriterTraceListener deleteListener;
        private TextWriterTraceListener deleteLog;

        public Logger(string filePath)
        {

                //Instantiate listeners to log files.
                deleteListener = new TextWriterTraceListener(filePath + LOG_EXT);
                deleteLog = new TextWriterTraceListener("internal_" + filePath + LOG_EXT);

                //add to trace listeners list
                Trace.Listeners.Add(deleteListener);
        }

        public static void WriteMessage(string message, bool time = false)
        {
            StringBuilder sb = new StringBuilder(256);

            //Append time?
            if (time)
            {
                sb.Append(string.Format("[{0}] ", DateTime.Now.ToString()));
                sb.Append(message);
            }
            else
            {
                sb.Append(message);
            }

            //WriteToLog(sb.ToString());
            Trace.WriteLine(sb.ToString());
            Trace.Flush();
            System.Windows.Forms.Application.DoEvents();
        }

        public static void WriteError(string message, Exception ex, bool fatal, bool time = false)
        {
            StringBuilder sb = new StringBuilder(256);

            //Append time?
            if (time)
            {
                sb.Append(string.Format("[{0}] ", DateTime.Now.ToString()));
            }

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

            //write to log listeners
            Trace.WriteLine(sb.ToString());
            Trace.Flush();
            System.Windows.Forms.Application.DoEvents();
        }

        public void Close()
        {
            //Close all log files.
            if (deleteListener != null)
                deleteListener.Close();

            if (deleteLog != null)
                deleteLog.Close();

            //Release resources.
            Trace.Listeners.Clear();
        }
    }
   }

