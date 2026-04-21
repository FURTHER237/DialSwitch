using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace DialSwitch
{
    class Program
    {
        // --- CONFIGURATION ---
        // 🛡️ REPLACED: Specific hardware GUIDs removed for privacy.
        // Users should follow the README to find their own device IDs.
        static readonly string[] DEVICES = {
            "YOUR_DEVICE_GUID_1", 
            "YOUR_DEVICE_GUID_2", 
            "YOUR_DEVICE_GUID_3", 
            "YOUR_DEVICE_GUID_4"
        };
        
        static int current_idx = 0;
        static int lastTick = 0;

        // --- WINDOWS COM INTERFACE ---
        // These remain intact as they are required for the system to talk to the audio service
        [ComImport, Guid("870AF99C-171D-4F9E-AF0D-E63DF40C2BC9")]
        private class PolicyConfigClient { }

        [ComImport, Guid("F8679F50-850A-41CF-9C72-430F290290C8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IPolicyConfig
        {
            [PreserveSig] int GetMixFormat([MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName, out IntPtr ppFormat);
            [PreserveSig] int GetDeviceFormat([MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName, int bDefault, out IntPtr ppFormat);
            [PreserveSig] int ResetDeviceFormat([MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName);
            [PreserveSig] int SetDeviceFormat([MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName, IntPtr pEndpointFormat, IntPtr mixFormat);
            [PreserveSig] int GetProcessingPeriod([MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName, int bDefault, out IntPtr pmftDefaultPeriod, out IntPtr pmftMinimumPeriod);
            [PreserveSig] int SetProcessingPeriod([MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName, IntPtr pmftPeriod);
            [PreserveSig] int GetShareMode([MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName, out IntPtr pMode);
            [PreserveSig] int SetShareMode([MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName, IntPtr mode);
            [PreserveSig] int GetPropertyValue([MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName, int bFxStore, IntPtr key, out IntPtr pv);
            [PreserveSig] int SetPropertyValue([MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName, int bFxStore, IntPtr key, IntPtr pv);
            [PreserveSig] int SetDefaultEndpoint([MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName, int role);
            [PreserveSig] int SetEndpointVisibility([MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName, int bVisible);
        }

        // --- WIN32 API FOR HOOKING ---
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)] private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", SetLastError = true)] private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll", SetLastError = true)] private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("user32.dll")] private static extern short GetAsyncKeyState(int vKey);
        [DllImport("user32.dll")] private static extern int GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
        [DllImport("user32.dll")] private static extern bool TranslateMessage(ref MSG lpMsg);
        [DllImport("user32.dll")] private static extern IntPtr DispatchMessage(ref MSG lpMsg);
        [DllImport("user32.dll")] private static extern void PostQuitMessage(int nExitCode);

        [StructLayout(LayoutKind.Sequential)] private struct MSG { public IntPtr hwnd; public uint message; public IntPtr wParam; public IntPtr lParam; public uint time; public POINT pt; }
        [StructLayout(LayoutKind.Sequential)] private struct POINT { public int x; public int y; }

        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        static void Main()
        {
            Console.WriteLine("DialSwitch Service Running...");
            _hookID = SetHook(_proc);
            MSG msg;
            while (GetMessage(out msg, IntPtr.Zero, 0, 0) > 0)
            {
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }
            UnhookWindowsHookEx(_hookID);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(13, proc, GetModuleHandle(curModule.ModuleName), 0); 
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)0x0100 || wParam == (IntPtr)0x0104)) 
            {
                int vkCode = Marshal.ReadInt32(lParam);

                // Exit sequence: Ctrl + Alt + Q
                if (vkCode == 0x51 && (GetAsyncKeyState(0x11) & 0x8000) != 0 && (GetAsyncKeyState(0x12) & 0x8000) != 0)
                {
                    Console.Beep(300, 150); Console.Beep(200, 150); Console.Beep(100, 300);
                    PostQuitMessage(0);
                    return (IntPtr)1;
                }

                bool winDown = (GetAsyncKeyState(0x5B) & 0x8000) != 0 || (GetAsyncKeyState(0x5C) & 0x8000) != 0;
                bool isWinL = winDown && (vkCode == 0x4C); 
                bool isPrintScreen = (vkCode == 0x2C);     

                if (isWinL || isPrintScreen)
                {
                    int currentTick = Environment.TickCount;
                    if (currentTick - lastTick > 400)
                    {
                        lastTick = currentTick;
                        current_idx = (current_idx + 1) % DEVICES.Length;
                        new Thread(CycleAudio).Start();
                    }
                    return (IntPtr)1; 
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private static void CycleAudio()
        {
            try
            {
                var policyConfig = (IPolicyConfig)new PolicyConfigClient();
                policyConfig.SetDefaultEndpoint(DEVICES[current_idx], 0);
                policyConfig.SetDefaultEndpoint(DEVICES[current_idx], 1);
                policyConfig.SetDefaultEndpoint(DEVICES[current_idx], 2);
                
                // Audio feedback: Higher index = Higher pitch
                Console.Beep(400 + (current_idx * 100), 100); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Switch Error: {ex.Message}");
                Console.Beep(200, 500); 
            }
        }
    }
}
