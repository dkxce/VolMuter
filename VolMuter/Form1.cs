//
// C#
// VolMuter.VolForm1
// v 0.1, 25.09.2024
// https://github.com/dkxce/VolMuter
// en,ru,1251,utf-8
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace VolMuter
{
    public partial class VolForm : Form
    {
        public VolForm() => InitializeComponent();

        protected override void OnLoad(EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;
            Opacity = 0;
            activeToolStripMenuItem.Click += MenuItemClick;
            base.OnLoad(e);
        }       

        private void volMuterByDkxceToolStripMenuItem_Click(object sender, EventArgs e) => System.Diagnostics.Process.Start("https://github.com/dkxce/VolMuter");

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();

        private void ListWindows()
        {
            activeToolStripMenuItem.Visible = false;
            Dictionary<uint, ISimpleAudioVolume> apps = ApplicationMuter.GetVolumeObjects();
            Dictionary<int, (string, string)> appx = ApplicationMuter.GetProcessesPaths();

            lwitem.DropDownItems.Clear();
            bool sett = true;
            foreach (KeyValuePair<IntPtr, string> window in OpenWindowGetter.GetOpenWindows())
            {
                if (window.Key == this.Handle) continue;
                OpenWindowGetter.GetWindowThreadProcessId(window.Key, out uint pid);
                if (sett)
                {
                    string path = appx.ContainsKey((int)pid) ? System.IO.Path.GetFileName(appx[(int)pid].Item2) : "-";
                    activeToolStripMenuItem.Text = $"{window.Value} {{{path}}} (P{pid})";
                    activeToolStripMenuItem.Tag = pid;
                    activeToolStripMenuItem.Enabled = true;
                    if (apps.ContainsKey(pid))
                    {
                        bool? muted = ApplicationMuter.GetApplicationMute(pid);
                        if (muted.HasValue)
                        {
                            activeToolStripMenuItem.Checked = muted.Value;
                            activeToolStripMenuItem.Visible = true;
                        };
                        float? value = ApplicationMuter.GetApplicationVolume(pid);
                        if (value.HasValue)
                            activeToolStripMenuItem.Text += $" - {value.Value:0}%";
                        sett = false;
                    };
                };
                ToolStripMenuItem tsi = new ToolStripMenuItem($"{window.Value} (P{pid})");
                tsi.Tag = pid;
                tsi.Click += MenuItemClick;
                if (apps.ContainsKey(pid))
                {
                    bool? muted = ApplicationMuter.GetApplicationMute(pid);
                    if (muted.HasValue)
                    {
                        tsi.Checked = muted.Value;
                        lwitem.DropDownItems.Add(tsi);
                    };
                    float? value = ApplicationMuter.GetApplicationVolume(pid);
                    if (value.HasValue)
                        tsi.Text += $" - {value.Value:0}%";
                };
            };
            if (sett)
            {
                activeToolStripMenuItem.Text = "[NO]";
                activeToolStripMenuItem.Enabled = false;
            };
            lwitem.Text = $"List Windows [{lwitem.DropDownItems.Count}]";
        }

        private void ListProcesses()
        {
            lpitem.DropDownItems.Clear();
            Dictionary<uint, ISimpleAudioVolume> apps = ApplicationMuter.GetVolumeObjects();
            Dictionary<int, (string, string)> appx = ApplicationMuter.GetProcessesPaths();
            foreach (KeyValuePair<uint, ISimpleAudioVolume> app in apps)
            {
                if (app.Key == 0) continue;
                string name = appx.ContainsKey((int)app.Key) ? appx[(int)app.Key].Item1 : "-";
                string path = appx.ContainsKey((int)app.Key) ? System.IO.Path.GetFileName(appx[(int)app.Key].Item2) : "-";
                ToolStripMenuItem tsi = new ToolStripMenuItem($"{name} {{{path}}} (P{app.Key})");
                tsi.Tag = app.Key;
                tsi.Click += MenuItemClick;
                bool? muted = ApplicationMuter.GetApplicationMute(app.Key);
                if (muted.HasValue)
                {
                    tsi.Checked = muted.Value;
                    lpitem.DropDownItems.Add(tsi);
                };
                float? value = ApplicationMuter.GetApplicationVolume(app.Key);
                if (value.HasValue)
                    tsi.Text += $" - {value.Value:0}%";
            };
            lpitem.Text = $"List Processes [{lpitem.DropDownItems.Count}]";
        }

        private void VMMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
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
    }
}

