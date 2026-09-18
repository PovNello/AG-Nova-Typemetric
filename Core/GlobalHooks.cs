using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Typemetric.Core
{
    public class GlobalHooks : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE_LL = 14;

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_MBUTTONDOWN = 0x0207;
        private const int WM_MOUSEWHEEL = 0x020A;

        private IntPtr _keyboardHookID = IntPtr.Zero;
        private IntPtr _mouseHookID = IntPtr.Zero;

        private readonly LowLevelProc _keyboardProc;
        private readonly LowLevelProc _mouseProc;

        public event Action<string> KeyPressed;
        public event Action<string> MouseClicked;
        public event Action<int> MouseScrolled;

        public GlobalHooks()
        {
            _keyboardProc = KeyboardHookCallback;
            _mouseProc = MouseHookCallback;
        }

        public void Start()
        {
            _keyboardHookID = SetHook(WH_KEYBOARD_LL, _keyboardProc);
            _mouseHookID = SetHook(WH_MOUSE_LL, _mouseProc);
        }

        public void Stop()
        {
            UnhookWindowsHookEx(_keyboardHookID);
            UnhookWindowsHookEx(_mouseHookID);
        }

        private IntPtr SetHook(int idHook, LowLevelProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(idHook, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                System.Windows.Input.Key wpfKey = System.Windows.Input.KeyInterop.KeyFromVirtualKey(vkCode);
                KeyPressed?.Invoke(wpfKey.ToString());
            }
            return CallNextHookEx(_keyboardHookID, nCode, wParam, lParam);
        }

        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                if (wParam == (IntPtr)WM_LBUTTONDOWN)
                {
                    MouseClicked?.Invoke("Left");
                }
                else if (wParam == (IntPtr)WM_RBUTTONDOWN)
                {
                    MouseClicked?.Invoke("Right");
                }
                else if (wParam == (IntPtr)WM_MBUTTONDOWN)
                {
                    MouseClicked?.Invoke("Middle");
                }
                else if (wParam == (IntPtr)WM_MOUSEWHEEL)
                {
                    int mouseData = Marshal.ReadInt32(lParam + 8);
                    int delta = (mouseData >> 16) & 0xffff;
                    if ((delta & 0x8000) != 0) delta = delta | unchecked((int)0xffff0000);
                    MouseScrolled?.Invoke(delta);
                }
            }
            return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            Stop();
        }

        // P/Invoke
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
