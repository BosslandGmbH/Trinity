using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using Trinity.Technicals;

namespace Trinity.Cache
{
    public class ItemDroppedAppender : IDisposable
    {
        bool _headerChecked;
        public ItemDroppedAppender()
        {
            _logItemQueue = new ConcurrentQueue<string>();

            _droppedItemLogPath = Path.Combine(FileManager.TrinityLogsPath, "ItemsDropped.csv");

            CheckHeader();

            _queueThread = new Thread(QueueWorker)
            {
                Name = "ItemDroppedWorker",
                IsBackground = true,
                Priority = ThreadPriority.Lowest
            };
            _queueThread.Start();

        }

        private void CheckHeader()
        {
            if (_headerChecked)
                return;

            bool writeHeader = !File.Exists(_droppedItemLogPath);

            if (writeHeader)
            {
                _logItemQueue.Enqueue("ActorSnoId,GameBalanceID,Name,InternalName,DBBaseType,DBItemType,TBaseType,TItemType,Quality,Level,Pickup\n");
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

        private readonly Mutex _mutex = new Mutex(false, "ItemDroppedMutex");

        private static ItemDroppedAppender _instance;
        public static ItemDroppedAppender Instance { get { return _instance ?? (_instance = new ItemDroppedAppender()); } }

        private StreamWriter _logWriter;
        private FileStream _fileStream;

        private readonly ConcurrentQueue<string> _logItemQueue;

        private Thread _queueThread;
        private readonly string _droppedItemLogPath;

        internal void AppendDroppedItem(PickupItem item)
        {
            bool pickupItem;
            CacheData.PickupItem.TryGetValue(item.RActorGUID, out pickupItem);

            StringBuilder sb = new StringBuilder();

            sb.Append(FormatCSVField(item.ActorSNO));
            sb.Append(FormatCSVField(item.BalanceID));
            sb.Append(FormatCSVField(item.Name));
            sb.Append(FormatCSVField(item.InternalName));
            sb.Append(FormatCSVField(item.DBBaseType.ToString()));
            sb.Append(FormatCSVField(item.DBItemType.ToString()));
            sb.Append(FormatCSVField(item.TBaseType.ToString()));
            sb.Append(FormatCSVField(item.TType.ToString()));
            sb.Append(FormatCSVField(item.Quality.ToString()));
            sb.Append(FormatCSVField(item.Level));
            sb.Append(FormatCSVField(pickupItem));
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
                        _fileStream = File.Open(_droppedItemLogPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);

                    if (_logWriter == null)
                        _logWriter = new StreamWriter(_fileStream, Encoding.UTF8, bufferSize);

                    string queueItem;
                    while (_logItemQueue.TryDequeue(out queueItem))
                    {
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
                                Logger.LogError("Error in LogDroppedItems QueueWorker: " + ex.Message);
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
                    Logger.LogError("Error in LogDroppedItems QueueWorker: " + ex.Message);
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
