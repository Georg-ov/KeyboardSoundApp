﻿using System;
using System.Drawing;
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
            toggleButton.Text = isActive ? "Deactivate" : "Activate";
            toggleButton.BackColor = isActive ? Color.Red : Color.FromArgb(50, 150, 250);
            if (isActive) keyboardHook.HookKeyboard();
            else keyboardHook.UnhookKeyboard();
        }

        private void PlaySound(Keys key)
        {
            if (isActive)
            {
                try
                {
                    // Obtiene la ruta base del ejecutable
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;

                    // Construye la ruta relativa al archivo de sonido
                    string soundFile;

                    if (key == Keys.Enter)
                    {
                        soundFile = System.IO.Path.Combine(basePath, "resources", "sounds", "enter_1.wav");
                    }
                    else
                    {
                        soundFile = System.IO.Path.Combine(basePath, "resources", "sounds", "tecla_1.wav");
                    }

                    // Verifica si el archivo existe antes de intentar reproducirlo
                    if (System.IO.File.Exists(soundFile))
                    {
                        using (SoundPlayer player = new SoundPlayer(soundFile))
                        {
                            player.Play(); // Reproduce el sonido
                        }
                    }
                    else
                    {
                        MessageBox.Show("El archivo de sonido no se encontró: " + soundFile);
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
