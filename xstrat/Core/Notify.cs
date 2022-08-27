using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xstrat.Core
{
    public static class Notify
    {
        private static bool endLogging = false;
        static NotificationManager notificationManager = new NotificationManager();
        public static void sendInfo(string message)
        {
            if (endLogging) return;
            notificationManager.Show(new NotificationContent
            {
                Title = "Info",
                Message = message,
                Type = NotificationType.Information
            });
        }
        public static void sendWarn(string message)
        {
            if (endLogging) return;
            notificationManager.Show(new NotificationContent
            {
                Title = "Warning!",
                Message = message,
                Type = NotificationType.Warning
            });
        }
        public static void sendSuccess(string message)
        {
            if (endLogging) return;
            notificationManager.Show(new NotificationContent
            {
                Title = "Success!",
                Message = message,
                Type = NotificationType.Success
            });
        }
        public static void sendError(string message)
        {
            if (endLogging) return;
            notificationManager.Show(new NotificationContent
            {
                Title = "Error!",
                Message = message,
                Type = NotificationType.Error
            });
        }
        public static void EndLogging()
        {
            endLogging = true;
        }
        public static void ResumeLogging()
        {
            endLogging = false;
        }
    }
}
