using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CreateHiddenProcessLib.CreateWindowUtility;
using CreateHiddenProcessLib.CreateWindowUtility.Model;

namespace CreateHiddenProcess
{
    public partial class ParentForm : Form
    {

        public ParentForm()
        {
            InitializeComponent();
        }

        private void ParentForm_Load(object sender, EventArgs e)
        {
            HookFirefoxCreation();
        }

        public void HookFirefoxCreation()
        {
            ByRef<Process> process=new ByRef<Process>();
            WindowCreationHookerByClassName.GetInstance("MozillaWindowClass").HookFirefoxCreation(process, Instance_FirefowWindowCreated);
            var psi = new ProcessStartInfo();
            psi.FileName = @"C:\Program Files\Mozilla Firefox\firefox.exe";
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = false;
            psi.UseShellExecute = false;
            process.Value = Process.Start(psi);
        }

        private void Instance_FirefowWindowCreated(object sender, WindowHookEventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                SetParent(e.Handle, Handle);
            }));
            var windowCreationHookerByClassName = sender as WindowCreationHookerByClassName;
            windowCreationHookerByClassName?.Dispose();
        }

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);


    }
}