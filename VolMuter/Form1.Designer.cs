namespace VolMuter
{
    partial class VolForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VolForm));
            this.VolNotifier = new System.Windows.Forms.NotifyIcon(this.components);
            this.VMMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.activeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lwitem = new System.Windows.Forms.ToolStripMenuItem();
            this.lpitem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.volMuterByDkxceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.VMMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // VolNotifier
            // 
            this.VolNotifier.ContextMenuStrip = this.VMMenu;
            this.VolNotifier.Icon = ((System.Drawing.Icon)(resources.GetObject("VolNotifier.Icon")));
            this.VolNotifier.Text = "VolMuter (by dkxce)";
            this.VolNotifier.Visible = true;
            this.VolNotifier.Click += new System.EventHandler(this.VolNotifier_Click);
            // 
            // VMMenu
            // 
            this.VMMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.activeToolStripMenuItem,
            this.lwitem,
            this.lpitem,
            this.toolStripMenuItem1,
            this.volMuterByDkxceToolStripMenuItem,
            this.toolStripMenuItem3,
            this.exitToolStripMenuItem});
            this.VMMenu.Name = "VMMenu";
            this.VMMenu.Size = new System.Drawing.Size(173, 126);
            this.VMMenu.Opening += new System.ComponentModel.CancelEventHandler(this.VMMenu_Opening);
            // 
            // activeToolStripMenuItem
            // 
            this.activeToolStripMenuItem.Name = "activeToolStripMenuItem";
            this.activeToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.activeToolStripMenuItem.Text = "active";
            // 
            // lwitem
            // 
            this.lwitem.Name = "lwitem";
            this.lwitem.Size = new System.Drawing.Size(172, 22);
            this.lwitem.Text = "List Windows";
            // 
            // lpitem
            // 
            this.lpitem.Name = "lpitem";
            this.lpitem.Size = new System.Drawing.Size(172, 22);
            this.lpitem.Text = "List Processes";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(169, 6);
            // 
            // volMuterByDkxceToolStripMenuItem
            // 
            this.volMuterByDkxceToolStripMenuItem.Name = "volMuterByDkxceToolStripMenuItem";
            this.volMuterByDkxceToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.volMuterByDkxceToolStripMenuItem.Text = "VolMuter by dkxce";
            this.volMuterByDkxceToolStripMenuItem.Click += new System.EventHandler(this.volMuterByDkxceToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(169, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // VolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(245, 76);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VolForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VolMuter";
            this.VMMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon VolNotifier;
        private System.Windows.Forms.ContextMenuStrip VMMenu;
        private System.Windows.Forms.ToolStripMenuItem volMuterByDkxceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem activeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem lwitem;
        private System.Windows.Forms.ToolStripMenuItem lpitem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
    }
}

