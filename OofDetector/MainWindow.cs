using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Configuration;

namespace OofDetector
{
    public partial class Main : Form
    {
        private static string HighlightPath;
        private static string TarkovConfigPath;
        private static string SelfProcName;
        private static Helpers.Audio Audio;

        public Main()
        {
            InitializeComponent();
            InitContextMenu();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            SelfProcName = "OOFDetector " + new Random().Next(0, 999999999);
            
            HighlightPath = Path.Combine(Path.GetTempPath(), @"Highlights/Escape From Tarkov");
            TarkovConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Escape from Tarkov/shared.ini");
            if (!Helpers.Checks.IsNvidia())
            {
                MessageBox.Show("No Nvidia card found, exiting...");
                Environment.Exit(0);
            }
            try
            {
                Helpers.Checks.PrepHighlightDir(HighlightPath);
                Helpers.Checks.PrepTarkovConfig(TarkovConfigPath);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Something went wrong\n\n" + ex.Message);
                MessageBox.Show("Some features might not work");
            }

            var eftProcListener = new Thread(new ThreadStart(TarkovProcessListener));
            eftProcListener.Start();
            Audio = new Helpers.Audio();
        }
        private void InitContextMenu()
        {
            var menu = new ContextMenu();
            var exitItem = new MenuItem()
            {
                Index = 0,
                Text = "E&xit"
            };
            exitItem.Click += new EventHandler(exitItem_Click);

            menu.MenuItems.Add(exitItem);

            notification.ContextMenu = menu;
        }
        private void exitItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        private void SetInactive()
        {
            this.Invoke(new Action(() => processData.Enabled = false));
            this.Invoke(new Action(() => gameData.Enabled = false));
            this.Invoke(new Action(() => killData.Enabled = false));
            this.Invoke(new Action(() => this.Text = "(inactive) " + SelfProcName));
            notification.Text = "(inactive) OOFDetector";
        }
        private void SetActive()
        {
            this.Invoke(new Action(() => processData.Enabled = true));
            this.Invoke(new Action(() => gameData.Enabled = true));
            this.Invoke(new Action(() => killData.Enabled = true));
            this.Invoke(new Action(() => this.Text = "(active) " + SelfProcName));
            notification.Text = "(active) OOFDetector";
        }
        private void ShowForm()
        {
            this.Invoke(new Action(() => this.Show()));
            this.Invoke(new Action(() => this.WindowState = FormWindowState.Normal));
            notification.Visible = false;
        }
        private void HideForm()
        {
            notification.Visible = true;
            this.Invoke(new Action(() => this.Hide()));
        }

        private void TarkovProcessListener()
        {
            bool isRunning = false;
            var activityListenerThread = new Thread(new ThreadStart(HighlightActivityListener));
            while (true)
            {
                var proc = Process.GetProcessesByName(ConfigurationSettings.AppSettings["eftProcName"]);
                if (proc.Length > 0)
                {
                    if (!isRunning)
                    {
                        isRunning = true;
                        SetActive();
                        Audio.PlayAudio(Helpers.AudioSelect.WAKEUP);
                        activityListenerThread = new Thread(new ThreadStart(HighlightActivityListener));
                        activityListenerThread.Start();
                        processData.Invoke(new Action(() => processData.Items.Add(String.Format("{0} - Tarkov instance opened, starting listener", new DateTime().ToString("hh:mm:ss")))));
                    }
                }
                else
                {
                    if (isRunning)
                    {
                        processData.Invoke(new Action(() => processData.Items.Add(String.Format("{0} - Tarkov instance closed, stopping listener", new DateTime().ToString("hh:mm:ss")))));
                        activityListenerThread.Abort();
                        SetInactive();
                        HideForm();
                    }
                    isRunning = false;
                }
                Thread.Sleep(5000);
            }
        }

        private void HighlightActivityListener()
        {
            int actCount = 0;
            while (true)
            {
                var files = Directory.GetFiles(HighlightPath);
                if (files.Length > actCount)
                {
                    Audio.PlayAudio(Helpers.AudioSelect.OOF);
                    killData.Invoke(new Action(() => killData.Items.Add(String.Format("{0} - {1} kills", DateTime.Now.ToString("hh:mm:ss"), files.Length))));
                }
                else if (files.Length == 0 && actCount != 0)
                {
                    killData.Invoke(new Action(() => killData.Items.Clear()));
                    gameData.Invoke(new Action(() => gameData.Items.Add(String.Format("{0} - Game finished, recorded {1} activities", DateTime.Now.ToString("hh:mm:ss"), actCount))));
                }
                actCount = files.Length;
                Thread.Sleep(750);
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            HideForm();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowForm();
        }

        private void Main_Shown(object sender, EventArgs e)
        {
            SetInactive();
            HideForm();
        }
    }
}
