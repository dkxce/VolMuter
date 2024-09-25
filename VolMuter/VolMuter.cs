//
// C#
// VolMuter.ApplicationMuter
// v 0.1, 25.09.2024
// https://github.com/dkxce/VolMuter
// en,ru,1251,utf-8
//

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Management;
using System.Collections.Generic;

namespace VolMuter
{
    public static class ApplicationMuter
    {
        public static void Test()
        {
            const int app = 21348;

            Console.WriteLine("Mute:" + GetApplicationMute(app));
            Console.WriteLine("Volume:" + GetApplicationVolume(app));

            // mute the application
            SetApplicationMute(app, false);

            // set the volume to half of master volume (50%)
            // SetApplicationVolume(app, 100);
        }

        #region Mute
        public static bool SetMute(uint ProcessID, bool mute) => SetApplicationMute(ProcessID, mute);

        public static bool SetMute(string ProcessName, bool mute) => SetApplicationMute(GetProcessByName(ProcessName), mute);

        public static bool SetMute(bool mute, string ProcessExeName) => SetApplicationMute(GetProcessByExe(ProcessExeName), mute);
        #endregion Mute

        #region Volume
        public static bool SetVolume(uint ProcessID, int vol_0_100) => SetApplicationVolume(ProcessID, (float)vol_0_100);

        public static bool SetVolume(string ProcessName, int vol_0_100) => SetApplicationVolume(GetProcessByName(ProcessName), (float)vol_0_100);

        public static bool SetVolume(int vol_0_100, string ProcessExeName) => SetApplicationVolume(GetProcessByExe(ProcessExeName), (float)vol_0_100);
        #endregion Volume

        #region process
        public static uint GetProcessByName(string ProcessName)
        {
            foreach (System.Diagnostics.Process proc in System.Diagnostics.Process.GetProcesses())
                if (string.Compare(proc.ProcessName, ProcessName, StringComparison.OrdinalIgnoreCase) == 0)
                    return (uint)proc.Id;
            return uint.MaxValue;
        }

        public static uint GetProcessByExe(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return uint.MaxValue; else filename = filename.ToLower();

            string wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
            using (var searcher = new ManagementObjectSearcher(wmiQueryString))
            using (var results = searcher.Get())
            {
                var query = from p in Process.GetProcesses()
                            join mo in results.Cast<ManagementObject>()
                            on p.Id equals (int)(uint)mo["ProcessId"]
                            select new
                            {
                                Process = p,
                                Path = (string)mo["ExecutablePath"],
                                CommandLine = (string)mo["CommandLine"],
                            };
                foreach (var item in query)
                    if (item.Path != null && item.Path.ToLower().EndsWith(filename))
                        return (uint)item.Process.Id;
            };
            return uint.MaxValue;
        }

        public static string GetProcessByPID(int pid)
        {
            string wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
            using (var searcher = new ManagementObjectSearcher(wmiQueryString))
            using (var results = searcher.Get())
            {
                var query = from p in Process.GetProcesses()
                            join mo in results.Cast<ManagementObject>()
                            on p.Id equals (int)(uint)mo["ProcessId"]
                            select new
                            {
                                Process = p,
                                Path = (string)mo["ExecutablePath"],
                                CommandLine = (string)mo["CommandLine"],
                            };
                foreach (var item in query)
                    if (pid == item.Process.Id)
                        return item.Path.ToLower();
            };
            return null;
        }

        public static Dictionary<int, (string, string)> GetProcessesPaths()
        {
            Dictionary<int, (string, string)> res = new Dictionary<int, (string, string)>();
            string wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
            using (var searcher = new ManagementObjectSearcher(wmiQueryString))
            using (var results = searcher.Get())
            {
                var query = from p in Process.GetProcesses()
                            join mo in results.Cast<ManagementObject>()
                            on p.Id equals (int)(uint)mo["ProcessId"]
                            select new
                            {
                                Process = p,
                                Path = (string)mo["ExecutablePath"],
                                CommandLine = (string)mo["CommandLine"],
                            };
                foreach (var item in query) res.Add(item.Process.Id, (item.Process.ProcessName, item.Path));
            };
            return res;
        }
        #endregion process        

        #region GET
        public static float? GetApplicationVolume(uint procId)
        {
            ISimpleAudioVolume volume = GetVolumeObject(procId);
            if (volume == null)
                return null;

            float level;
            volume.GetMasterVolume(out level);
            return level * 100;
        }

        public static bool? GetApplicationMute(uint procId)
        {
            ISimpleAudioVolume volume = GetVolumeObject(procId);
            if (volume == null) return null;

            bool mute;
            volume.GetMute(out mute);
            return mute;
        }


        public static bool SetApplicationVolume(uint procId, float level)
        {
            if (procId == uint.MaxValue) return false;

            ISimpleAudioVolume volume = GetVolumeObject(procId);
            if (volume == null) return false;

            Guid guid = Guid.Empty;
            int res = volume.SetMasterVolume(level / 100, ref guid);
            return res == 0;
        }

        public static bool SetApplicationMute(uint procId, bool mute)
        {
            if (procId == uint.MaxValue) return false;

            ISimpleAudioVolume volume = GetVolumeObject(procId);
            if (volume == null) return false;

            Guid guid = Guid.Empty;
            int res = volume.SetMute(mute, ref guid);
            return res == 0;
        }
       
        
        private static ISimpleAudioVolume GetVolumeObject(uint pid)
        {
            // Get the speakers (1st render + multimedia) device
            IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")));
            IMMDevice speakers;
            deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out speakers);

            // Activate the session manager, enumerator
            Guid IID_IAudioSessionManager2 = typeof(IAudioSessionManager2).GUID;
            speakers.Activate(ref IID_IAudioSessionManager2, 0, IntPtr.Zero, out object o);
            IAudioSessionManager2 mgr = (IAudioSessionManager2)o;

            // enumerate sessions for on this device
            IAudioSessionEnumerator sessionEnumerator;
            mgr.GetSessionEnumerator(out sessionEnumerator);
            sessionEnumerator.GetCount(out int count);

            ISimpleAudioVolume volumeControl = null;
            for (int i = 0; i < count; i++)
            {
                IAudioSessionControl ctl;
                sessionEnumerator.GetSession(i, out ctl);

                IAudioSessionControl2 ctl2 = (IAudioSessionControl2)ctl;
                ctl2.GetProcessId(out uint val);

                if (val == pid)
                {
                    volumeControl = ctl as ISimpleAudioVolume;
                    break;
                };

                Marshal.ReleaseComObject(ctl);
                Marshal.ReleaseComObject(ctl2);
            };

            Marshal.ReleaseComObject(sessionEnumerator);
            Marshal.ReleaseComObject(mgr);
            Marshal.ReleaseComObject(speakers);
            Marshal.ReleaseComObject(deviceEnumerator);

            return volumeControl;
        }

        public static Dictionary<uint, ISimpleAudioVolume> GetVolumeObjects()
        {
            Dictionary<uint, ISimpleAudioVolume> result = new Dictionary<uint, ISimpleAudioVolume>();

            // Get the speakers (1st render + multimedia) device
            IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")));
            IMMDevice speakers;
            deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out speakers);

            // Activate the session manager, enumerator
            Guid IID_IAudioSessionManager2 = typeof(IAudioSessionManager2).GUID;
            speakers.Activate(ref IID_IAudioSessionManager2, 0, IntPtr.Zero, out object o);
            IAudioSessionManager2 mgr = (IAudioSessionManager2)o;

            // enumerate sessions for on this device
            IAudioSessionEnumerator sessionEnumerator;
            mgr.GetSessionEnumerator(out sessionEnumerator);
            sessionEnumerator.GetCount(out int count);

            ISimpleAudioVolume volumeControl = null;
            for (int i = 0; i < count; i++)
            {
                IAudioSessionControl ctl;
                sessionEnumerator.GetSession(i, out ctl);

                IAudioSessionControl2 ctl2 = (IAudioSessionControl2)ctl;
                ctl2.GetProcessId(out uint val);
                result.Add(val, ctl as ISimpleAudioVolume);                
                Marshal.ReleaseComObject(ctl);
                Marshal.ReleaseComObject(ctl2);
            };

            Marshal.ReleaseComObject(sessionEnumerator);
            Marshal.ReleaseComObject(mgr);
            Marshal.ReleaseComObject(speakers);
            Marshal.ReleaseComObject(deviceEnumerator);

            return result;
        }
        #endregion GET
    }

    #region PRIVATES
    [ComImport]
    [Guid("bfb7ff88-7239-4fc9-8fa2-07c950be9c6d"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IAudioSessionControl2
    {
        [PreserveSig]
        int GetState(out object state);
        [PreserveSig]
        int GetDisplayName(out IntPtr name);
        [PreserveSig]
        int SetDisplayName(string value, Guid EventContext);
        [PreserveSig]
        int GetIconPath(out IntPtr Path);
        [PreserveSig]
        int SetIconPath(string Value, Guid EventContext);
        [PreserveSig]
        int GetGroupingParam(out Guid GroupingParam);
        [PreserveSig]
        int SetGroupingParam(Guid Override, Guid Eventcontext);
        [PreserveSig]
        int RegisterAudioSessionNotification(object NewNotifications);
        [PreserveSig]
        int UnregisterAudioSessionNotification(object NewNotifications);

        [PreserveSig]
        int GetSessionIdentifier(out IntPtr retVal);
        [PreserveSig]
        int GetSessionInstanceIdentifier(out IntPtr retVal);
        [PreserveSig]
        int GetProcessId(out UInt32 retvVal);
        [PreserveSig]
        int IsSystemSoundsSession();
        [PreserveSig]
        int SetDuckingPreference(bool optOut);
    }
    [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
    internal class MMDeviceEnumerator {}

    internal enum EDataFlow
    {
        eRender,
        eCapture,
        eAll,
        EDataFlow_enum_count
    }

    internal enum ERole
    {
        eConsole,
        eMultimedia,
        eCommunications,
        ERole_enum_count
    }

    [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMDeviceEnumerator
    {
        int NotImpl1();

        [PreserveSig]
        int GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role, out IMMDevice ppDevice);

        // the rest is not implemented
    }

    [Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMDevice
    {
        [PreserveSig]
        int Activate(ref Guid iid, int dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);

        // the rest is not implemented
    }

    [Guid("77AA99A0-1BD6-484F-8BC7-2C654C9A9B6F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioSessionManager2
    {
        int NotImpl1();
        int NotImpl2();

        [PreserveSig]
        int GetSessionEnumerator(out IAudioSessionEnumerator SessionEnum);

        // the rest is not implemented
    }

    [Guid("E2F5BB11-0570-40CA-ACDD-3AA01277DEE8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioSessionEnumerator
    {
        [PreserveSig]
        int GetCount(out int SessionCount);

        [PreserveSig]
        int GetSession(int SessionCount, out IAudioSessionControl Session);
    }

    [Guid("F4B1A599-7266-4319-A8CA-E70ACB11E8CD"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioSessionControl
    {
        int NotImpl1();

        [PreserveSig]
        int GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);

        // the rest is not implemented
    }

    [Guid("87CE5498-68D6-44E5-9215-6DA47EF883D8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ISimpleAudioVolume
    {
        [PreserveSig]
        int SetMasterVolume(float fLevel, ref Guid EventContext);

        [PreserveSig]
        int GetMasterVolume(out float pfLevel);

        [PreserveSig]
        int SetMute(bool bMute, ref Guid EventContext);

        [PreserveSig]
        int GetMute(out bool pbMute);
    }
    #endregion PRIVATES
}