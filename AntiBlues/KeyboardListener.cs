using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using AntiBlues.Helpers;
using System.IO;

namespace AntiBlues
{
    // Includes code from http://stackoverflow.com/a/604417 for listening for keypresses.
    static class KeyboardListener
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        private static RateTrigger mRateTrigger;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Try 
            Config c = null;
            try {
                c = new Config("appconfig.xml");
            } catch(FileNotFoundException e) {
                System.Xml.Linq.XDocument xdoc = new System.Xml.Linq.XDocument();
                xdoc.Add(new System.Xml.Linq.XElement("configfile"));
                xdoc.Save("appconfig.xml");

                c = new Config("appconfig.xml", xdoc);
                c.setConfig("ActicationPoint", (10).ToString());
                c.setConfig("SamlePeriod", (2000).ToString());
                c.setConfig("Cooldown", (5000).ToString());
            }

            int act_point = c.getConfig<int>("ActivationPoint", 10);
            int sample_period = c.getConfig<int>("SamplePeriod", 2000);
            int cooldown = c.getConfig<int>("Cooldown", 5000);

            // Set up a trigger on 10 keys pressed in less than two seconds
            // and set up a handler for reaching the activation point that
            // mutes the microphone.
            mRateTrigger = new RateTrigger(act_point, sample_period);
            MicrophoneHandling mic = new MicrophoneHandling(cooldown);
            mRateTrigger.ActivationPointReached += mic.TriggeredLimit;

            // Set up hooks and stuff for the application.
            _hookID = SetHook(_proc);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            UnhookWindowsHookEx(_hookID);
        }

        #region Keyboard listening hooks.
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                mRateTrigger.Hit();
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        #endregion

    }
}
