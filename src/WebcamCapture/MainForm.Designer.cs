using WebcamCapture.Plugin;

namespace WebcamCapture
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeCameraFormatMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.devicesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commandsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelVideo = new System.Windows.Forms.Panel();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusCurrentFormat = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusVideoWindowSize = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusFPS = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainMenuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.devicesToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.commandsToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(550, 24);
            this.mainMenuStrip.TabIndex = 1;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeCameraFormatMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // changeCameraFormatMenuItem
            // 
            this.changeCameraFormatMenuItem.Name = "changeCameraFormatMenuItem";
            this.changeCameraFormatMenuItem.Size = new System.Drawing.Size(200, 22);
            this.changeCameraFormatMenuItem.Text = "Change Camera Format";
            this.changeCameraFormatMenuItem.Click += new System.EventHandler(this.ChangeCameraFormatClick);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitClick);
            // 
            // devicesToolStripMenuItem
            // 
            this.devicesToolStripMenuItem.Name = "devicesToolStripMenuItem";
            this.devicesToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.devicesToolStripMenuItem.Text = "Devices";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // commandsToolStripMenuItem
            // 
            this.commandsToolStripMenuItem.Name = "commandsToolStripMenuItem";
            this.commandsToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
            this.commandsToolStripMenuItem.Text = "Commands";
            // 
            // panelVideo
            // 
            this.panelVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelVideo.Location = new System.Drawing.Point(13, 28);
            this.panelVideo.Name = "panelVideo";
            this.panelVideo.Size = new System.Drawing.Size(400, 292);
            this.panelVideo.TabIndex = 2;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusCurrentFormat,
            this.statusVideoWindowSize,
            this.statusFPS});
            this.statusStrip.Location = new System.Drawing.Point(0, 369);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(550, 22);
            this.statusStrip.TabIndex = 3;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusCurrentFormat
            // 
            this.statusCurrentFormat.Name = "statusCurrentFormat";
            this.statusCurrentFormat.Size = new System.Drawing.Size(118, 17);
            this.statusCurrentFormat.Text = "toolStripStatusLabel1";
            // 
            // statusVideoWindowSize
            // 
            this.statusVideoWindowSize.Name = "statusVideoWindowSize";
            this.statusVideoWindowSize.Size = new System.Drawing.Size(118, 17);
            this.statusVideoWindowSize.Text = "toolStripStatusLabel1";
            // 
            // statusFPS
            // 
            this.statusFPS.Name = "statusFPS";
            this.statusFPS.Size = new System.Drawing.Size(118, 17);
            this.statusFPS.Text = "toolStripStatusLabel1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 391);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.panelVideo);
            this.Controls.Add(this.mainMenuStrip);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem devicesToolStripMenuItem;
        private System.Windows.Forms.Panel panelVideo;
        private System.Windows.Forms.ToolStripMenuItem changeCameraFormatMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusCurrentFormat;
        private System.Windows.Forms.ToolStripStatusLabel statusVideoWindowSize;
        private System.Windows.Forms.ToolStripStatusLabel statusFPS;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem commandsToolStripMenuItem;
    }
}