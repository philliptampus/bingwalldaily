using BPRO.Apps.BingWallDaily.Core;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;

namespace BingWallDailyService
{
    public partial class BingWallDailyService : ServiceBase
    {

        public BingWallDailyService()
        {
            InitializeComponent();

            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("BingWallDaily"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "BingWallDaily", "BingWallDaily");
            }
            eventLog1.Source = "BingWallDaily";
            eventLog1.Log = "BingWallDaily";

            // Set up a timer that triggers every minute.
            Timer timer = new Timer();
            timer.Interval = 60000; // 60 seconds
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();

        }

        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            var moment = DateTime.Now;
            if (moment.Hour == 17 && moment.Minute == 0)
            {
                try
                {
                    BingImageProcessor.Init();
                    eventLog1.WriteEntry("Successfully initiated BingWallDaily process on timer.", EventLogEntryType.Information);
                }
                catch (Exception e)
                {
                    eventLog1.WriteEntry("Failed initiating BingWallDaily process. Error: " + e.Message, EventLogEntryType.Error);
                }
            }
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            switch (changeDescription.Reason)
            {
                case SessionChangeReason.SessionLogon:
                case SessionChangeReason.SessionUnlock:
                    BingImageProcessor.Init();
                    break;
                default:
                    break;
            }
        }

        protected override void OnStart(string[] args)
        {
            System.Diagnostics.Debugger.Launch();
            eventLog1.WriteEntry("Started BingWallDaily service.", EventLogEntryType.Information);
        }

        protected override void OnStop()
        {
        }

    }
}
