namespace OofDetector
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.processData = new System.Windows.Forms.ListBox();
            this.gameData = new System.Windows.Forms.ListBox();
            this.killData = new System.Windows.Forms.ListBox();
            this.notification = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // processData
            // 
            this.processData.FormattingEnabled = true;
            this.processData.Location = new System.Drawing.Point(12, 12);
            this.processData.Name = "processData";
            this.processData.Size = new System.Drawing.Size(338, 381);
            this.processData.TabIndex = 0;
            // 
            // gameData
            // 
            this.gameData.FormattingEnabled = true;
            this.gameData.Location = new System.Drawing.Point(380, 12);
            this.gameData.Name = "gameData";
            this.gameData.Size = new System.Drawing.Size(338, 381);
            this.gameData.TabIndex = 1;
            // 
            // killData
            // 
            this.killData.FormattingEnabled = true;
            this.killData.Location = new System.Drawing.Point(740, 12);
            this.killData.Name = "killData";
            this.killData.Size = new System.Drawing.Size(338, 381);
            this.killData.TabIndex = 2;
            // 
            // notification
            // 
            this.notification.Icon = ((System.Drawing.Icon)(resources.GetObject("notification.Icon")));
            this.notification.Text = "notifyIcon1";
            this.notification.Visible = true;
            this.notification.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1094, 403);
            this.Controls.Add(this.killData);
            this.Controls.Add(this.gameData);
            this.Controls.Add(this.processData);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OofDetector";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.Shown += new System.EventHandler(this.Main_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox processData;
        private System.Windows.Forms.ListBox gameData;
        private System.Windows.Forms.ListBox killData;
        private System.Windows.Forms.NotifyIcon notification;
    }
}

