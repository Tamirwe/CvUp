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
        public static void Message(string source, string message, EventViewerMessageType messageType)
        {
            using (EventLog eventLog = new())
            {
                if (!EventLog.SourceExists(source))
                {
                    EventLog.CreateEventSource(source, source);
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

                eventLog.Source = source;
                eventLog.WriteEntry(message, EventViewerMessageType);
            }
        }

        public static void ErrorMessage(string message)
        {
            Message("CvUp-ImportGmailCvs", message, EventViewerMessageType.Error);
        }

        public static void InfoMessage(string message)
        {
            Message("CvUp-ImportGmailCvs", message, EventViewerMessageType.Information);
        }

    }
}
