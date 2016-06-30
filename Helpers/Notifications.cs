using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.DbProvider;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Notifications;
using Trinity.Technicals;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Helpers
{
    class Notifications
    {
        private static bool _loggedAnythingThisStash;
        private static bool _loggedJunkThisStash;

        public static void SendEmailNotification()
        {
            if (TrinityPlugin.Settings.Notification.MailEnabled && NotificationManager.EmailMessage.Length > 0)
                NotificationManager.SendEmail(
                    TrinityPlugin.Settings.Notification.EmailAddress,
                    TrinityPlugin.Settings.Notification.EmailAddress,
                    "New DB stash loot - " + FileManager.BattleTagName,
                    NotificationManager.EmailMessage.ToString(),
                    NotificationManager.SmtpServer,
                    TrinityPlugin.Settings.Notification.EmailPassword);
            NotificationManager.EmailMessage.Clear();
        }

        public static void SendMobileNotifications()
        {
            while (NotificationManager.pushQueue.Count > 0)
            {
                NotificationManager.SendNotification(NotificationManager.pushQueue.Dequeue());
            }
        }


        /// <summary>
        ///     Log the nice items we found and stashed
        /// </summary>
        public static void LogStashedItems(TrinityItem item, TrinityItemBaseType itemBaseType, TrinityItemType itemType, double itemValue)
        {
            FileStream logStream = null;
            try
            {
                string filePath = Path.Combine(FileManager.LoggingPath, "StashLog - " + TrinityPlugin.Player.ActorClass + ".log");
                logStream = File.Open(filePath, FileMode.Append, FileAccess.Write, FileShare.Read);

                //TODO : Change File Log writing
                using (var logWriter = new StreamWriter(logStream))
                {
                    if (!_loggedAnythingThisStash)
                    {
                        _loggedAnythingThisStash = true;
                        logWriter.WriteLine(DateTime.Now + ":");
                        logWriter.WriteLine("====================");
                    }
                    string sLegendaryString = "";
                    bool shouldSendNotifications = false;

                    if (item.ItemQualityLevel >= ItemQuality.Legendary)
                    {

                        shouldSendNotifications = true;

                            NotificationManager.AddNotificationToQueue(item.Name + " [" + itemType +
                                                                       "] (Score=" + itemValue + ". " + item.Attributes.Summary() + ")",
                                ZetaDia.Service.Hero.Name + " new legendary!", ProwlNotificationPriority.Emergency);
                        sLegendaryString = " {legendary item}";

                        // Change made by bombastic
                        Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "+=+=+=+=+=+=+=+=+ LEGENDARY FOUND +=+=+=+=+=+=+=+=+");
                        Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "+  Name:       {0} ({1})", item.Name, itemType);
                        Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "+  Score:       {0:0}", itemValue);
                        Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "+  Attributes: {0}", item.Attributes.Summary());
                        Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+");
                    }
                    else
                    {
                        // Check for non-legendary notifications
                        shouldSendNotifications = ItemValuation.CheckScoreForNotification(itemBaseType, itemValue);
                        if (shouldSendNotifications)
                            NotificationManager.AddNotificationToQueue(item.Name + " [" + itemType + "] (Score=" + itemValue + ". " + item.Attributes.Summary() + ")",
                                ZetaDia.Service.Hero.BattleTagName + " new item!", ProwlNotificationPriority.Normal);
                    }
                    if (shouldSendNotifications)
                    {
                        NotificationManager.EmailMessage.AppendLine(itemBaseType + " - " + itemType + " '" + item.Name + "'. Score = " + Math.Round(itemValue) + sLegendaryString)
                            .AppendLine("  " + item.Attributes.Summary())
                            .AppendLine();
                    }
                    logWriter.WriteLine(itemBaseType + " - " + itemType + " '" + item.Name + "'. Score = " + Math.Round(itemValue) + sLegendaryString);
                    logWriter.WriteLine("  " + item.Attributes.Summary());
                    logWriter.WriteLine("");
                }
                logStream.Close();
            }
            catch (IOException)
            {
                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Error: File access error for stash log file.");
                if (logStream != null)
                    logStream.Close();
            }
        }

        /// <summary>
        ///     Log the rubbish junk items we salvaged or sold
        /// </summary>
        public static void LogJunkItems(TrinityItem item, TrinityItemBaseType itemBaseType, TrinityItemType itemType, double itemValue)
        {
            FileStream logStream = null;
            try
            {
                string filePath = Path.Combine(FileManager.LoggingPath, "JunkLog - " + TrinityPlugin.Player.ActorClass + ".log");
                logStream = File.Open(filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                using (var logWriter = new StreamWriter(logStream))
                {
                    if (!_loggedJunkThisStash)
                    {
                        _loggedJunkThisStash = true;
                        logWriter.WriteLine(DateTime.Now + ":");
                        logWriter.WriteLine("====================");
                    }
                    string isLegendaryItem = "";

                    if (item.ItemQualityLevel >= ItemQuality.Legendary)
                        isLegendaryItem = " {legendary item}";

                    logWriter.WriteLine(itemBaseType + " - " + itemType + " '" + item.Name + "'. Score = " + itemValue.ToString("0") + isLegendaryItem);

                    logWriter.WriteLine(item.Attributes.Summary());

                    logWriter.WriteLine("");
                }
                logStream.Close();
            }
            catch (IOException)
            {
                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Error: File access error for junk log file.");
                if (logStream != null)
                    logStream.Close();
            }
        }
    }
}
