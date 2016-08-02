using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

using Buddy.Overlay;
using JetBrains.Annotations;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Actors;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Helpers;
using Trinity.Objects;
using Trinity.Technicals;
using Trinity.UI.UIComponents;
using Trinity.UI.UIComponents.RadarCanvas;
using Trinity.UIComponents;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.UI.Overlays
{

    public class ViewPortModel : NotifyBase
    {
        private double _cameraFieldOfView;
        private double _cameraPositionX;
        private double _cameraPositionY;
        private double _cameraPositionZ;
        private double _cameraLookDirectionX;
        private double _cameraLookDirectionY;
        private double _cameraLookDirectionZ;


        public ViewPortModel()
        {
            CameraFieldOfView = 0.6;
            CameraPosition = new Point3D(5017, 5682, 5212);
            CameraLookDirection = new Vector3D(-1, -1, -1);
            CameraUpDirection = new Vector3D(0, 0, 1);

            //CameraFieldOfView = 0.6;
            //CameraPosition = new Point3D(5000, 5000, 5000);
            //CameraLookDirection = new Vector3D(-1, -1, -1);
            //CameraUpDirection = new Vector3D(0, 0, 1);

            //// UP 0,0,1 FOV 25 POS -100, -100, 100, LOOK -1 -1 -1
            //CameraFieldOfView = 25;
            //CameraPosition = new Point3D(-100, -100, 100);
            //CameraLookDirection = new Vector3D(-1, -1, -1);
            //CameraScale = 1;

            //// UP 0,0,1 FOV 5.8 POS 50, 50, 68, LOOK -10 -6 -2
            //CameraFieldOfView = 5.8;
            //CameraPosition = new Point3D(50, 50, 68);
            //CameraLookDirection = new Vector3D(-10, -6, -2);

            ////FOV 50
            ////UP 0,1,0
            //CameraFieldOfView = 25;
            //CameraPosition = new Point3D(0, -36, -33);
            //CameraLookDirection = new Vector3D(0, 1, 1);

            // UP 0,1,0 / 50 FOV / Pos 0 -36 -32  // Look 0 1 1

            //UP 0,0,1
            //CameraFieldOfView = 20;
            //CameraPosition = new Point3D(0,28,20);
            //CameraLookDirection = new Vector3D(0,-6,-4.5);

            Objects = new ObservableCollection<TrinityActor>();

            Instance = this;

            using (ZetaDia.Memory.AcquireFrame())
            {
                _axialCylinderPosition = ZetaDia.Me.ActorInfo.AxialCylinder.Position;
                PlayerPosition = Internals.DemonBuddyObjects.ActivePlayerData.Position;
            }
            //StartThread();
        }

        public void StartThread()
        {
            StartThreadAllowed = false;

            Logger.LogVerbose("Starting Thread for ViewPortModel Updates");

            UpdateVisualizer();

            PlayerPosition = Internals.DemonBuddyObjects.ActivePlayerData.Position;

            Task.Factory.StartNew(UpdatePosition);

            OverlayHeight = (int)OverlayLoader.UnscaledGrid.ActualHeight;
            OverlayWidth = (int)OverlayLoader.UnscaledGrid.ActualWidth;

            Pulsator.OnPulse += (sender, args) =>
            {
                PlayerPosition = ZetaDia.Me.Position;
            };

        }

        private void UpdatePosition()
        {
            while (true)
            {
                if (DateTime.UtcNow.Subtract(_lastUpdate).TotalMilliseconds > 250)
                {
                    //using (ZetaDia.Memory.ReleaseFrame())
                    //{

                    //}
                    using (ZetaDia.Memory.AcquireFrame())
                    {
                        //PlayerPosition = ZetaDia.Me.Position;

                        PlayerPosition = Internals.DemonBuddyObjects.ActivePlayerData.Position - _axialCylinderPosition; //MathEx.GetPointAt(ZetaDia.Me.Position, ZetaDia.Me.Position  ZetaDia.Me.CollisionSphere.Center, ZetaDia.Me.Movement.Rotation);// MathEx.WrapAngle((float)(ZetaDia.Me.Movement.Rotation + Math.PI)));

                        if (_lastPosition == PlayerPosition)
                        {
                            Logger.Log("Updated Position: {0} UNCHANGED", DateTime.UtcNow.Subtract(_lastUpdate).TotalMilliseconds);
                        }
                        else
                        {
                            Logger.Log("Updated Position: {0}", DateTime.UtcNow.Subtract(_lastUpdate).TotalMilliseconds);
                        }

                        _lastUpdate = DateTime.UtcNow;
                        _lastPosition = PlayerPosition;
                    }
                }
            }
        }

        //private void UpdateAction()
        //{
        //    //while (true)
        //    //{
        //        if (!BotMain.IsPausedForStateExecution && !BotMain.IsRunning)
        //        {
        //            UpdateVisualizer();
        //        }
        //        //Thread.Sleep(10);
        //    //}
        //}

        public bool StartThreadAllowed
        {
            get { return _startThreadAllowed; }
            set { SetField(ref _startThreadAllowed, value); }
        }

        public DateTime LastUpdatedNav = DateTime.MinValue;

        public DateTime LastUpdated = DateTime.MinValue;

        private void StopThread()
        {
            if (Worker.IsRunning)
                Logger.LogVerbose("Stopping Thread for Visualizer Updates");

            Worker.Stop();
            StartThreadAllowed = true;
        }

        public void UpdateVisualizer()
        {
            using (new PerformanceLogger("3D ViewPortModel Update"))
            {
                if (DateTime.UtcNow.Subtract(LastRefresh).TotalMilliseconds <= RefreshRateMs)
                    return;

                List<TrinityActor> objects;
                using (ZetaDia.Memory.AcquireFrame())
                {
                    ZetaDia.Actors.Update();

                    var myAcd = ZetaDia.Me.ACDId;
                    objects = ZetaDia.Actors.GetActorsOfType<DiaUnit>(true, true)
                        .Where(a => a.IsNPC || a.ACDId == myAcd)
                        .Select(ActorFactory.CreateActor).ToList();
                }

                Logger.Log("UpdateVisualizer");

                LastRefresh = DateTime.UtcNow;
                Objects = new ObservableCollection<TrinityActor>(objects);
            }
        }

        public Vector3 PlayerPosition { get; set; }

        public ICommand StartThreadCommand
        {
            get
            {
                return new RelayCommand(param =>
                {
                    try
                    {
                        StartThread();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Exception in StartThreadCommand Command. {0}", ex);
                    }
                });
            }
        }

        //[DllImport("user32.dll")]
        //static extern IntPtr SetWindowsHookEx
        //    (int idHook, keyboardHookProc callback, IntPtr hInstance, uint threadId);

        //IntPtr hInstance = LoadLibrary("User32");
        //hhook = SetWindowsHookEx(WH_KEYBOARD_LL, hookProc, hInstance, 0);

        //private void Form1_Load(object sender, EventArgs e)
        //{
        //    gkh.HookedKeys.Add(Keys.A);
        //    gkh.HookedKeys.Add(Keys.B);
        //    gkh.KeyDown += new KeyEventHandler(gkh_KeyDown);
        //    gkh.KeyUp += new KeyEventHandler(gkh_KeyUp);
        //}

        //void gkh_KeyUp(object sender, KeyEventArgs e)
        //{
        //    lstLog.Items.Add("Up\t" + e.KeyCode.ToString());
        //    e.Handled = true;
        //}

        //void gkh_KeyDown(object sender, KeyEventArgs e)
        //{
        //    lstLog.Items.Add("Down\t" + e.KeyCode.ToString());
        //    e.Handled = true;
        //}

        public static ViewPortModel Instance { get; set; }

        public static ObservableCollection<TrinityActor> StaticObjects
        {
            get { return Instance.Objects; }
        }

        public ObservableCollection<TrinityActor> Objects
        {
            get { return _objects; }
            set { SetField(ref _objects, value); }
        }

        public double RefreshRateMs = 1000;

        public DateTime LastRefresh = DateTime.MinValue;
        private ObservableCollection<TrinityActor> _objects;
        private bool _startThreadAllowed;
        private double _cameraScale;
        private double _cameraFarPlaneDistance;
        private double _cameraNearPlaneDistance;
        private DateTime _lastUpdate = DateTime.MinValue;
        private int _overlayWidth;
        private int _overlayHeight;
        private Vector3 _axialCylinderPosition;
        private Vector3 _lastPosition;
        private double _cameraUpDirectionZ;
        private double _cameraUpDirectionY;
        private double _cameraUpDirectionX;

        public double CameraFieldOfView
        {
            get { return _cameraFieldOfView; }
            set
            {
                SetField(ref _cameraFieldOfView, value);
            }
        }

        public double CameraPositionX
        {
            get { return _cameraPositionX; }
            set
            {
                SetField(ref _cameraPositionX, value);
                OnPropertyChanged("CameraPosition");
            }
        }

        public double CameraScale
        {
            get { return _cameraScale; }
            set
            {
                SetField(ref _cameraScale, value);
                OnPropertyChanged("CameraPosition");
            }
        }

        public double CameraPositionY
        {
            get { return _cameraPositionY; }
            set
            {
                SetField(ref _cameraPositionY, value);
                OnPropertyChanged("CameraPosition");
            }
        }

        public int OverlayHeight
        {
            get { return _overlayHeight; }
            set { SetField(ref _overlayHeight, value); }
        }

        public int OverlayWidth
        {
            get { return _overlayWidth; }
            set { SetField(ref _overlayWidth, value); }
        }

        public double CameraPositionZ
        {
            get { return _cameraPositionZ; }
            set
            {
                SetField(ref _cameraPositionZ, value);
                OnPropertyChanged("CameraPosition");
            }
        }

        public double CameraNearPlaneDistance
        {
            get { return _cameraNearPlaneDistance; }
            set { SetField(ref _cameraNearPlaneDistance, value); }
        }

        public double CameraFarPlaneDistance
        {
            get { return _cameraFarPlaneDistance; }
            set { SetField(ref _cameraFarPlaneDistance, value); }
        }

        public Point3D CameraPosition
        {
            get
            {
                return new Point3D(PlayerPosition.X + CameraPositionX, PlayerPosition.Y + CameraPositionY, PlayerPosition.Z + CameraPositionZ);
            }
            set
            {
                if (Math.Abs(value.X - CameraPositionX) > double.Epsilon)
                    CameraPositionX = value.X;

                if (Math.Abs(value.Y - CameraPositionY) > double.Epsilon)
                    CameraPositionY = value.Y;

                if (Math.Abs(value.Z - CameraPositionZ) > double.Epsilon)
                    CameraPositionZ = value.Z;
            }
        }

        public double CameraLookDirectionX
        {
            get { return _cameraLookDirectionX; }
            set
            {
                SetField(ref _cameraLookDirectionX, value);
                OnPropertyChanged("CameraLookDirection");
            }
        }

        public double CameraLookDirectionY
        {
            get { return _cameraLookDirectionY; }
            set
            {
                SetField(ref _cameraLookDirectionY, value);
                OnPropertyChanged("CameraLookDirection");
            }
        }

        public double CameraLookDirectionZ
        {
            get { return _cameraLookDirectionZ; }
            set
            {
                SetField(ref _cameraLookDirectionZ, value);
                OnPropertyChanged("CameraLookDirection");
            }
        }

        public Vector3D CameraLookDirection
        {
            get
            {
                return new Vector3D(CameraLookDirectionX, CameraLookDirectionY, CameraLookDirectionZ);
            }
            set
            {
                if (Math.Abs(value.X - CameraLookDirectionX) > double.Epsilon)
                    CameraLookDirectionX = value.X;

                if (Math.Abs(value.Y - CameraLookDirectionY) > double.Epsilon)
                    CameraLookDirectionY = value.Y;

                if (Math.Abs(value.Z - CameraLookDirectionZ) > double.Epsilon)
                    CameraLookDirectionZ = value.Z;
            }
        }

        public double CameraUpDirectionX
        {
            get { return _cameraUpDirectionX; }
            set
            {
                SetField(ref _cameraUpDirectionX, value);
                OnPropertyChanged("CameraUpDirection");
            }
        }

        public double CameraUpDirectionY
        {
            get { return _cameraUpDirectionY; }
            set
            {
                SetField(ref _cameraUpDirectionY, value);
                OnPropertyChanged("CameraUpDirection");
            }
        }

        public double CameraUpDirectionZ
        {
            get { return _cameraUpDirectionZ; }
            set
            {
                SetField(ref _cameraUpDirectionZ, value);
                OnPropertyChanged("CameraUpDirection");
            }
        }

        public Vector3D CameraUpDirection
        {
            get
            {
                return new Vector3D(CameraUpDirectionX, CameraUpDirectionY, CameraUpDirectionZ);
            }
            set
            {
                if (Math.Abs(value.X - CameraUpDirectionX) > double.Epsilon)
                    CameraUpDirectionX = value.X;

                if (Math.Abs(value.Y - CameraUpDirectionY) > double.Epsilon)
                    CameraUpDirectionY = value.Y;

                if (Math.Abs(value.Z - CameraUpDirectionZ) > double.Epsilon)
                    CameraUpDirectionZ = value.Z;
            }
        }


    }
}

