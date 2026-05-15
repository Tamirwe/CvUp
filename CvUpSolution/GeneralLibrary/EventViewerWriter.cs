using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralLibrary
{
    public enum EventViewerMessageType
    {
        Error,
        Information,
        Warning,
        Success
    }

    public static class EventViewerWriter
    {
        public static void ErrorMessage(string message)
        {
            Message("CvUp-ImportGmailCvs", message, EventViewerMessageType.Error);
        }

        public static void InfoMessage(string message)
        {
            Message("CvUp-ImportGmailCvs", message, EventViewerMessageType.Information);
        }


        private static void Message(string logName, string message, EventViewerMessageType messageType)
        {
            string _source = "CvUp";

            if (!OperatingSystem.IsWindows()) return;

            using (EventLog eventLog = new())
            {
                if (!EventLog.SourceExists(_source))
                {
                    EventLog.CreateEventSource(_source, logName);
                }

                EventLogEntryType EventViewerMessageType = EventLogEntryType.Error;

                switch (messageType)
                {
                    case GeneralLibrary.EventViewerMessageType.Information:
                        EventViewerMessageType = EventLogEntryType.Information;
                        break;
                    case GeneralLibrary.EventViewerMessageType.Warning:
                        EventViewerMessageType = EventLogEntryType.Warning;
                        break;
                    case GeneralLibrary.EventViewerMessageType.Success:
                        EventViewerMessageType = EventLogEntryType.SuccessAudit;
                        break;
                    default:
                        break;
                }

                eventLog.Source = _source;
                eventLog.WriteEntry(message, EventViewerMessageType);
            }
        }

      
    }
}
