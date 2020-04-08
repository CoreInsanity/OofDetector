using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using System.Linq;
using System.Text;

namespace OofDetector
{
    public partial class Main : Form
    {
        private static string NvidiaSDKPath;
        private static string HighlightPath;
        private static string TarkovConfigPath;
        private static string SelfProcName;
        private static Helpers.Audio Audio;
        private static FileSystemWatcher LogWatcher;
        private static int SessionKills;

        public Main()
        {
            InitializeComponent();
            InitContextMenu();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            SelfProcName = "OOFDetector " + new Random().Next(0, 999999999);
            SessionKills = 0;
            HighlightPath = Path.Combine(Path.GetTempPath(), @"Highlights/Escape From Tarkov");
            TarkovConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Escape from Tarkov/shared.ini");
            NvidiaSDKPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "nvidia corporation/gfesdk");

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
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong\n\n" + ex.Message);
                MessageBox.Show("Some features might not work");
            }

            Audio = new Helpers.Audio();

            var eftProcListener = new Thread(new ThreadStart(TarkovProcessListener));
            eftProcListener.Start();
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
                        processData.Invoke(new Action(() => processData.Items.Add(String.Format("{0} - Tarkov instance opened, starting listener", DateTime.Now.ToString("hh:mm:ss")))));
                        isRunning = true;
                        SetActive();
                        Audio.PlayAudio(Helpers.AudioSelect.WAKEUP);
                        activityListenerThread.Start();
                    }
                }
                else
                {
                    if (isRunning)
                    {
                        processData.Invoke(new Action(() => processData.Items.Add(String.Format("{0} - Tarkov instance closed, stopping listener", DateTime.Now.ToString("hh:mm:ss")))));
                        LogWatcher.EnableRaisingEvents = false;
                        SessionKills = 0;
                        LogWatcher = new FileSystemWatcher();
                        activityListenerThread.Abort();
                        SetInactive();
                        HideForm();
                    }
                    isRunning = false;
                }
                Thread.Sleep(10000);
            }
        }

        private void HighlightActivityListener()
        {
            Thread.Sleep(25000);

            FileInfo[] files = new DirectoryInfo(NvidiaSDKPath).GetFiles("*.log");
            var log = files.Where(f => f.Name.ToLower().Contains("tarkov")).OrderByDescending(f => f.LastWriteTime).First();

            processData.Invoke(new Action(() => processData.Items.Add(String.Format("{0} - Listening for activities in {1}", DateTime.Now.ToString("hh:mm:ss"), log.Name))));

            long lastKnownFileSize = 0;
            while (true)
            {
                try
                {
                    var curFileSize = new FileInfo(log.FullName).Length;
                    if (curFileSize <= lastKnownFileSize)
                    {
                        Thread.Sleep(400);
                        continue;
                    }
                    lastKnownFileSize = curFileSize;

                    var logContents = new List<string>();
                    using (var fs = new FileStream(log.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var sr = new StreamReader(fs, Encoding.Default))
                    {
                        logContents = sr.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
                        sr.Close();
                    }
                    var killsInLog = logContents.Where(l => l.ToLower().Contains("posted request hlgetusersettings")).ToList().Count;

                    if (killsInLog > SessionKills)
                    {
                        //Ladies and gentlemen, we got em
                        new Helpers.Audio().PlayAudio(Helpers.AudioSelect.OOF);
                        killData.Invoke(new Action(() => killData.Items.Add(String.Format("{0} - Kill", DateTime.Now.ToString("hh:mm:ss")))));
                    }
                    SessionKills = killsInLog;
                }
                catch(Exception ex)
                {
                    gameData.Invoke(new Action(() => gameData.Items.Add(ex.Message)));
                    Thread.Sleep(500);
                }
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
