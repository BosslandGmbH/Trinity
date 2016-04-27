using System;
using System.Linq;
using System.Management;

namespace Trinity.Helpers
{
    public class SystemInformation
    {
        private static string _resolution;
        public static string Resolution
        {
            get
            {
                if (!_initialized)
                    Initialize();

                return _resolution;
            }
        }

        private static string _operatingSystem;
        public static string OperatingSystem
        {
            get
            {
                if (!_initialized)
                    Initialize();

                return _operatingSystem;
            }
        }

        private static string _videoCard;
        public static string VideoCard
        {
            get
            {
                if (!_initialized)
                    Initialize();

                return _videoCard;
            }
        }

        private static string _processor;
        public static string Processor
        {
            get
            {
                if (!_initialized)
                    Initialize();

                return _processor;
            }
        }

        private static string _actualProcessorSpeed;
        public static string ActualProcessorSpeed
        {
            get
            {
                if (!_initialized)
                    Initialize();

                return _actualProcessorSpeed;
            }
        }

        private static string _hardDisk;
        public static string HardDisk
        {
            get
            {
                if (!_initialized)
                    Initialize();

                return _hardDisk;
            }
        }

        private static string _freeMemory;
        public static string FreeMemory
        {
            get
            {
                if (!_initialized)
                    Initialize();

                return _freeMemory;
            }
        }

        private static string _systemType;
        public static string SystemType
        {
            get
            {
                if (!_initialized)
                    Initialize();

                return _systemType;
            }
        }

        private static string _motherBoard;
        public static string MotherBoard
        {
            get
            {
                if (!_initialized)
                    Initialize();

                return _motherBoard;
            }
        }

        private static bool _initialized;

        public static void Initialize()
        {
            _initialized = true;

            var win32OperatingSystem = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_OperatingSystem");
            var win32VideoController = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController");
            var win32ComputerSystem = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_ComputerSystem");
            var win32DiskDrive = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DiskDrive");
            var win32Processor = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");

            try
            {
                foreach (var queryObj in win32OperatingSystem.Get().Cast<ManagementObject>())
                {
                    _freeMemory = queryObj["FreePhysicalMemory"].ToString();
                    _operatingSystem = queryObj["Caption"].ToString();
                    queryObj.Dispose();
                }

                foreach (var queryObj in win32VideoController.Get().Cast<ManagementObject>())
                {
                    _videoCard = queryObj["Caption"].ToString();
                    _resolution = queryObj["CurrentHorizontalResolution"].ToString() + " x " + queryObj["CurrentVerticalResolution"].ToString();
                    queryObj.Dispose();                    
                }

                foreach (var queryObj in win32ComputerSystem.Get().Cast<ManagementObject>())
                {
                    _systemType = queryObj["SystemType"].ToString();
                    _motherBoard = queryObj["Model"].ToString();
                    queryObj.Dispose();
                }

                foreach (var queryObj in win32DiskDrive.Get().Cast<ManagementObject>())
                {
                    _hardDisk = queryObj["Model"].ToString();
                    queryObj.Dispose();

                    // Just grab the first one, system may have multiple drives
                    break;
                }

                foreach (var queryObj in win32Processor.Get().Cast<ManagementObject>())
                {
                    _processor = queryObj["Name"].ToString();
                    _actualProcessorSpeed = queryObj["CurrentClockSpeed"].ToString();
                    queryObj.Dispose();
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                win32OperatingSystem.Dispose();
                win32VideoController.Dispose();
                win32ComputerSystem.Dispose();
                win32DiskDrive.Dispose();
                win32Processor.Dispose();
            }

        }

    }
}
