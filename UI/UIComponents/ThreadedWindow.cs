using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Demonbuddy;
using Trinity.Cache;
using Trinity.Helpers;
using Trinity.Technicals;
using Trinity.UIComponents;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Application = System.Windows.Application;
using Logger = Trinity.Technicals.Logger;
using ThreadState = System.Threading.ThreadState;
using UserControl = System.Windows.Controls.UserControl;

namespace Trinity.UI.UIComponents
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
        public ThreadedWindow(string xamlPath, object dataContext, string name = "", int height = 0, int width = 0)
        {            
            try
            {
                

                if (String.IsNullOrEmpty(xamlPath))
                    throw new ArgumentException("xamlPath");

                if (dataContext == null)
                    throw new ArgumentException("dataContext");

                if (!File.Exists(xamlPath))
                    throw new ArgumentException("xamlPath: file doesn't exist");

                _thread = new Thread(threadDataContext =>
                {
                    _window = new Window
                    {
                        Height = height > MinimumHeight ? height : 750,
                        Width = height > MininumWidth ? width : 1200,
                        MinHeight = MinimumHeight,
                        MinWidth = MininumWidth,
                        Title = name,
                        Content = UILoader.LoadAndTransformXamlFile<UserControl>(xamlPath),
                        DataContext = threadDataContext
                    };

                    IsWindowCreated = true;

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Application.Current.MainWindow.Closing += (sender, args) =>
                        {
                            Logger.Log("ThreadedWindow: Closing {0} window ", name);
                            _allowedToClose = true;
                            _dispatcher.InvokeShutdown();
                        };
                    });                    

                    // Hide window instead of closing to avoid WPF thread ownership issues.
                    _window.Closing += (sender, args) =>
                    {
                        if (!_allowedToClose)
                        {
                            Logger.Log("ThreadedWindow: Hiding {0} window ", name);                            
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
                        Logger.Log("ThreadedWindow: Shutting down {0} thread ", name);
                        _window.Dispatcher.InvokeShutdown();
                    };

                    _window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                    Dispatcher.Run();
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
                Logger.Log("Unable to create ThreadedWindow: {0}", ex);
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
