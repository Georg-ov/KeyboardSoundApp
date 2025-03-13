using System;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KeyboardSoundApp
{
    public partial class MainForm : Form
    {
        private bool isActive = false;
        private LowLevelKeyboardHook keyboardHook;

        public MainForm()
        {
            InitializeComponent();
            keyboardHook = new LowLevelKeyboardHook();
            keyboardHook.OnKeyPressed += PlaySound;
        }

        private void toggleButton_Click(object sender, EventArgs e)
        {
            isActive = !isActive;
            toggleButton.Text = isActive ? "Desactivar" : "Activar";
            if (isActive) keyboardHook.HookKeyboard();
            else keyboardHook.UnhookKeyboard();
        }

        private void PlaySound(Keys key)
        {
            if (isActive)
            {
                try
                {
                    string soundFile;

                    if (key == Keys.Enter)
                    {
                        soundFile = "C:\\Users\\Yoki\\source\\repos\\KeyboardSoundApp\\KeyboardSoundApp\\resources\\sounds\\enter_1.wav";
                    }
                    else
                    {
                        soundFile = "C:\\Users\\Yoki\\source\\repos\\KeyboardSoundApp\\KeyboardSoundApp\\resources\\sounds\\tecla_1.wav";
                    }

                    using (SoundPlayer player = new SoundPlayer(soundFile))
                    {
                        player.Play(); // Reproduce el sonido
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al reproducir el sonido: " + ex.Message);
                }
            }
        }


        public class LowLevelKeyboardHook
        {

            private const int WH_KEYBOARD_LL = 13;
            private const int WM_KEYDOWN = 0x0100;
            private IntPtr hookId = IntPtr.Zero;
            private HookProc hookCallback;

            public event Action<Keys> OnKeyPressed;

            public LowLevelKeyboardHook()
            {
                hookCallback = HookCallback;
            }

            public void HookKeyboard()
            {
                hookId = SetHook(hookCallback);
            }

            public void UnhookKeyboard()
            {
                UnhookWindowsHookEx(hookId);
            }

            private IntPtr SetHook(HookProc proc)
            {
                using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
                using (var curModule = curProcess.MainModule)
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }

            private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
            {
                if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
                {
                    int vkCode = Marshal.ReadInt32(lParam);
                    OnKeyPressed?.Invoke((Keys)vkCode);
                }
                return CallNextHookEx(hookId, nCode, wParam, lParam);
            }

            [DllImport("user32.dll")]
            private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool UnhookWindowsHookEx(IntPtr hhk);

            [DllImport("user32.dll")]
            private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr GetModuleHandle(string lpModuleName);

            private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        }
    }
}
