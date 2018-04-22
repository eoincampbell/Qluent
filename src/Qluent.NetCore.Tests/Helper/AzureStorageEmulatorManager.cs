namespace Qluent.NetCore.Tests.Helper
{
    using System.Diagnostics;
    using System.Linq;

    public static class AzureStorageEmulatorManager
    {
        private const string WindowsAzureStorageEmulatorPath =
            @"";

        private const string Win10ProcessName = "AzureStorageEmulator";

        private static readonly ProcessStartInfo StartStorageEmulator = new ProcessStartInfo
        {
            FileName = WindowsAzureStorageEmulatorPath,
            Arguments = "start",
        };

        private static readonly ProcessStartInfo StopStorageEmulator = new ProcessStartInfo
        {
            FileName = WindowsAzureStorageEmulatorPath,
            Arguments = "stop",
        };

        private static Process GetProcess()
        {
            return Process.GetProcessesByName(Win10ProcessName).FirstOrDefault();
        }

        public static bool IsProcessStarted()
        {
            return GetProcess() != null;
        }

        public static void Start()
        {
            if (IsProcessStarted()) return;

            using (var process = Process.Start(StartStorageEmulator))
            {
                process?.WaitForExit();
            }
        }

        public static void Stop()
        {
            using (var process = Process.Start(StopStorageEmulator))
            {
                process?.WaitForExit();
            }
        }
    }
}
