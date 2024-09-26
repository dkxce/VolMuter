//
// C#
// VolMuter.VolForm1
// v 0.2, 26.09.2024
// https://github.com/dkxce/VolMuter
// en,ru,1251,utf-8
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using VolMuter.Properties;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace VolMuter
{
    public partial class VolForm : Form
    {
        public VolForm() => InitializeComponent();

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80; // WS_EX_TOOLWINDOW 
                return cp;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
            Opacity = 0;
            miActive.Click += MenuItemClick;
            base.OnLoad(e);
        }       

        private void volMuterByDkxceToolStripMenuItem_Click(object sender, EventArgs e) => System.Diagnostics.Process.Start("https://github.com/dkxce/VolMuter");

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();

        private void ListWindows()
        {
            miActive.Visible = false;            

            Dictionary<uint, ISimpleAudioVolume> appsAudio = ApplicationMuter.GetVolumeObjects();
            Dictionary<int, (string, string)> appsAll = ApplicationMuter.GetProcessesPaths();
            IDictionary<IntPtr, string> appsWindowed = OpenWindowGetter.GetOpenWindows();


            bool sett = true;
            foreach (KeyValuePair<IntPtr, string> appwin in appsWindowed)
            {
                if (appwin.Key == this.Handle) continue;
                OpenWindowGetter.GetWindowThreadProcessId(appwin.Key, out uint pid);
                if (sett)
                {
                    string path = appsAll.ContainsKey((int)pid) ? System.IO.Path.GetFileName(appsAll[(int)pid].Item2) : "-";
                    miActive.Text = $"{appwin.Value} {{{path}}} (P{pid})";
                    miActive.Tag = pid;
                    miActive.Enabled = true;
                    if (appsAudio.ContainsKey(pid))
                    {
                        bool? muted = ApplicationMuter.GetApplicationMute(pid);
                        if (muted.HasValue)
                        {
                            miActive.Checked = muted.Value;
                            miActive.Visible = true;
                        };
                        float? value = ApplicationMuter.GetApplicationVolume(pid);
                        if (value.HasValue)
                            miActive.Text += $" - {value.Value:0}%";
                        sett = false;
                    };
                };
                ToolStripMenuItem tsi = new ToolStripMenuItem($"{appwin.Value} (P{pid})");
                tsi.Tag = pid;
                tsi.Click += MenuItemClick;
                if (appsAudio.ContainsKey(pid))
                {
                    bool? muted = ApplicationMuter.GetApplicationMute(pid);
                    if (muted.HasValue)
                    {
                        tsi.Checked = muted.Value;
                        miWindows.DropDownItems.Add(tsi);
                    };
                    float? value = ApplicationMuter.GetApplicationVolume(pid);
                    if (value.HasValue)
                        tsi.Text += $" - {value.Value:0}%";
                };
            };
            if (sett)
            {
                miActive.Text = "[NO]";
                miActive.Enabled = false;
            };
            miWindows.Text = $"List Windows [{miWindows.DropDownItems.Count}]";
        }

        private void ListProcesses()
        {            
            Dictionary<uint, ISimpleAudioVolume> appsAudio = ApplicationMuter.GetVolumeObjects();
            Dictionary<int, (string, string)> appsPaths = ApplicationMuter.GetProcessesPaths();

            foreach (KeyValuePair<uint, ISimpleAudioVolume> app in appsAudio)
            {
                if (app.Key == 0) continue;
                string name = appsPaths.ContainsKey((int)app.Key) ? appsPaths[(int)app.Key].Item1 : "-";
                string path = appsPaths.ContainsKey((int)app.Key) ? System.IO.Path.GetFileName(appsPaths[(int)app.Key].Item2) : "-";
                ToolStripMenuItem tsi = new ToolStripMenuItem($"{name} {{{path}}} (P{app.Key})");
                tsi.Tag = app.Key;
                tsi.Click += MenuItemClick;
                bool? muted = ApplicationMuter.GetApplicationMute(app.Key);
                if (muted.HasValue)
                {
                    tsi.Checked = muted.Value;
                    miProcs.DropDownItems.Add(tsi);
                };
                float? value = ApplicationMuter.GetApplicationVolume(app.Key);
                if (value.HasValue)
                    tsi.Text += $" - {value.Value:0}%";
            };
            miProcs.Text = $"List Processes [{miProcs.DropDownItems.Count}]";
        }

        private void VMMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            miWindows.DropDownItems.Clear();
            miProcs.DropDownItems.Clear();
            ListWindows();
            ListProcesses();
        }

        private void MenuItemClick(object sender, EventArgs e)
        {
            uint procId = uint.Parse((sender as ToolStripMenuItem).Tag.ToString());
            bool? muted = ApplicationMuter.GetApplicationMute(procId);
            if (!muted.HasValue) return;
            ApplicationMuter.SetMute(procId, !muted.Value);
        }

        private void VolNotifier_Click(object sender, EventArgs e)
        {
            Point mp = MousePos.GetCursorPosition();
            VMMenu.Show(mp.X,mp.Y);
        }

        private void openSystemMixerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try { System.Diagnostics.Process.Start("sndvol.exe"); } catch { };
        }

        private void openSystemSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try { System.Diagnostics.Process.Start("ms-settings:apps-volume"); } catch { };            
        }
    }
}

