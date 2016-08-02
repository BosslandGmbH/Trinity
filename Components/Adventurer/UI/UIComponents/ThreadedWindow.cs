using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Trinity.Components.Adventurer.Util;

namespace Trinity.Components.Adventurer.UI.UIComponents
{
    /// <summary>
    /// A window with its own UI Thread.
    /// </summary>
    public class ThreadedWindow
    {
        private Window _window;
        private readonly Thread _thread;
        private bool _isWindowOpen;
        private Dispatcher _dispatcher;
        private bool _allowedToClose;
        private const int MininumWidth = 25;
        private const int MinimumHeight = 25;
        private readonly DispatcherTimer _internalTimer = new DispatcherTimer();
        private readonly Queue<Action> _queuedActions = new Queue<Action>();

        public bool IsWindowCreated;
        public bool IsWindowLoaded;

        /// <summary>
        /// A window with its own UI Thread.
        /// </summary>
        public ThreadedWindow(string path, object dataContext, string name = "", int height = 0, int width = 0)
        {
            try
            {
                if (String.IsNullOrEmpty(path))
                    throw new ArgumentException("xamlPath");

                if (dataContext == null)
                    throw new ArgumentException("dataContext");

                //var fullPathToFile = Path.Combine(FileManager.UiPath, path);
                var fullPathToFile = path;

                if (!File.Exists(fullPathToFile))
                    throw new ArgumentException("xamlPath: file doesn't exist");

                _thread = new Thread(threadDataContext =>
                {
                    try
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _window = new Window
                            {
                                Height = height > MinimumHeight ? height : 750,
                                Width = height > MininumWidth ? width : 1200,
                                MinHeight = MinimumHeight,
                                MinWidth = MininumWidth,
                                Title = name,
                                Content = UILoader.LoadAndTransformXamlFile<UserControl>(fullPathToFile),
                                //Content = UILoader.LoadWindowContent(FileManager.UiPath, filename),
                                DataContext = threadDataContext
                            };

                            IsWindowCreated = true;


                            Application.Current.MainWindow.Closing += (sender, args) =>
                            {
                                Logger.Debug("ThreadedWindow: Closing {0} window ", name);
                                _allowedToClose = true;
                                _dispatcher.InvokeShutdown();
                            };

                            // Hide window instead of closing to avoid WPF thread ownership issues.
                            _window.Closing += (sender, args) =>
                            {
                                if (!_allowedToClose)
                                {
                                    Logger.Debug("ThreadedWindow: Hiding {0} window ", name);
                                    _window.Hide();
                                    OnHidden();
                                    args.Cancel = true;
                                }
                                _isWindowOpen = false;
                            };

                            _window.ContentRendered += (sender, args) =>
                            {
                                IsWindowLoaded = true;
                            };

                            _window.Closed += (s, e) =>
                            {
                                Logger.Debug("ThreadedWindow: Shutting down {0} thread ", name);
                                _window.Dispatcher.InvokeShutdown();
                            };

                            _window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                            Dispatcher.Run();
                        });
                    }
                    catch (Exception ex)
                    {
                        Logger.Debug("Exception in ThreadedWindow '{0}'. XamlPath={1} {2}", name, path, ex);
                    }
                });

                // Single thread apartment prevents access to owned objects by other threads
                _thread.SetApartmentState(ApartmentState.STA);

                if (!String.IsNullOrEmpty(name))
                    _thread.Name = name + " ThreadedWindow";

                _thread.Start(dataContext);

                _internalTimer.Tick += InternalTimerTick;
                _internalTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
                _internalTimer.Start();
            }
            catch (Exception ex)
            {
                Logger.Debug("Unable to create ThreadedWindow: {0}", ex);
            }

        }

        // It takes a while for the thread to start up and new window to exist. 
        // This tick allows the delay of dispatch until thread/window are ready to handle them
        private void InternalTimerTick(object sender, EventArgs e)
        {
            if (!IsReadyToDispatch)
                return;

            var count = _queuedActions.Count;
            for (int i = 0; i < count; i++)
            {
                Dispatch(_queuedActions.Dequeue());
            }
        }

        private bool IsReadyToDispatch
        {
            get
            {
                return ThreadDispatcher != null && _thread != null && _thread.IsAlive && !_thread.ThreadState.HasFlag(ThreadState.Aborted | ThreadState.Stopped | ThreadState.Unstarted) && IsWindowCreated;
            }
        }

        public void Dispatch(Action action)
        {
            if (IsReadyToDispatch)
            {
                ThreadDispatcher.Invoke(DispatcherPriority.Send, new TimeSpan(0, 0, 0, 0, 500), action);
            }
            else
            {
                _queuedActions.Enqueue(action);
            }
        }

        public Dispatcher ThreadDispatcher
        {
            get { return _dispatcher ?? (_dispatcher = Dispatcher.FromThread(_thread)); }
        }

        public Thread Thread
        {
            get { return _thread; }
        }

        public bool IsWindowOpen
        {
            get { return _isWindowOpen; }
        }

        public void Show()
        {
            if (_thread == null)
                return;

            if (!_isWindowOpen)
            {
                Action action = () =>
                {
                    _isWindowOpen = true;
                    _window.Show();
                    _window.Activate();
                };

                Dispatch(action);
            }
        }

        public void Hide()
        {
            if (_isWindowOpen && _thread != null)
            {
                Action action = () =>
                {
                    _isWindowOpen = false;
                    _window.Hide();
                };

                Dispatch(action);
            }
        }

        public void Close()
        {
            if (_isWindowOpen && _thread != null)
                Dispatch(_window.Close);
        }

        public delegate void ThreadedWindowEvent();

        public event ThreadedWindowEvent OnHidden = () => { };
    }
}
