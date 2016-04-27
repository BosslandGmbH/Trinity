using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Trinity.Technicals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Cache
{
    public class ItemStashSellAppender : IDisposable
    {
        internal static Regex ItemNameRegex = new Regex(@"-\d+", RegexOptions.Compiled);

        internal static string GetCleanName(string itemInternalName)
        {
            return ItemNameRegex.Replace(itemInternalName, "");
        }

        private HashSet<int> _loggedItems = new HashSet<int>();

        bool _headerChecked;
        public ItemStashSellAppender()
        {
            _logItemQueue = new ConcurrentQueue<string>();

            _itemLogPath = Path.Combine(FileManager.TrinityLogsPath, "StashSellSalvage.csv");

            CheckHeader();

            _queueThread = new Thread(QueueWorker)
            {
                Name = "StashSellSalvageWorker",
                IsBackground = true,
                Priority = ThreadPriority.Lowest
            };
            _queueThread.Start();

        }

        private void CheckHeader()
        {
            if (_headerChecked)
                return;

            bool writeHeader = !File.Exists(_itemLogPath);

            if (writeHeader)
            {
                _logItemQueue.Enqueue("ActorSnoId,Name,InternalName,DBBaseType,DBItemType,TBaseType,TItemType,Quality,Level,Action,Stats\n");
            }
            _headerChecked = true;
        }
        public void Dispose()
        {
            try
            {
                if (_queueThread != null)
                    _queueThread.Abort();
            }
            catch { }
            _queueThread = null;
        }

        private readonly Mutex _mutex = new Mutex(false, "ItemStashedMutex");

        private static ItemStashSellAppender _instance;
        public static ItemStashSellAppender Instance { get { return _instance ?? (_instance = new ItemStashSellAppender()); } }

        private StreamWriter _logWriter;
        private FileStream _fileStream;

        private readonly ConcurrentQueue<string> _logItemQueue;

        private Thread _queueThread;
        private readonly string _itemLogPath;

        internal void AppendItem(CachedACDItem item, string action)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(FormatCSVField(item.AcdItem.ActorSnoId));
            sb.Append(FormatCSVField(item.RealName));

            if (item.Quality >= ItemQuality.Legendary || item.BaseType == ItemBaseType.Misc)
                sb.Append(FormatCSVField(GetCleanName(item.InternalName)));
            else
                sb.Append(FormatCSVField(item.Quality + " " + item.BaseType));

            sb.Append(FormatCSVField(item.BaseType.ToString()));
            sb.Append(FormatCSVField(item.ItemType.ToString()));
            sb.Append(FormatCSVField(item.TrinityItemBaseType.ToString()));
            sb.Append(FormatCSVField(item.TrinityItemType.ToString()));
            sb.Append(FormatCSVField(item.Quality.ToString()));
            sb.Append(FormatCSVField(item.Level));
            sb.Append(FormatCSVField(action));
            var stats = item.AcdItem.Stats.ToString();
            stats = stats.Replace("\r\n\t", " ").Replace(" - ", ": ").Trim(new[] { '\r', '\n', '\t', ' ' });
            sb.Append(FormatCSVField(stats));
            sb.Append("\n");

            _logItemQueue.Enqueue(sb.ToString());

        }

        private void QueueWorker()
        {
            const int bufferSize = 65536;
            const int maxAttempts = 50;

            while (true)
            {
                try
                {
                    CheckHeader();

                    if (_fileStream == null)
                        _fileStream = File.Open(_itemLogPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);

                    if (_logWriter == null)
                        _logWriter = new StreamWriter(_fileStream, Encoding.UTF8, bufferSize);

                    string queueItem;
                    while (_logItemQueue.TryDequeue(out queueItem))
                    {
                        int hashCode = queueItem.GetHashCode();
                        if (_loggedItems.Contains(hashCode))
                            continue;
                        if (!_loggedItems.Contains(hashCode))
                            _loggedItems.Add(hashCode);

                        bool success = false;
                        int attempts = 0;
                        while (!string.IsNullOrWhiteSpace(queueItem) && !success && attempts <= maxAttempts)
                        {
                            try
                            {

                                _mutex.WaitOne();

                                attempts++;
                                _logWriter.Write(queueItem);
                                _logWriter.Flush();
                                _fileStream.Flush();
                                success = true;
                                queueItem = "";

                            }
                            catch (Exception ex)
                            {
                                Logger.LogError("Error in StashSellSalvage QueueWorker: " + ex.Message);
                            }
                            finally
                            {
                                _mutex.ReleaseMutex();
                            }
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    // ssh...
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error in StashSellSalvage QueueWorker: " + ex.Message);
                }

                Thread.Sleep(10);
            }
        }

        private static string FormatCSVField(DateTime time)
        {
            return String.Format("\"{0:yyyy-MM-ddTHH:mm:ss.ffff}\",", time.ToLocalTime());
        }

        private static string FormatCSVField(string text)
        {
            return String.Format("\"{0}\",", text);
        }

        private static string FormatCSVField(int number)
        {
            return String.Format("\"{0}\",", number);
        }

        private static string FormatCSVField(double number)
        {
            return String.Format("\"{0:0}\",", number);
        }

        private static string FormatCSVField(bool value)
        {
            return String.Format("\"{0}\",", value);
        }
    }
}
