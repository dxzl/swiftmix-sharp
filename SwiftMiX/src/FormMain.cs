using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics; // "Process" needs this...
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AxWMPLib;
using TorboSS;
using MediaTags;
using Microsoft.Win32;
using System.Net;

namespace SwiftMiX
{
    #region Form1Class

    public partial class FormMain : DockableForm
    {
        #region Constants

//        internal const bool FREEWARE = true; // Set false to add the license-key code

        internal const string TRIAL_EMAIL = "SwiftMiX@trial.com";

        // FormDirictories default size
        internal const int FDSIZE_X = 873;
        internal const int FDSIZE_Y = 417;

        // This is now stored in "Settings"
        //internal const string TRIAL_KEY = "X47YYH90S957M2Q9DE"; // Unlimited, 02/08/2013 
        // ForceKey/PaidKey/ExpireKey
        // not set.

        // No changes, just rebuilt for VS2012 Express on Windows 7 and packaged with NSIS
        // Don't forget to change version in Properties->Assembly Information also!
        internal const string REVISION = "1.77"; // Released 12/14/2019

        internal const string HELPSITE = "http://www.yahcolorize.com/swiftmix/help/help2.htm";
        internal const string WEBSITE = "http://www.yahcolorize.com/swiftmix/";
        internal const string SWIFTMIX = @"\SwiftMiX";
        internal const string SWIFTMIX2 = "SwiftMiX";
        internal const string SHORTCUT_FILE = @"SwiftMiX.lnk";
        internal const string EXE_FILE = @"SwiftMiX.exe";
        internal const string REGISTRY_KEY = @"SOFTWARE\Discrete-Time Systems\SwiftMiX";
        internal const string UNINSTALL_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        internal const string PROG_ID = "SwiftMiX.1.XX";
        //internal const string GUID = "097e38a7-a1d9-4ff0-a877-ddab9966618f";  

        internal const int MAX_PATH = 260;

        // initial View fields visible in FormDirectories songView
        internal const int FIELD_VISIBLE = 4623;

        // Product ID is unique to each product from DTS
        // Super Rev. is bumped up (hidden from user) to draw a
        // line between possibly incompatable sub-versions
        // of the Product. So THIS program will run ONLY with
        // a key with the proper Product ID and Super-Rev #.
        internal const int DEF_PRODUCT_ID = 1; // Product ID...
        internal const int DEF_SUPER_REV = 2; // Super-Revision...

        internal const int RED_STATUS_TIME = 15; // turn red at 15 sec.
        internal const int YELLOW_STATUS_TIME = 30; // turn yellow at 30 sec.
        internal const int NEXT_SONG_TIME = 2; // go to next song at 2 sec.

        internal const int LOCATION_TOP = -1; // open in center
        internal const int LOCATION_LEFT = -1; // open in center
        internal const int WMP_HEIGHT = 44; // player A/B height

        private const int AUTOFADE_TIMER_INTERVAL = 50; // 100 steps at 50ms/step
        private const int TRACKBAR_MAXIMUM = 100; // 100 steps
        private const int TRACKBAR_MINIMUM = 0;

        private const int NUMERIC_FADESPEED_MAXIMUM = 30; // 30 seconds
        private const int NUMERIC_FADESPEED_MINIMUM = 0; // 50 miliseconds
        private const int NUMERIC_FADESPEED = 5;

        private const int NUMERIC_FADEPOINT_MAXIMUM = +240;
        private const int NUMERIC_FADEPOINT_MINIMUM = -240;
        private const int NUMERIC_FADEPOINT = 10;

        private const int INITIAL_VOLUME = 50; // 50%

        #endregion

        #region Variables

        // Private
        private string filesDirA, filesDirB;
        private bool bNormalFade = true; // user-set fader center-fade mode
        private int yellowStatusTime, redStatusTime;
        private int RWM_SwiftMixPlay = 0;
        private int RWM_SwiftMixTime = 0;
        private int RWM_SwiftMixState = 0;

        // Registry globals
        private System.Drawing.Point g_pLocation;
        private int g_iFadeSpeed, g_iFadePoint, g_iVolumeA, g_iVolumeB, g_fieldVisible;
        private bool g_bAutoFade, g_bNormalFade, g_bSendTelemetry, g_bRootFolderHasBeenSet;
        private string g_sLicenseKey, g_sRootFolder, g_sFileFilters;
        private Size g_formDirectoriesSize;

        // Public
        //public KeyClass pk = null;

        #endregion

        #region Properties

        // Read-Only

        public string Version
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        public Environment.SpecialFolder DefaultSpecialFolder
        {
            get { return Environment.SpecialFolder.MyMusic; }
        }

        public string DefaultFolderPath
        {
            get { return (string)Environment.GetFolderPath(DefaultSpecialFolder); }
        }

        public int StartFadeRemainingTime
        {
            get { return (int)this.numericFadePoint.Value; }
        }

        public int TrackBarMax
        {
            get { return this.trackBar1.Maximum; }
        }

        public int TrackBarValue
        {
            get { return this.trackBar1.Value; }
        }

        public bool SendTelemetry
        {
            get { return this.g_bSendTelemetry; }
        }

        public bool AutoFadePlayerLists
        {
            get { return this.g_bAutoFade; }
        }

        public int VolA
        {
            get { return this.g_iVolumeA; }
        }

        public int VolB
        {
            get { return this.g_iVolumeB; }
        }

        // Used to encourage user to select a folder
        public bool RootFolderHasBeenSet
        {
            get { return this.g_bRootFolderHasBeenSet; }
            set { this.g_bRootFolderHasBeenSet = value; }
        }

        // Used to encourage user to select a folder
        public Size FormDirectoriesSize
        {
            get { return g_formDirectoriesSize; }
            set { g_formDirectoriesSize = value; }
        }

        public string RootFolder
        {
            get { return this.g_sRootFolder; }
            set { this.g_sRootFolder = value; }
        }

        public int FieldVisible // Calling Form should poll this...
        {
            get { return g_fieldVisible; }
            set { g_fieldVisible = value; }
        }

        public string LicenseKey
        {
            get { return this.g_sLicenseKey; }
        }

        // Make public references to Form1's designer-objects
        private FormPlaylist listA = null;
        public FormPlaylist ListA
        {
            get { return this.listA; }
        }

        private FormPlaylist listB = null;
        public FormPlaylist ListB
        {
            get { return this.listB; }
        }

        // Read/Write
        public int ProgressBarValue
        {
            get { return this.progressBar1.Value; }

            set
            {
                if (value < 0)
                    this.progressBar1.Hide();
                else
                {
                    this.progressBar1.Value = value;

                    if (!this.progressBar1.Visible)
                        this.progressBar1.Show();
                }
            }
        }

        public bool AutoFadeTimerEnabled
        {
            get { return this.AutoFadeTimer.Enabled; }
            set { this.AutoFadeTimer.Enabled = value; }
        }

        // Here the calling program sets the file-filters
        // such as .mp3|.wma|.wav
        private List<string> fileFilterList;
        public List<string> FileFilterList
        {
            get { return this.fileFilterList; }
            set { this.fileFilterList = value; }
        }

        private FormDirectories browserDialog = null;
        public FormDirectories BrowserDialog
        {
            get { return this.browserDialog; }
            set { this.browserDialog = value; }
        }

        private bool bFadeRight = false;
        public bool FadeRight
        {
            get { return this.bFadeRight; }
            set { this.bFadeRight = value; }
        }

        // Used by the FileList class...
        private int keyPressed = -1; // Will hold a key value if a key is pressed
        public int KeyPressed
        {
            get { return (int)this.keyPressed; }
            set { this.keyPressed = value; }
        }
        #endregion

        #region Form Event Handlers

        public FormMain()
        {
            // Sets the UI culture to english-usa
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            // Sets the culture to French (France)
            //Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");

            // Sets the UI culture to French (France)
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");

            //To Generate a new GUID for our entry in add/remove
            //programs in CreateUninstaller
            //fb4f3a2c-6ad5-4e61-a3d8-d1d46bf2db2b
            //Clipboard.SetText(Guid.NewGuid().ToString());

            InitializeComponent();
        }
        //---------------------------------------------------------------------------
        void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // Create main play-lists
                listA = new FormPlaylist(this, axWindowsMP1, "Player A");
                listB = new FormPlaylist(this, axWindowsMP2, "Player B");

                RegistryRead(); // Do this after lists created and before volumes set...

                // Only files we want to see in the song-lists...
                listA.FileFilterList = this.fileFilterList;
                listB.FileFilterList = this.fileFilterList;

                // Test that we have two valid instances of WMP (if not it throws an exception)
                if (!SetVolumes())
                {
                    MessageBox.Show("Re-Install Windows Media Player 10 or above!");
                    Application.ExitThread();
                }

                // Create License Key vars and methods
                //pk = new KeyClass();

                // Using Torbo.DockableForm.dll
                // 1) Added it to the project (move dll to project
                //    directory, right-click Torbo.DockableForm.dll
                //    and click "Include In Project" select "Copy Always"
                // 2) Created a reference (Project->Add Reference)
                // 3) Added "Using Torbo" (in Form1 and Form2)
                // 4) ConnectForm() is a method
                // 5) Need to derrive Form1 and Form2 from "DockableForm"
                //this.DockingGap = 10;
                ConnectForm(listA);
                ConnectForm(listB);
                listA.ConnectForm(listB); // Lists will dock to one-another...

                filesDirA = this.DefaultFolderPath;
                filesDirB = this.DefaultFolderPath;

                Text = SWIFTMIX2 + "® " + REVISION;
            }
            catch
            {
                MessageBox.Show("Re-Install Windows Media Player 10 or above!");
                Application.ExitThread();
            }

            try
            {
                string Info = Convert.ToString(axWindowsMP1.versionInfo);

                int tmp = Info.IndexOf(".");
                string Version = Info.Substring(0, tmp);
                tmp = Convert.ToInt32(Version);
                if (tmp < 10)
                {
                    MessageBox.Show("Sorry, you must get the newest version of\n" +
                      "Microsoft's Windows Media-Player!\n" +
                      "Your version is: " + Info);
                    Application.ExitThread();
                }
            }
            catch
            {
                MessageBox.Show("You must get the new version of\n" +
                    "Microsoft's Windows Media-Player!");
                Application.ExitThread();
            }

            //if (!FREEWARE)
            //{
            //    bool bRet;
            //    LicenseKey lk = new LicenseKey(this);
            //    bRet = lk.ValidateLicenseKey(false);

            //    if (!bRet)
            //    {
            //        MessageBox.Show("Critical error in class: TLicenseKey!");
            //        Application.ExitThread();
            //    }
            //}

            TimeDisplay(0, 0);
            TimeDisplay(0, 1);
            TimeDisplay(0, 3);
            TimeDisplay(0, 4);

            AutoFadeTimer.Interval = AUTOFADE_TIMER_INTERVAL;
            numericFadeSpeed.Minimum = NUMERIC_FADESPEED_MINIMUM;
            numericFadeSpeed.Maximum = NUMERIC_FADESPEED_MAXIMUM;
            trackBar1.Maximum = TRACKBAR_MAXIMUM;

            yellowStatusTime = YELLOW_STATUS_TIME;
            redStatusTime = RED_STATUS_TIME;

            // Register custom messages for the Trinity client
            RWM_SwiftMixPlay = (int)NativeMethods.RegisterWindowMessage("WM_SwiftMixPlay");
            RWM_SwiftMixTime = (int)NativeMethods.RegisterWindowMessage("WM_SwiftMixTime");
            RWM_SwiftMixState = (int)NativeMethods.RegisterWindowMessage("WM_SwiftMixState");

            // Adjust our width based on the media-player
            try
            {
                int w = axWindowsMP1.Width;

                axWindowsMP2.Width = w;

                axWindowsMP1.Left = axWindowsMP2.Left = 0;
                axWindowsMP1.Height = axWindowsMP2.Height = WMP_HEIGHT;

                axWindowsMP1.Top = SystemInformation.ToolWindowCaptionHeight + 2;
                axWindowsMP2.Top = axWindowsMP1.Top + axWindowsMP1.Height + 1;

                panel1.Left = panel2.Left = 0;

                panel1.Top = axWindowsMP2.Top + axWindowsMP2.Height + 1;
                panel2.Top = panel1.Top + panel1.Height + 1;

                panel1.Width = panel2.Width = w;

                this.Width = w + (2 * SystemInformation.BorderSize.Width) + 2;
                this.Height = panel2.Top + panel2.Height + SystemInformation.ToolWindowCaptionHeight + (2 * SystemInformation.BorderSize.Height) + 4;
            }
            catch
            {
                this.Width = 312;
                this.Height = 242;
            }

            // Add SwiftMiX to the Add/Remove programs list
            // Handled by the NSIS installer now
            //try {if (IsNewRevision()) CreateUninstaller();}
            //catch {}

            // Create program-association
            //if (ProgramAssociation(true))
            //{
            //  // Song types
            //  FileAssociation(true, ".mp3");
            //  FileAssociation(true, ".wma");
            //  FileAssociation(true, ".wav");
            //  // Playlists
            //  FileAssociation(true, ".wpl");
            //  FileAssociation(true, ".m3u");
            //  FileAssociation(true, ".m3u8");
            //  FileAssociation(true, ".pls");
            //}
        }
        //---------------------------------------------------------------------------
        void Form1_Shown(object sender, EventArgs e)
        {
            ToolTipsInit();

            // Process command-line option to uninstall
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                if (args[1].ToLower() == "/uninstall")
                {
                    // NSIS does this now
                    //uninstallToolStripMenuItem_Click(null, null);
                }
                else
                {
                    try
                    {
                        // File path that is passed via the "open With" list will be a 
                        // "short" file path so we must convert it...
                        string fileName = Path.GetFullPath(args[1]);

                        if (ListA.clbAdd(fileName))
                        {
                            ListA.Queue(0);
                            ListA.PlayerControlMode = 3; // time-delay start player
                            ListA.PlayerControlTimerEnabled = true;
                        }
                        else // maybe it's a UTF-8 playlist?
                        {
                            ImportClass ic = new ImportClass(this);
                            if (ic.ProcessSongListFile(-1, fileName, ListA, ListB) > 0) // specify "Auto" encoding
                            {
                                if (ListB.clbCount > 0)
                                    ListB.Queue(0);

                                if (ListA.clbCount > 0)
                                {
                                    ListA.Queue(0);
                                    ListA.PlayerControlMode = 3; // time-delay start player
                                    ListA.PlayerControlTimerEnabled = true;
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }
        //---------------------------------------------------------------------------
        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            axWindowsMP1.settings.mute = true;
            axWindowsMP1.Ctlcontrols.stop();
            axWindowsMP2.settings.mute = true;
            axWindowsMP2.Ctlcontrols.stop();

            // Stop Timers
            if (listA != null)
                listA.PlayerPositionTimerEnabled = false;

            if (listB != null)
                listB.PlayerPositionTimerEnabled = false;

            AutoFadeTimer.Enabled = false;

            // Save settings
            RegistryWrite();
        }
        //---------------------------------------------------------------------------
        void RegistryRead(bool bReset = false)
        {
            if (ListA != null && ListB != null)
            {
                if (ListA.IsPlayOrPause() || ListB.IsPlayOrPause())
                {
                    DialogResult result1 = MessageBox.Show("Stop player(s) and restore default settings...\nAre you sure?",
                                               "SwiftMiX",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Warning,
                                                MessageBoxDefaultButton.Button2);

                    if (result1 == DialogResult.No) return;
                }

                ListA.clbStop();
                ListB.clbStop();

                if (listA.Visible) listA.Hide();
                if (listB.Visible) listB.Hide();

                ListA.clbClear();
                ListB.clbClear();
            }

            // Disable up/down control event-handlers
            numericFadeSpeed.ValueChanged -= numericFadeSpeed_ValueChanged;
            numericFadePoint.ValueChanged -= numericFadePoint_ValueChanged;

            AutoFadeTimer.Enabled = false;
            AutoFadeTimer.Interval = AUTOFADE_TIMER_INTERVAL;

            numericFadeSpeed.Value = NUMERIC_FADESPEED;
            numericFadePoint.Value = NUMERIC_FADEPOINT;

            trackBar1.Value = TRACKBAR_MINIMUM;

            yellowStatusTime = YELLOW_STATUS_TIME;
            redStatusTime = RED_STATUS_TIME;

            filesDirA = this.DefaultFolderPath;
            filesDirB = this.DefaultFolderPath;

            TimeDisplay(0, 0);
            TimeDisplay(0, 1);
            TimeDisplay(0, 3);
            TimeDisplay(0, 4);

            if (browserDialog != null)
                browserDialog.Dispose();

            // Registry item temp vars...
            Point pLocation = new System.Drawing.Point(-1, -1);
            int iFadeSpeed = NUMERIC_FADESPEED;
            int iFadePoint = NUMERIC_FADEPOINT;
            int iVolumeA = INITIAL_VOLUME;
            int iVolumeB = INITIAL_VOLUME;
            int iFieldVisible = FIELD_VISIBLE;
            bool bRandomA = false;
            bool bRandomB = false;
            bool bRepeatA = false;
            bool bRepeatB = false;
            bool bAutoFade = true;
            bool bNormalFade = true;
            bool bSendTelemetry = false;
            bool bRootFolderHasBeenSet = false;
            string sLicenseKey = "XI7YYH90S9LRM2Q9DE";
            string sRootFolder = "";
            string sFileFilters = ".wma .mp3 .wav .aif .aiff";
            Size pFormDirectoriesSize = new Size(FDSIZE_X, FDSIZE_Y);

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY, false))
            {
                if (key != null)
                {
                    try
                    {
                        // If installing an older rev than is in the registry, keep the default values (reset)
                        string sRevision = (string)key.GetValue("sRevision");
                        if (Convert.ToDouble(REVISION) < Convert.ToDouble(sRevision))
                            bReset = true;
                    }
                    catch { bReset = true; }

                    if (!bReset)
                    {
                        try
                        {
                            iFadeSpeed = (int)key.GetValue("iFadeSpeed", iFadeSpeed);
                            iFadePoint = (int)key.GetValue("iFadePoint", iFadePoint);
                            iVolumeA = (int)key.GetValue("iVolumeA", iVolumeA);
                            iVolumeB = (int)key.GetValue("iVolumeB", iVolumeB);
                            sLicenseKey = (string)key.GetValue("sLicenseKey", sLicenseKey);
                            sRootFolder = (string)key.GetValue("sRootFolder", sRootFolder);
                            sFileFilters = (string)key.GetValue("sFileFilters", sFileFilters);
                            pLocation.X = (int)key.GetValue("pLocationX", pLocation.X);
                            pLocation.Y = (int)key.GetValue("pLocationY", pLocation.Y);
                            bRandomA = Convert.ToBoolean(key.GetValue("bRandomA", bRandomA));
                            bRandomB = Convert.ToBoolean(key.GetValue("bRandomB", bRandomB));
                            bRepeatA = Convert.ToBoolean(key.GetValue("bRepeatA", bRepeatA));
                            bRepeatB = Convert.ToBoolean(key.GetValue("bRepeatB", bRepeatB));
                            bAutoFade = Convert.ToBoolean(key.GetValue("bAutoFade", bAutoFade));
                            bNormalFade = Convert.ToBoolean(key.GetValue("bNormalFade", bNormalFade));
                            bSendTelemetry = Convert.ToBoolean(key.GetValue("bSendTelemetry", bSendTelemetry));
                            bRootFolderHasBeenSet = Convert.ToBoolean(key.GetValue("bRootFolderHasBeenSet", bRootFolderHasBeenSet));
                            iFieldVisible = (int)key.GetValue("iFieldVisible", iFieldVisible);
                            pFormDirectoriesSize.Width = (int)key.GetValue("iFDSize_w", pFormDirectoriesSize.Width);
                            pFormDirectoriesSize.Height = (int)key.GetValue("iFDSize_h", pFormDirectoriesSize.Height);
                        }
                        catch { }
                    }
                }
            }

            // Set location
            g_pLocation = pLocation;
            if (g_pLocation.X < 0 || g_pLocation.Y < 0)
            {
                // Set to center-screen
                int screenH = (Screen.PrimaryScreen.WorkingArea.Top + Screen.PrimaryScreen.WorkingArea.Height) / 2;
                int screenW = (Screen.PrimaryScreen.WorkingArea.Left + Screen.PrimaryScreen.WorkingArea.Width) / 2;
                int top = screenH - (Width / 2);
                int left = screenW - (Height / 2);
                Location = new Point(left, top);
            }
            else
                Location = g_pLocation;

            g_fieldVisible = iFieldVisible;
            g_formDirectoriesSize = pFormDirectoriesSize;

            g_iFadeSpeed = iFadeSpeed;
            numericFadeSpeed.Value = g_iFadeSpeed;
            AutoFadeTimer.Interval = g_iFadeSpeed * 1000 / TRACKBAR_MAXIMUM;

            g_iFadePoint = iFadePoint;
            numericFadePoint.Value = g_iFadePoint;

            g_iVolumeA = iVolumeA;
            SetVolumeMenuFromValueA();
            g_iVolumeB = iVolumeB;
            SetVolumeMenuFromValueB();

            ListA.RandomMode = bRandomA;
            ListA.RandomMode = !ListA.RandomMode;
            randomToolStripMenuItemA_Click(null, null);

            ListB.RandomMode = bRandomB;
            ListB.RandomMode = !ListB.RandomMode;
            randomToolStripMenuItemB_Click(null, null);

            ListA.RepeatMode = bRepeatA;
            ListA.RepeatMode = !ListA.RepeatMode;
            repeatToolStripMenuItemA_Click(null, null);

            ListB.RepeatMode = bRepeatB;
            ListB.RepeatMode = !ListB.RepeatMode;
            repeatToolStripMenuItemB_Click(null, null);

            g_bAutoFade = bAutoFade;
            g_bAutoFade = !g_bAutoFade;
            faderModeAutoToolStripMenuItem_Click(null, null);

            g_bNormalFade = bNormalFade;
            g_bNormalFade = !g_bNormalFade;
            faderTypeNormalToolStripMenuItem_Click(null, null);

            g_bSendTelemetry = bSendTelemetry;
            g_bSendTelemetry = !g_bSendTelemetry;
            SendTelemetryToolStripMenuItem_Click(null, null);

            g_bRootFolderHasBeenSet = bRootFolderHasBeenSet;
            g_sLicenseKey = sLicenseKey;

            g_sRootFolder = sRootFolder;
            if (String.IsNullOrEmpty(g_sRootFolder)) // Clean install or factory reset...
                g_sRootFolder = this.DefaultFolderPath;

            g_sFileFilters = sFileFilters;
            fileFilterList = EncodeFilters(g_sFileFilters); // create and assign the song filter list object (.mp3 etc)...

            // Other things to init when user restores defaults
            filesDirA = this.DefaultFolderPath;
            filesDirB = this.DefaultFolderPath;
            Text = SWIFTMIX2 + "® " + REVISION;

            // Enable up/down control event-handlers
            numericFadeSpeed.ValueChanged += numericFadeSpeed_ValueChanged;
            numericFadePoint.ValueChanged += numericFadePoint_ValueChanged;
        }
        //---------------------------------------------------------------------------
        private bool RegistryWrite()
        {
            RegistryKey key;

            using (key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY, true))
            {
                if ((key = Registry.CurrentUser.CreateSubKey(REGISTRY_KEY)) == null)
                    return false;

                try
                {
                    key.SetValue("iFadeSpeed", g_iFadeSpeed);
                    key.SetValue("iFadePoint", g_iFadePoint);
                    key.SetValue("iVolumeA", g_iVolumeA);
                    key.SetValue("iVolumeB", g_iVolumeB);
                    key.SetValue("sLicenseKey", g_sLicenseKey);
                    key.SetValue("sRootFolder", g_sRootFolder);
                    key.SetValue("sFileFilters", g_sRootFolder);
                    key.SetValue("sFileFilters", DecodeFilters(fileFilterList));
                    key.SetValue("pLocationX", (int)this.Location.X);
                    key.SetValue("pLocationY", (int)this.Location.Y);
                    key.SetValue("bRandomA", ListA.RandomMode);
                    key.SetValue("bRandomB", ListB.RandomMode);
                    key.SetValue("bRepeatA", ListA.RepeatMode);
                    key.SetValue("bRepeatB", ListB.RepeatMode);
                    key.SetValue("bAutoFade", g_bAutoFade);
                    key.SetValue("bNormalFade", g_bNormalFade);
                    key.SetValue("bSendTelemetry", g_bSendTelemetry);
                    key.SetValue("bRootFolderHasBeenSet", g_bRootFolderHasBeenSet);
                    key.SetValue("iFieldVisible", g_fieldVisible);
                    key.SetValue("iFDSize_w", g_formDirectoriesSize.Width);
                    key.SetValue("iFDSize_h", g_formDirectoriesSize.Height);
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
        //---------------------------------------------------------------------------
        private string DecodeFilters(List<string> fileFilterList)
        {
            string s = "";

            try
            {
                foreach (string filter in fileFilterList)
                    s += filter + ' ';

                if (s.Length > 0)
                    s = s.Substring(0, s.Length - 1);
            }
            catch
            {
                s = ".wma .mp3 .wav .aif .aiff";
            }

            return s;
        }
        //---------------------------------------------------------------------------
        private List<string> EncodeFilters(string textFilterList)
        {
            List<string> fileFilterList = new List<string>();

            try
            {
                string[] split = textFilterList.Split(' ');
                fileFilterList.AddRange(split);
            }
            catch
            {
                fileFilterList.AddRange(new string[] { ".wma", ".mp3", ".wav", ".aif", ".aiff" });
            }

            return fileFilterList;
        }
        //---------------------------------------------------------------------------
        #endregion

        #region ToolTips

        void ToolTipsInit()
        {
            toolTips.BackColor = Color.FromArgb(184, 207, 245);
            toolTips.ShowAlways = true;

            // Set ToolTips
            toolTips.SetToolTip(numericFadeSpeed,
              "Scroll up/down to set the fade-time from one" + Environment.NewLine +
              "song to the next (in seconds).");

            toolTips.SetToolTip(numericFadePoint,
              "Scroll up/down to set how many seconds before" + Environment.NewLine +
              "the end of the song to begin a fadeout.");

            // These tips are to show song-info
            toolTips.SetToolTip(axWindowsMP1, "");
            toolTips.SetToolTip(axWindowsMP2, "");
        }
        //---------------------------------------------------------------------------
        #endregion

        #region Media Player Event Handlers and Methods

        void axWindowsMP1_OpenStateChange(object sender, _WMPOCXEvents_OpenStateChangeEvent e)
        {
            OpenChange(ref listA, (AxWindowsMediaPlayer)sender);
        }
        //---------------------------------------------------------------------------
        void axWindowsMP2_OpenStateChange(object sender, _WMPOCXEvents_OpenStateChangeEvent e)
        {
            OpenChange(ref listB, (AxWindowsMediaPlayer)sender);
        }
        //---------------------------------------------------------------------------
        void OpenChange(ref FormPlaylist f, AxWindowsMediaPlayer mp)
        {
            //typedef enum WMPOpenState { 
            //  wmposUndefined                = 0,
            //  wmposPlaylistChanging         = 1,
            //  wmposPlaylistLocating         = 2,
            //  wmposPlaylistConnecting       = 3,
            //  wmposPlaylistLoading          = 4,
            //  wmposPlaylistOpening          = 5,
            //  wmposPlaylistOpenNoMedia      = 6,
            //  wmposPlaylistChanged          = 7,
            //  wmposMediaChanging            = 8,
            //  wmposMediaLocating            = 9,
            //  wmposMediaConnecting          = 10,
            //  wmposMediaLoading             = 11,
            //  wmposMediaOpening             = 12,
            //  wmposMediaOpen                = 13,
            //  wmposBeginCodecAcquisition    = 14,
            //  wmposEndCodecAcquisition      = 15,
            //  wmposBeginLicenseAcquisition  = 16,
            //  wmposEndLicenseAcquisition    = 17,
            //  wmposBeginIndividualization   = 18,
            //  wmposEndIndividualization     = 19,
            //  wmposMediaWaiting             = 20,
            //  wmposOpeningUnknownURL        = 21
            //} WMPOpenState;

            if (f == null)
                return;

            try
            {
                //MessageBox.Show("openState:" + mp.openState.ToString());
                f.OpenState = mp.openState;

                if (f.OpenState == WMPLib.WMPOpenState.wmposMediaOpen)
                {
                    f.Duration = (int)mp.currentMedia.duration;

                    try
                    {
                        // Player Duration needs posting
                        if (f == listA)
                        {
                            TimeDisplay(f.Duration, 1);
                            SetStatusColorA(Color.FromArgb(184, 207, 245));
                        }
                        else
                        {
                            TimeDisplay(f.Duration, 4);
                            SetStatusColorB(Color.FromArgb(184, 207, 245));
                        }
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
                this.Text = "OpenChange() threw an exception";
            }
        }
        //---------------------------------------------------------------------------
        void axWindowsMP1_PlayStateChange(object sender, _WMPOCXEvents_PlayStateChangeEvent e)
        {
            StateChange(listA, (AxWindowsMediaPlayer)sender);
        }
        //---------------------------------------------------------------------------
        void axWindowsMP2_PlayStateChange(object sender, _WMPOCXEvents_PlayStateChangeEvent e)
        {
            StateChange(listB, (AxWindowsMediaPlayer)sender);
        }
        //---------------------------------------------------------------------------    
        void StateChange(FormPlaylist f, AxWindowsMediaPlayer mp)
        {
            //typedef enum WMPPlayState { 
            //  wmppsUndefined      = 0,
            //  wmppsStopped        = 1,
            //  wmppsPaused         = 2,
            //  wmppsPlaying        = 3,
            //  wmppsScanForward    = 4,
            //  wmppsScanReverse    = 5,
            //  wmppsBuffering      = 6,
            //  wmppsWaiting        = 7,
            //  wmppsMediaEnded     = 8,
            //  wmppsTransitioning  = 9,
            //  wmppsReady          = 10,
            //  wmppsReconnecting   = 11,
            //  wmppsLast           = 12
            //} WMPPlayState;

            if (f == null)
                return;

            bool bPlayerA;

            if (f == listA)
                bPlayerA = true;
            else
                bPlayerA = false;

            // 10=ready/stop
            // 1=stopped
            // 2=paused
            // 3=playing

            f.PlayState = mp.playState;

            bool bStopOrEnded = f.PlayState == WMPLib.WMPPlayState.wmppsStopped ||
                          f.PlayState == WMPLib.WMPPlayState.wmppsMediaEnded ? true : false;

            try
            {
                if (f.PlayState == WMPLib.WMPPlayState.wmppsTransitioning)
                    return;

                if (g_bSendTelemetry)
                {
                    if (f.PlayState == WMPLib.WMPPlayState.wmppsPlaying)
                    {
                        // Populate SwiftMixStruct
                        SWIFTMIX_2 sms = new SWIFTMIX_2();
                        int size = Marshal.SizeOf(typeof(SWIFTMIX_2)); // Fixed-size struct so this is OK!

                        sms.player = (bPlayerA == true) ? 0 : 1;
                        sms.duration = f.Duration;

                        // 4096 is hard-coded into the struct in YahCoLoRiZe, Trinity
                        // and in all versions of SwiftMiX...
                        int MaxCharCount = (4096 / 2) - 2; // Unicode
                        string Path = f.P.URL;
                        if (Path.Length > MaxCharCount)
                            Path = Path.Substring(0, MaxCharCount);
                        sms.path = Path;
                        sms.len_path = sms.path.Length;

                        // 1024 is hard-coded into the struct in YahCoLoRiZe, Trinity
                        // and in all versions of SwiftMiX...
                        MaxCharCount = (1024 / 2) - 2; // Unicode
                        string Name = f.P.currentMedia.name;
                        if (Name.Length > MaxCharCount)
                            Name = Name.Substring(0, MaxCharCount);
                        sms.name = Name;
                        sms.len_name = sms.name.Length;

                        try
                        {
                            string Artist = f.P.currentMedia.getItemInfo("WM/AlbumArtist");
                            string lca = Artist.ToLower();
                            if (string.IsNullOrEmpty(lca) || lca == "various" || lca == "various artists" || lca == "unknown" || lca == "unknown artist")
                                Artist = f.P.currentMedia.getItemInfo("Author");
                            if (Artist.Length > MaxCharCount) Artist = Artist.Substring(0, MaxCharCount);
                            sms.artist = Artist;
                            sms.len_artist = sms.artist.Length;
                        }
                        catch
                        {
                        }

                        try
                        {
                            string Album = f.P.currentMedia.getItemInfo("WM/AlbumTitle");
                            if (Album.Length > MaxCharCount)
                                Album = Album.Substring(0, MaxCharCount);
                            sms.album = Album;
                            sms.len_album = sms.album.Length;
                        }
                        catch
                        {
                        }

                        SendToSwiftMix(sms, size, RWM_SwiftMixPlay);
                    }
                    else
                    {
                        // Send data to YahCoLoRiZe
                        // Populate SwiftMixStruct
                        SWIFTMIX_3 sms = new SWIFTMIX_3();
                        int size = Marshal.SizeOf(typeof(SWIFTMIX_3)); // Fixed-size struct so this is OK!

                        sms.player = (bPlayerA == true) ? 0 : 1;
                        sms.state = (int)mp.playState;

                        SendToSwiftMix(sms, size, RWM_SwiftMixState);
                    }
                }

                if (bPlayerA)
                {
                    stopToolStripMenuItem.Checked = false;
                    playToolStripMenuItem.Checked = false;
                    pauseToolStripMenuItem.Checked = false;
                }
                else
                {
                    stopToolStripMenuItem1.Checked = false;
                    playToolStripMenuItem1.Checked = false;
                    pauseToolStripMenuItem1.Checked = false;
                }

                if (f.PlayState == WMPLib.WMPPlayState.wmppsPaused) // pause?
                {
                    f.PauseSong = true;

                    if (bPlayerA)
                        pauseToolStripMenuItem.Checked = true;
                    else
                        pauseToolStripMenuItem1.Checked = true;
                }
                else
                    f.PauseSong = false;

                if (f.PlayState == WMPLib.WMPPlayState.wmppsPlaying) // play?
                {
                    f.PlaySong = true;

                    if (bPlayerA)
                    {
                        playToolStripMenuItem.Checked = true;
                        SetStatusColorA(Color.SpringGreen);
                    }
                    else
                    {
                        playToolStripMenuItem1.Checked = true;
                        SetStatusColorB(Color.SpringGreen);
                    }

                    if (!InRange(f, f.PlayIndex))
                    {
                        // illegal to start if nothing checked
                        f.PlayerPositionTimerEnabled = false;

                        f.PlayerControlMode = 1; // time-delay stop player
                        f.PlayerControlTimerEnabled = true;

                        return;
                    }

                    // Start player A with player B stopped and fader in wrong position
                    // Autofade to this player...
                    if (g_bAutoFade)
                    {
                        if (bPlayerA)
                        {
                            if (trackBar1.Value != 0) // fade to Player A
                            {
                                bFadeRight = false;
                                AutoFadeTimer.Enabled = true;
                            }
                        }
                        else
                        {
                            if (trackBar1.Value != trackBar1.Maximum) // fade to Player B
                            {
                                bFadeRight = true;
                                AutoFadeTimer.Enabled = true;
                            }
                        }
                    }

                    f.SetCheck(f, f.PlayIndex, CheckState.Checked);

                    f.PlayerPositionTimerEnabled = true;
                    f.Text = GetTitle(f);

                    // Flash list item if list window not focused and playing
                    if (!f.ContainsFocus)
                        f.FlashTimerEnabled = true;
                }
                else
                {
                    f.PlaySong = false;
                    f.PlayerPositionTimerEnabled = false;
                }
            }
            catch
            {
            }

            try
            {
                if (f.PlayState == WMPLib.WMPPlayState.wmppsReady || bStopOrEnded) // ready or stopped?
                {
                    f.StopSong = true;

                    if (bPlayerA)
                    {
                        stopToolStripMenuItem.Checked = true;
                        SetStatusColorA(Color.FromArgb(184, 207, 245));
                    }
                    else
                    {
                        stopToolStripMenuItem1.Checked = true;
                        SetStatusColorB(Color.FromArgb(184, 207, 245));
                    }

                    UpdatePlayerStatus(f);
                }

                if (bStopOrEnded) // stop?
                {
                    // Old listbox checkbox needs to be cleared...
                    if (InRange(f, f.PlayIndex))
                    {
                        if (f.RepeatMode)
                        {
                            f.SetCheck(f, f.PlayIndex, CheckState.Indeterminate);
                            f.clbColor = Color.FromArgb(184, 207, 245);
                        }
                        else
                            f.SetCheck(f, f.PlayIndex, CheckState.Unchecked);
                    }

                    if (f.NextIndex < 0) // idle state if NextIndex is -1
                        f.NextIndex = f.PlayIndex + 1; // start looking "here" + 1

                    string fileName = GetNext(f, false, f.RandomMode);  // Allow random

                    if (InRange(f, f.PlayIndex))
                    {
                        if (SetURL(mp, fileName))
                        {

                            // Play stopped - queue next song
                            if (f.ForceNextPlay)
                            {
                                f.PlayerControlMode = 3; // time-delay start player 1
                                f.PlayerControlTimerEnabled = true;
                            }

                            f.clbSelectedIndex = f.PlayIndex;
                        }
                    }
                    else
                    {
                        f.clbSelectedIndex = -1;
                        f.clbColor = Color.Red; // All songs played
                    }

                    f.ForceNextPlay = false;

                    f.Text = GetTitle(f);
                }
                else
                    f.StopSong = false;
            }
            catch
            {
            }
        }
        //---------------------------------------------------------------------------
        public bool SetURL(AxWindowsMediaPlayer mp, String rPath)
        {
            if (String.IsNullOrEmpty(rPath)) return false;

            string sDuration;

            // this CAN throw a UrlFormatException!
            Uri uri;
            try { uri = new Uri(rPath); }
            catch { return false; }

            if (uri.IsFile)
            {
                mp.URL = uri.LocalPath; // set the player's URL to this song

                //if (!System.IO.File.Exists(rPath)) return false;

                string sInfo = GetSongInfo(mp, rPath, out sDuration);

                // Display duration at the bottom of SwiftMiX for one of the two players...
                if (!String.IsNullOrEmpty(sDuration))
                {
                    // Player Duration needs posting
                    if (mp == listA.P)
                        TimeDisplay(sDuration, 1); // Player A
                    else
                        TimeDisplay(sDuration, 4); // Player B
                }

                // set tooltip to show song info.
                if (!String.IsNullOrEmpty(sInfo))
                    toolTips.SetToolTip(mp, sInfo);
            }
            else
            {
                //mp.URL = uri.AbsoluteUri; // set the player's URL to this song
                //mp.URL = uri.OriginalString;
                //mp.URL = uri.PathAndQuery;
                //mp.URL = uri.Query;
                //mp.URL = uri.ToString();
                //string url = "http://popurls.com/go/msn.com/l4eba1e6a0ffbd9fc915948434423a7d5";

                string url = uri.OriginalString;

//                var request = (HttpWebRequest)WebRequest.Create(url);
//                request.AllowAutoRedirect = false;

//                var response = (HttpWebResponse)request.GetResponse();

//                if ((int)response.StatusCode == 301 || (int)response.StatusCode == 302)
//                    url = response.Headers["Location"];
//                else
//                {
//                    Uri newUri = request.Address;
//                    url = newUri.AbsolutePath;
//                }

                mp.URL = url;

                // Display duration at the bottom of SwiftMiX for one of the two players...
                // Player Duration needs posting
                sDuration = "99:99:99";
                if (mp == listA.P)
                    TimeDisplay(sDuration, 1); // Player A
                else
                    TimeDisplay(sDuration, 4); // Player B

                //MessageBox.Show(mp.URL.ToString());
            }

            //            MessageBox.Show(rPath);

            return true;
        }
        //---------------------------------------------------------------------------
        public string GetSongInfo(AxWindowsMediaPlayer mp, String rPath, out String sDuration)
        {
            string s =
            sDuration = String.Empty;

            try
            {
                // Get Music tag's Info.
                var mtr = new MediaTags.MediaTags();
                SongInfo2 si = mtr.Read2(rPath);

                try
                {
                    if (!si.bTitleTag || (!si.bArtistTag && !si.bPerformerTag) || !si.bAlbumTag)
                    {
                        SongInfo2 pathsio = mtr.ParsePath(rPath);

                        if (!si.bTitleTag)
                            si.Title = pathsio.Title;

                        if (!si.bArtistTag && !si.bPerformerTag)
                            si.Artist = pathsio.Artist;

                        if (!si.bAlbumTag)
                            si.Album = pathsio.Album;
                    }
                }
                catch { si.bException = true; } // Threw an exception

                // ToolTip song info
                s = "/";
                if (!String.IsNullOrEmpty(si.Artist)) s += si.Artist + "/";
                else if (!String.IsNullOrEmpty(si.Performer)) s += si.Performer + "/";
                if (!String.IsNullOrEmpty(si.Album)) s += si.Album + "/";
                if (s.Length > 30) s += "\r\n";
                if (!String.IsNullOrEmpty(si.Title)) s += si.Title + "/";

                // set our "out" return parameter...
                sDuration = si.bDurationTag ? si.Duration.ToString("hh':'mm':'ss") : string.Empty;

                // if we have a duration, tack it on...
                if (!String.IsNullOrEmpty(sDuration))
                    s += sDuration + "/";
            }
            catch { return ""; }

            return s;
        }
        //---------------------------------------------------------------------------
        void axWindowsMP1_PositionChange(object sender, _WMPOCXEvents_PositionChangeEvent e)
        {
            // User dragged the position-bar...
            int elapsed = listA.Duration - (int)listA.P.Ctlcontrols.currentPosition;

            if (listA.IsPlayOrPause())
            {
                // Set color of status Yellow/Red
                if (elapsed <= RED_STATUS_TIME)
                    SetStatusColorA(Color.Red);
                else if (elapsed <= YELLOW_STATUS_TIME)
                    SetStatusColorA(Color.Yellow);
                else
                    SetStatusColorA(Color.SpringGreen);
            }
            else
                SetStatusColorA(Color.FromArgb(184, 207, 245));
        }
        //---------------------------------------------------------------------------
        void axWindowsMP2_PositionChange(object sender, _WMPOCXEvents_PositionChangeEvent e)
        {
            // User dragged the position-bar...
            int elapsed = listB.Duration - (int)listB.P.Ctlcontrols.currentPosition;

            if (listB.IsPlayOrPause())
            {
                // Set color of status Yellow/Red
                if (elapsed <= RED_STATUS_TIME)
                    SetStatusColorB(Color.Red);
                else if (elapsed <= YELLOW_STATUS_TIME)
                    SetStatusColorB(Color.Yellow);
                else
                    SetStatusColorB(Color.SpringGreen);
            }
            else
                SetStatusColorB(Color.FromArgb(184, 207, 245));
        }
        //---------------------------------------------------------------------------
        void axWindowsMP1_MediaError(object sender, _WMPOCXEvents_MediaErrorEvent e)
        {
            //If this media is bad, queue next
            listA.NextPlayer();

            //try
            //// If the Player encounters a corrupt or missing file, 
            //// show the hexadecimal error code and URL.
            //{
            //  IWMPMedia2 errSource = e.pMediaObject as IWMPMedia2;
            //  IWMPErrorItem errorItem = errSource.Error;
            //  MessageBox.Show("PlayerA Bad Media! -- " + errorItem.errorCode.ToString() +
            //                  " in " + errSource.sourceURL);
            //}
            //catch (InvalidCastException)
            //// In case pMediaObject is not an IWMPMedia item.
            //{
            //  MessageBox.Show("PlayerA Bad Media!");
            //}
        }
        //---------------------------------------------------------------------------
        void axWindowsMP2_MediaError(object sender, _WMPOCXEvents_MediaErrorEvent e)
        {
            //If this media is bad, queue next
            listB.NextPlayer();

            //try
            //// If the Player encounters a corrupt or missing file, 
            //// show the hexadecimal error code and URL.
            //{
            //  IWMPMedia2 errSource = e.pMediaObject as IWMPMedia2;
            //  IWMPErrorItem errorItem = errSource.Error;
            //  MessageBox.Show("PlayerB Bad Media! -- " + errorItem.errorCode.ToString() +
            //                  " in " + errSource.sourceURL);
            //}
            //catch (InvalidCastException)
            //// In case pMediaObject is not an IWMPMedia item.
            //{
            //  MessageBox.Show("PlayerB Bad Media!");
            //}
        }

        #endregion

        #region Status Display/Colors

        public void SetPlaylistColor(FormPlaylist f)
        {
            if (f.clbCheckedCount == 0)
                f.clbColor = Color.Red;
            else if (f.clbCheckedCount == 1)
                f.clbColor = Color.Yellow;
            else
                f.clbColor = Color.FromArgb(184, 207, 245);
        }
        //---------------------------------------------------------------------------
        // Overloaded

        void TimeDisplay(string s, int item)
        // Preformatted
        {
            statusStrip1.Items[item].Text = s;
        }

        void TimeDisplay(int t, int item)
        // t == time in seconds
        {
            int hours = t / (60 * 60);
            int minutes = t / 60;
            int seconds = t % 60;

            statusStrip1.Items[item].Text = String.Format("{0:D2}", hours) + ":" +
              String.Format("{0:D2}", minutes) + ":" + String.Format("{0:D2}", seconds);
        }
        //---------------------------------------------------------------------------

        public void SetStatusColorA(Color c)
        {
            toolStripStatusLabel1.BackColor = c;
            toolStripStatusLabel2.BackColor = c;
            playerAToolStripMenuItem.BackColor = c;
        }
        //---------------------------------------------------------------------------

        public void SetStatusColorB(Color c)
        {
            toolStripStatusLabel4.BackColor = c;
            toolStripStatusLabel5.BackColor = c;
            playerBToolStripMenuItem.BackColor = c;
        }
        //---------------------------------------------------------------------------

        public void UpdatePlayerStatus(FormPlaylist f)
        {
            UpdatePlayerStatus(f, 100000000); // random large #
        }

        public void UpdatePlayerStatus(FormPlaylist f, int remainingTime)
        {
            if (f == null)
                return;

            int yellow = yellowStatusTime;
            if (yellow > f.Duration) yellow = f.Duration;
            int red = redStatusTime;
            if (red > f.Duration) red = f.Duration;

            int current = 0;
            int color = -1;

            try
            {
                if (f == listA)
                {
                    current = (int)axWindowsMP1.Ctlcontrols.currentPosition;

                    if ((red >= 0 && remainingTime <= red) || (red < 0 && current >= -red))
                    {
                        SetStatusColorA(Color.Red);
                        color = 0;
                    }
                    else if ((yellow >= 0 && remainingTime <= yellow) || (yellow < 0 && current >= -yellow))
                    {
                        SetStatusColorA(Color.Yellow);
                        color = 1;
                    }

                    TimeDisplay(current, 0);
                }
                else
                {
                    current = (int)axWindowsMP2.Ctlcontrols.currentPosition;

                    if ((red >= 0 && remainingTime <= red) || (red < 0 && current >= -red))
                    {
                        SetStatusColorB(Color.Red);
                        color = 0;
                    }
                    else if ((yellow >= 0 && remainingTime <= yellow) || (yellow < 0 && current >= -yellow))
                    {
                        SetStatusColorB(Color.Yellow);
                        color = 1;
                    }

                    TimeDisplay(current, 3);
                }

                if (g_bAutoFade)
                    statusStrip1.Items[2].Text = "Auto";
                else
                    statusStrip1.Items[2].Text = "Manual";
            }
            catch
            {
            }

            if (g_bSendTelemetry)
            {
                // Send info to YahCoLoRiZe program
                int size = Marshal.SizeOf(typeof(SWIFTMIX_1));

                // Populate SwiftMixStruct
                SWIFTMIX_1 sms = new SWIFTMIX_1();
                sms.player = (f == listA) ? 0 : 1;
                sms.duration = f.Duration;
                sms.current = current;
                sms.color = color;
                sms.redtime = redStatusTime;
                sms.yellowtime = yellowStatusTime;

                SendToSwiftMix(sms, size, RWM_SwiftMixTime);
            }
        }
        #endregion

        #region Send Telemetry To YahCoLoRiZe

        private bool SendToSwiftMix(Object sms, int size, int msg)
        {
            try
            {
                bool RetVal = false;

                // Allocate memory and move SwiftMixStruct to it
                IntPtr smsMemory = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(sms, smsMemory, true);

                // Populate CopyDataStruct
                NativeMethods.CopyDataStruct cds;
                cds.dwData = msg; // Unique windows message we registered
                cds.cbData = size;
                cds.data = smsMemory;

                // Allocate memory and move CopyDataStruct to it
                IntPtr cdsMemory = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeMethods.CopyDataStruct)));
                Marshal.StructureToPtr(cds, cdsMemory, true);

                // Find the target application window (if running)
                IntPtr WHnd = NativeMethods.FindWindow("TDTSColor", "YahCoLoRiZe");

                // Send the message to target 
                if (!WHnd.Equals(System.IntPtr.Zero) && msg != 0)
                {
                    NativeMethods.SendMessage(WHnd, NativeMethods.WM_COPYDATA, this.Handle, cdsMemory);
                    RetVal = true;
                }

                // Free allocated memory 
                Marshal.FreeHGlobal(smsMemory);
                Marshal.FreeHGlobal(cdsMemory);

                return (RetVal);
            }
            catch
            {
                return (false);
            }
        }
        #endregion

        #region Show Play List

        public Point ShowPlaylist(FormPlaylist f)
        {
            return (ShowPlaylist(f, false));
        }

        public Point ShowPlaylist(FormPlaylist f, bool bForceResize)
        {
            Point NullPoint = new Point(-1, -1);

            if (f == null) return NullPoint;

            try
            {
                // This was not allowing files to be dragged/dropped to empty list...
                //if (f.clbCount == 0 && !bForceResize)
                //{
                //  f.Hide();
                //  MessageBox.Show("Playlist is Empty!");
                //  return (NullPoint);
                //}

                if (f.WindowState == FormWindowState.Minimized)
                {
                    f.WindowState = FormWindowState.Normal;
                    if (!bForceResize) return NullPoint;
                }

                // already showing? skip...
                if (f.Visible && !bForceResize) return NullPoint;

                f.Text = GetTitle(f);

                int tempHeight = 2 * this.Height / 3 - 3;
                int tempWidth = 3 * this.Width / 2 + 5;

                int X = 0, Y = 0;

                if (f == listA)
                {
                    // Player 1
                    X = this.Left - BorderWidth;
                    Y = this.Top - tempHeight - BorderHeight;
                }
                else
                {
                    // Player 2
                    X = (this.Left + this.Width) - tempWidth + BorderWidth;
                    Y = this.Top + this.Height + BorderHeight;
                }

                if (X < 1) X = 1;

                Screen[] screens = Screen.AllScreens;
                Rectangle r = screens[0].WorkingArea;

                int diff = (X + tempWidth) - r.Width;
                if (diff > 0) X -= diff;
                if (Y < 1) Y = 1;
                diff = (Y + tempHeight) - r.Height;
                if (diff > 0) Y -= diff;

                f.Size = new Size(tempWidth, tempHeight);
                f.DesktopLocation = new Point(X, Y);

                f.Show();
                this.Focus(); // Change focus to main window
            }
            catch
            {
                this.Text = "ShowPlaylist() threw an exception";
                return NullPoint;
            }
            return f.DesktopLocation;
        }

        #endregion

        #region Misc

        public bool ForceFade()
        // Manually initiate an "auto-fade"
        // Also triggers next Queued song
        {
            if (listA == null || listB == null) return (false);

            // Check for case where only one player has songs...
            // If only one player has songs, call NextTrack
            // and fade to that player if AutoFade is enabled
            if (listA.clbCheckedCount == 0 && listB.clbCheckedCount != 0)
            {
                listB.NextPlayer();
                return (true);
            }

            if (listA.clbCheckedCount != 0 && listB.clbCheckedCount == 0)
            {
                listA.NextPlayer();
                return true;
            }

            if (!g_bAutoFade)
                return false;

            try
            {
                // player A on now?
                if (listB.clbCheckedCount != 0 && listA.PlayState == WMPLib.WMPPlayState.wmppsPlaying)
                {
                    // Start Player B
                    if (listB.PlayState != WMPLib.WMPPlayState.wmppsPlaying) listB.Play();

                    bFadeRight = true; // Set fade-direction
                    AutoFadeTimer.Enabled = true; // Start a fade
                    return true;
                }

                // player B on now?
                if (listA.clbCheckedCount != 0 && listB.PlayState == WMPLib.WMPPlayState.wmppsPlaying)
                {
                    // Start Player A
                    if (listA.PlayState != WMPLib.WMPPlayState.wmppsPlaying) listA.Play();

                    bFadeRight = false; // Set fade-direction
                    AutoFadeTimer.Enabled = true; // Start a fade
                    return (true);
                }
            }
            catch { MessageBox.Show("ForceFade() threw an exception"); }

            return false;
        }
        //---------------------------------------------------------------------------

        private bool InRange(FormPlaylist f, int idx)
        {
            return (f != null && f.clbCount != 0 && idx >= 0 && idx < f.clbCount);
        }
        //---------------------------------------------------------------------------

        public bool SetVolumes()
        // NOTE: call players directly here since we use this as a 
        // test for valid instances of WMP
        {
            try
            {
                if (bNormalFade)
                {
                    axWindowsMP1.settings.volume = (g_iVolumeA * (trackBar1.Maximum - trackBar1.Value)) / trackBar1.Maximum;
                    axWindowsMP2.settings.volume = (g_iVolumeB * trackBar1.Value) / trackBar1.Maximum;
                }
                else
                {
                    if (trackBar1.Value < trackBar1.Maximum / 2) axWindowsMP1.settings.volume = g_iVolumeA;
                    else axWindowsMP1.settings.volume = g_iVolumeA * ((trackBar1.Maximum - trackBar1.Value) * 2) / trackBar1.Maximum;

                    if (trackBar1.Value >= trackBar1.Maximum / 2) axWindowsMP2.settings.volume = g_iVolumeB;
                    else axWindowsMP2.settings.volume = g_iVolumeB * (trackBar1.Value * 2) / trackBar1.Maximum;
                }
            }
            catch { return false; }

            return true;
        }
        //---------------------------------------------------------------------------

        void Form1_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            Process.Start(HELPSITE);
        }
        //---------------------------------------------------------------------------

        void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            // Fader Moved
            SetVolumes();
        }
        //---------------------------------------------------------------------------

        void Form1_KeyPress(object sender, KeyPressEventArgs e)
        // Get any key press value...
        {
            keyPressed = e.KeyChar;
        }

        //---------------------------------------------------------------------------
        private void toolStripStatusLabel3_Click(object sender, EventArgs e)
        {
            faderModeAutoToolStripMenuItem_Click(null, null);
        }

        #endregion

        #region GetNext

        public string GetNext(FormPlaylist f)
        {
            return (GetNext(f, false, false));
        }

        public string GetNext(FormPlaylist f, bool bNoSet)
        {
            return (GetNext(f, bNoSet, false));
        }

        public string GetNext(FormPlaylist f, bool bNoSet, bool bRandom)
        // Given pointer to a Form containing a listbox,
        // returns the first file-name that has its check-box grayed (queued song).
        // The new index is returned. -1 is returned if no play-enabled files remain.
        //
        // Do not include the second arguement to return PlayIndex and targetIndex.
        // TargetIndex is the index of the next checked item after Tag...
        //
        // Set bNoSet true to cause PlayIndex to be unaffected, instead the next
        // checked item is returned in targetIndex.
        //
        // BEFORE calling this function, set nextIndex to a value to force
        // searching to begin there.
        {
            if (f == null)
                return ("");

            string fileName = "";

            // Problem this fixes: When you programmatically change the check-state
            // it triggers the "check-change" event which sets this flag.  Then
            // when the uset tries to change the song-pointer... nothing happens
            // because the listbox-click method checks this flag and ignores
            // events that were also check-box changes... this seemed like
            // a good spot to clear the flag...
            f.CheckedStateChanged = false;

            try
            {
                if (f == null || f.clbCount == 0)
                {
                    if (!bNoSet)
                        f.PlayIndex = -1;

                    return ("");
                }

                int c = f.clbCount;

                int loops;

                if (bNoSet)
                    loops = 1;
                else
                    loops = 2;

                int songIndex;

                if (!bRandom)
                    songIndex = f.PlayIndex;
                else
                {
                    Random r = new Random();
                    songIndex = r.Next(f.clbCount);
                }

                // 1st iteration gets the next song in the list that is "Queueable" (Indeterminant state)
                // and will return its title. Index is returned as Tag.
                // 2nd iteration gets the index of the next song following that (returned as TargetIndex)
                for (int ii = 0; ii < loops; ii++)
                {
                    // Set NextIndex = -1 to begin search at playIndex.
                    // Set it to pos. number to begin search at that location.
                    if (f.NextIndex < 0 || bRandom)
                        f.NextIndex = songIndex;

                    if (f.NextIndex < 0)
                        return (""); // return null string for a PlayIndex of -1 (no songs left)

                    int jj;
                    for (jj = 0; jj < c; jj++, f.NextIndex++) // for each list item...
                    {
                        if (f.NextIndex >= c)
                            f.NextIndex = 0; // Back to top of list

                        // Is this a queueable item?
                        if (f.clbGetCheckState(f.NextIndex) == CheckState.Indeterminate)
                        {
                            if (ii == 0) // first loop...
                                fileName = f.clbItem(f.NextIndex);

                            break;
                        }
                    }

                    if (jj == c)
                        f.NextIndex = -1; // no Play-flags set

                    if (f.NextIndex >= c)
                    {
                        f.Text = "GetNext() PlayIndex Index is out-of-range!";
                        f.PlayIndex = -1;
                        f.TargetIndex = -1;
                        f.NextIndex = -1;
                        return ("");
                    }

                    if (!bNoSet)
                    {
                        if (ii == 0) // first loop...
                        {
                            f.PlayIndex = f.NextIndex;

                            if (!bRandom)
                                f.NextIndex++;
                            else
                            {
                                Random r = new Random();
                                f.NextIndex = r.Next(f.clbCount);
                            }
                        }
                        else
                            f.TargetIndex = f.NextIndex; // TargetIndex holds 2nd queueable song...
                    }
                    else
                        f.TargetIndex = f.NextIndex; // TargetIndex holds 1st queueable song... (NoSet true)
                }

                f.NextIndex = -1; // return to idle-state... (needed in player state-change!)

                f.Text = GetTitle(f);

                if (f.PlayIndex < c)
                    f.clbSelectedIndex = f.PlayIndex;
                else
                {
                    f.PlayIndex = -1;
                    f.clbSelectedIndex = -1;
                }
            }
            catch
            {
                f.Text = "GetNext() threw an exception...";
            }

            return (fileName);
        }
        #endregion

        #region GetTitle

        public string GetTitle(FormPlaylist f)
        {
            return (GetTitle(f, false));
        }

        public string GetTitle(FormPlaylist f, bool bQ)
        {
            try
            {
                string S1;
                string S2 = "(nothing queued)";

                if (f == listA)
                    S1 = "PlayerA ";
                else if (f == listB)
                    S1 = "PlayerB ";
                else
                    return ("");

                if (f.clbCount != 0)
                {
                    if (f.Focused || bQ) // List Window has focus?
                    {
                        if (f.TargetIndex >= 0)
                        {
                            S1 += "(Q) ";

                            if (InRange(f, f.TargetIndex))
                                S2 = f.clbItem(f.TargetIndex);
                        }
                    }
                    else // Not focused...
                    {
                        if (InRange(f, f.PlayIndex))
                        {
                            if (f.clbGetCheckState(f.PlayIndex) == CheckState.Checked)
                            {
                                S1 += "(P) ";

                                string name = f.P.currentMedia.name;
                                string artist = f.P.currentMedia.getItemInfo("WM/AlbumArtist");
                                string album = f.P.currentMedia.getItemInfo("WM/AlbumTitle");

                                if (name.Length != 0 && album.Length != 0 && artist.Length != 0)
                                    S2 = artist + "/" + album + "/" + name;
                                else
                                    S2 = f.clbItem(f.PlayIndex);
                            }
                            else // not focused and player is idle...
                            {
                                S1 += "(Q) ";
                                S2 = f.clbItem(f.PlayIndex);
                            }
                        }
                    }
                }

                // Buttons and icon
                int Misc = SystemInformation.SmallIconSize.Width +
                    SystemInformation.SmallCaptionButtonSize.Width;

                // Available # chars for song title is the
                // total form width - icon widths - S1 width - "..."
                int W = (f.Width - Misc) / 8 - S1.Length - 3;

                if (S2.Length > W)
                {
                    S1 += "...";
                    return (S1 + S2.Substring(S2.Length - W));
                }
                else
                    return (S1 + S2);
            }
            catch
            {
                return ("(error)");
            }
        }
        #endregion

        #region Auto-Fade Timer

        void AutoFadeTimer_Tick(object sender, EventArgs e)
        {
            // 50ms timer-tick...
            try
            {
                if (bFadeRight) // Fading to PlayerB
                {
                    // Fade all the way after only one timer-tick if user selected 0 in the up/down fade-time control
                    if (AutoFadeTimer.Interval == 1)
                        trackBar1.Value = trackBar1.Maximum;

                    if (trackBar1.Value < trackBar1.Maximum)
                        trackBar1.Value++;
                    else
                    {
                        AutoFadeTimer.Enabled = false;

                        // This is a way to periodically return focus to this main form...
                        //RestoreFocus();

                        if (!listA.ForceNextPlay) // if not too late...
                        {
                            listA.clbStop();
                            SetURL(listA.P, GetNext(listA, false, listA.RandomMode)); // Queue next song
                            if (listA.PlayIndex < 0)
                                listA.clbColor = Color.Red;
                        }
                    }
                }
                else // Fading to PlayerA
                {
                    // Fade all the way after only one timer-tick if
                    // user selected 0 in the up/down fade-time control
                    if (AutoFadeTimer.Interval == 1)
                        trackBar1.Value = trackBar1.Minimum;

                    if (trackBar1.Value > trackBar1.Minimum)
                        trackBar1.Value--;
                    else
                    {
                        AutoFadeTimer.Enabled = false;

                        // this is a way to periodically return focus to this
                        // main form...
                        //RestoreFocus();
                        if (!listB.ForceNextPlay) // if not too late...
                        {
                            listB.clbStop();
                            SetURL(listB.P, GetNext(listB, false, listB.RandomMode)); // Queue next song
                            if (listB.PlayIndex < 0)
                                listB.clbColor = Color.Red;
                        }
                    }
                }
            }
            catch { MessageBox.Show("AutoFadeTimerEvent() threw an exception..."); }
        }
        #endregion

        #region ToolStrip Menu Item Event Handlers

        void eliminateDuplicatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPlaylist fLargest, fSmallest;

            if (listA.clbCount >= listB.clbCount)
            {
                fLargest = listA;
                fSmallest = listB;
            }
            else
            {
                fLargest = listB;
                fSmallest = listA;
            }

            int smallestDel = 0;
            int largestDel = 0;

            for (int ii = 0; ii < fLargest.clbCount; ii++)
            {
                // Remove dups in other list
                for (int jj = 0; jj < fSmallest.clbCount; jj++)
                {
                    if (fLargest.clbItem(ii) == fSmallest.clbItem(jj))
                    {
                        if (DeleteItem(jj, fSmallest))
                        {
                            smallestDel++;
                            jj--;
                        }
                    }
                }

                // Remove dups in this list
                for (int jj = ii + 1; jj < fLargest.clbCount; jj++)
                {
                    if (fLargest.clbItem(ii) == fLargest.clbItem(jj))
                    {
                        if (DeleteItem(jj, fLargest))
                        {
                            largestDel++;
                            jj--;
                        }
                    }
                }
            }

            MessageBox.Show("Removed: " + (smallestDel + largestDel).ToString() + " duplicates.");
        }

        public bool DeleteItem(int ii, FormPlaylist f)
        {
            if (ii == CheckedListBox.NoMatches)
                return false;

            try
            {
                f.clbRemove(ii);

                if (f.clbCount == 0)
                {
                    f.TargetIndex = -1;
                    f.PlayIndex = -1;
                }
                else
                {
                    if (f.TargetIndex > ii)
                        f.TargetIndex--;

                    if (f.PlayIndex > ii)
                        f.PlayIndex--;
                }

                // Set SelectedIndex
                f.SetSelectedToTarget();
                SetPlaylistColor(f);

                return true;
            }
            catch { return false; }
        }
        //---------------------------------------------------------------------------

        void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Add Directories to Player A or B
            DirDialog();
        }
        //---------------------------------------------------------------------------

        void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            // Add Directories to Player A or B
            DirDialog();
        }
        //---------------------------------------------------------------------------

        public bool DirDialog()
        {
            if (listA == null || listB == null)
                return false;

            //if (!FREEWARE && pk.ComputeDaysRemaining() <= 0)
            //{
            //    MessageBox.Show("License Expired: " + FormMain.WEBSITE.ToString());
            //    return false;
            //}

            if (browserDialog != null)
            {
                try
                {
                    if (browserDialog.WindowState != FormWindowState.Normal)
                        browserDialog.WindowState = FormWindowState.Normal;

                    browserDialog.BringToFront();

                    return true;
                }
                catch
                {
                    browserDialog = null;
                }
            }

            try
            {
                browserDialog = new FormDirectories(this, ListA, ListB);

                // Only files we DO want to see in the songView box...
                browserDialog.FileFilterList = this.fileFilterList;

                // Files we DO NOT want to see in the fileView box...
                List<string> fl = new List<string>();
                fl.Add(".db");
                fl.Add(".ini");
                fl.Add(".jpg");
                browserDialog.ExcludeFilterList = fl;

                browserDialog.TitleBar = FormMain.SWIFTMIX2 +
                  ": Right-click a folder to add its music...";
                browserDialog.RootTitle = "My Music";
                browserDialog.RootFolder = this.g_sRootFolder;

                browserDialog.Show();
            }
            catch
            {
                return false;
            }

            return true;
        }
        //---------------------------------------------------------------------------

        //void addFilesToPlaylistToolStripMenuItemA_Click(object sender, EventArgs e)
        //{
        //  if (pk.ComputeDaysRemaining() <= 0)
        //  {
        //    MessageBox.Show("License Expired: " + Form1.WEBSITE.ToString());
        //    return;
        //  }

        //  if (listA == null)
        //    return;

        //  int save = listA.clbCount;
        //  if (FileDialog(listA, listA.T, ref filesDirA) && !PlayOrPause(ListA))
        //    listA.Queue(save);
        //}
        //---------------------------------------------------------------------------

        //void addFilesToPlaylistToolStripMenuItemB_Click(object sender, EventArgs e)
        //{
        //  if (pk.ComputeDaysRemaining() <= 0)
        //  {
        //    MessageBox.Show("License Expired: " + Form1.WEBSITE.ToString());
        //    return;
        //  }

        //  if (listB == null)
        //    return;

        //  int save = listB.clbCount;
        //  if (FileDialog(listB, listB.T, ref filesDirB) && !PlayOrPause(ListB))
        //    listB.Queue(save);
        //}
        //---------------------------------------------------------------------------

        void forceFadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ForceFade();
        }
        //---------------------------------------------------------------------------

        void nextTrackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listA.NextPlayer();
        }
        //---------------------------------------------------------------------------

        void nextTrackToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            listB.NextPlayer();
        }
        //---------------------------------------------------------------------------

        void repeatToolStripMenuItemA_Click(object sender, EventArgs e)
        {
            if (listA == null)
                return;

            if (listA.RepeatMode)
            {
                listA.RepeatMode = false;
                repeatToolStripMenuItemA.Checked = false;
            }
            else
            {
                listA.RepeatMode = true;
                repeatToolStripMenuItemA.Checked = true;
            }
        }
        //---------------------------------------------------------------------------

        void repeatToolStripMenuItemB_Click(object sender, EventArgs e)
        {
            if (listB == null)
                return;

            if (listB.RepeatMode)
            {
                listB.RepeatMode = false;
                repeatToolStripMenuItemB.Checked = false;
            }
            else
            {
                listB.RepeatMode = true;
                repeatToolStripMenuItemB.Checked = true;
            }
        }
        //---------------------------------------------------------------------------

        void randomToolStripMenuItemA_Click(object sender, EventArgs e)
        {
            if (listA == null)
                return;

            if (listA.RandomMode)
            {
                listA.RandomMode = false;
                randomToolStripMenuItemA.Checked = false;
            }
            else
            {
                listA.RandomMode = true;
                randomToolStripMenuItemA.Checked = true;
            }
        }
        //---------------------------------------------------------------------------

        void randomToolStripMenuItemB_Click(object sender, EventArgs e)
        {
            if (listB == null)
                return;

            if (listB.RandomMode)
            {
                listB.RandomMode = false;
                randomToolStripMenuItemB.Checked = false;
            }
            else
            {
                listB.RandomMode = true;
                randomToolStripMenuItemB.Checked = true;
            }
        }
        //---------------------------------------------------------------------------

        void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listA.PlayState == WMPLib.WMPPlayState.wmppsPaused) // pause?
                listA.P.Ctlcontrols.play();
            else
                listA.Play();
        }
        //---------------------------------------------------------------------------

        void playToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            listB.Play();
        }
        //---------------------------------------------------------------------------

        void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            axWindowsMP1.settings.mute = true;
            axWindowsMP1.Ctlcontrols.stop();
            axWindowsMP1.settings.mute = false;
        }
        //---------------------------------------------------------------------------

        void stopToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            axWindowsMP2.settings.mute = true;
            axWindowsMP2.Ctlcontrols.stop();
            axWindowsMP2.settings.mute = false;
        }
        //---------------------------------------------------------------------------

        void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listA.PlayState == WMPLib.WMPPlayState.wmppsPaused) // pause?
                axWindowsMP1.Ctlcontrols.play();
            else
                axWindowsMP1.Ctlcontrols.pause();
        }
        //---------------------------------------------------------------------------

        void pauseToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listA.PlayState == WMPLib.WMPPlayState.wmppsPaused) // pause?
                axWindowsMP2.Ctlcontrols.play();
            else
                axWindowsMP2.Ctlcontrols.pause();
        }
        //---------------------------------------------------------------------------

        void SendTelemetryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!g_bSendTelemetry)
            {
                // Telemetry on
                g_bSendTelemetry = true;
                SendTelemetryToolStripMenuItem.Checked = true;
            }
            else
            {
                // Telemetry off
                g_bSendTelemetry = false;
                SendTelemetryToolStripMenuItem.Checked = false;
            }
        }
        //---------------------------------------------------------------------------

        void faderModeAutoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!g_bAutoFade)
            {
                // Auto fade on
                g_bAutoFade = true;
                faderModeAutoToolStripMenuItem.Checked = true;
                faderModeAutoToolStripMenuItem.Text = "Fader Mode: Auto";
                forceFadeToolStripMenuItem.Enabled = true;
                toolStripStatusLabel3.Text = "Auto";
            }
            else
            {
                // Auto fade off
                g_bAutoFade = false;
                faderModeAutoToolStripMenuItem.Checked = false;
                faderModeAutoToolStripMenuItem.Text = "Fader Mode: Manual";
                forceFadeToolStripMenuItem.Enabled = false;
                toolStripStatusLabel3.Text = "Manual";
            }
        }
        //---------------------------------------------------------------------------

        void faderTypeNormalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bNormalFade)
            {
                // Fader Mode - Center-Fade
                bNormalFade = false;
                faderTypeNormalToolStripMenuItem.Checked = false;
                faderTypeNormalToolStripMenuItem.Text = "Fader Type: Center";
            }
            else
            {
                // Fader Mode - Normal
                bNormalFade = true;
                faderTypeNormalToolStripMenuItem.Checked = true;
                faderTypeNormalToolStripMenuItem.Text = "Fader Type: Normal";
            }
        }
        //---------------------------------------------------------------------------

        void clearPlaylistStopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listA == null)
                return;

            listA.PlayIndex = -1; listA.TargetIndex = -1; listA.NextIndex = -1;
            listA.clbClear(); // reset list

            axWindowsMP1.settings.mute = true;
            axWindowsMP1.Ctlcontrols.stop();
            axWindowsMP1.settings.mute = false;
        }
        //---------------------------------------------------------------------------

        void clearPlaylistStopToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listB == null)
                return;

            listB.PlayIndex = -1; listB.TargetIndex = -1; listB.NextIndex = -1;
            listB.clbClear(); // reset list

            axWindowsMP2.settings.mute = true;
            axWindowsMP2.Ctlcontrols.stop();
            axWindowsMP2.settings.mute = false;
        }

        //---------------------------------------------------------------------------

        void viewPlaylistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show the listbox
            ShowPlaylist(listA);
        }
        //---------------------------------------------------------------------------

        void viewPlaylistToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Show the listbox
            ShowPlaylist(listB);
        }
        //---------------------------------------------------------------------------

        void exportPlaylistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listA == null)
                return;

            ExportClass ec = new ExportClass(this);
            ec.Export(listA, "Player A: Export Song-List", SWIFTMIX2 + " A1");
        }
        //---------------------------------------------------------------------------

        void exportPlaylistToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listB == null)
                return;

            ExportClass ec = new ExportClass(this);
            ec.Export(listB, "Player B: Export Song-List", SWIFTMIX2 + " B1");
        }
        //---------------------------------------------------------------------------

        void importPlaylistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listA == null)
                return;

            ImportClass ec = new ImportClass(this);

            int save = listA.clbCount;
            if (ec.Import(listA, "Player A: Import Song-List") > 0)
            {
                listA.Queue(save);

                // Show the listbox
                SetPlaylistColor(listA);
                ShowPlaylist(listA);
            }
        }
        //---------------------------------------------------------------------------

        void importPlaylistToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listB == null)
                return;

            ImportClass ec = new ImportClass(this);

            int save = listB.clbCount;
            if (ec.Import(listB, "Player B: Import Song-List") > 0)
            {
                listB.Queue(save);

                // Show the listbox
                SetPlaylistColor(listB);
                ShowPlaylist(listB);
            }
        }
        //---------------------------------------------------------------------------

        void highPriorityProcessingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const int NORMAL_PRIORITY_CLASS = 0x00000020;
            //const int IDLE_PRIORITY_CLASS = 0x00000040;
            const int HIGH_PRIORITY_CLASS = 0x00000080;
            //const int REALTIME_PRIORITY_CLASS = 0x00000100;

            const int PROCESS_SET_INFORMATION = 0x0200;
            const int PROCESS_QUERY_INFORMATION = 0x0400;

            try
            {
                int dwProcessId1, dwProcessId2;

                NativeMethods.GetWindowThreadProcessId(axWindowsMP1.Handle, out dwProcessId1);
                NativeMethods.GetWindowThreadProcessId(axWindowsMP2.Handle, out dwProcessId2);

                // Get handle to Process
                int hProcess1 = NativeMethods.OpenProcess(PROCESS_QUERY_INFORMATION | // access flag
                                        PROCESS_SET_INFORMATION,
                                        0, // handle inheritance flag
                                        dwProcessId1); // process identifier
                if (hProcess1 == 0)
                    return;

                // Get handle to Process
                int hProcess2 = NativeMethods.OpenProcess(PROCESS_QUERY_INFORMATION | // access flag
                                        PROCESS_SET_INFORMATION,
                                        0, // handle inheritance flag
                                        dwProcessId2); // process identifier

                if (hProcess2 == 0)
                {
                    NativeMethods.CloseHandle(hProcess1);
                    return;
                }

                int Priority1 = (int)NativeMethods.GetPriorityClass(hProcess1);
                int Priority2 = (int)NativeMethods.GetPriorityClass(hProcess2);

                if (Priority1 == HIGH_PRIORITY_CLASS || Priority2 == HIGH_PRIORITY_CLASS)
                {
                    NativeMethods.SetPriorityClass(hProcess1, NORMAL_PRIORITY_CLASS);
                    NativeMethods.SetPriorityClass(hProcess2, NORMAL_PRIORITY_CLASS);
                }
                else
                {
                    NativeMethods.SetPriorityClass(hProcess1, HIGH_PRIORITY_CLASS);
                    NativeMethods.SetPriorityClass(hProcess2, HIGH_PRIORITY_CLASS);
                }

                Priority1 = (int)NativeMethods.GetPriorityClass(hProcess1);
                Priority2 = (int)NativeMethods.GetPriorityClass(hProcess2);

                // Menu check-mark will reflect the true priority
                if (Priority1 == HIGH_PRIORITY_CLASS && Priority2 == HIGH_PRIORITY_CLASS)
                    highPriorityProcessingToolStripMenuItem.Checked = true;
                else
                    highPriorityProcessingToolStripMenuItem.Checked = false;

                NativeMethods.CloseHandle(hProcess1);
                NativeMethods.CloseHandle(hProcess2);
            }
            catch
            {
            }
        }
        //---------------------------------------------------------------------------

        void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // Vol A 10%
            this.g_iVolumeA = 10;
            SetVolumes();
            UncheckVolA();
            toolStripMenuItem2.Checked = true;
        }
        //---------------------------------------------------------------------------

        void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            // Vol A 25%
            this.g_iVolumeA = 25;
            SetVolumes();
            UncheckVolA();
            toolStripMenuItem3.Checked = true;
        }
        //---------------------------------------------------------------------------

        void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            // Vol A 50%
            this.g_iVolumeA = 50;
            SetVolumes();
            UncheckVolA();
            toolStripMenuItem4.Checked = true;
        }
        //---------------------------------------------------------------------------

        void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            // Vol A 75%
            this.g_iVolumeA = 75;
            SetVolumes();
            UncheckVolA();
            toolStripMenuItem5.Checked = true;
        }
        //---------------------------------------------------------------------------

        void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            // Vol A 100%
            this.g_iVolumeA = 100;
            SetVolumes();
            UncheckVolA();
            toolStripMenuItem6.Checked = true;
        }
        //---------------------------------------------------------------------------

        void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            // Vol B 10%
            this.g_iVolumeB = 10;
            SetVolumes();
            UncheckVolB();
            toolStripMenuItem7.Checked = true;
        }
        //---------------------------------------------------------------------------

        void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            // Vol B 25%
            this.g_iVolumeB = 25;
            SetVolumes();
            UncheckVolB();
            toolStripMenuItem8.Checked = true;
        }
        //---------------------------------------------------------------------------

        void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            // Vol B 50%
            this.g_iVolumeB = 50;
            SetVolumes();
            UncheckVolB();
            toolStripMenuItem9.Checked = true;
        }
        //---------------------------------------------------------------------------

        void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            // Vol B 75%
            this.g_iVolumeB = 75;
            SetVolumes();
            UncheckVolB();
            toolStripMenuItem10.Checked = true;
        }
        //---------------------------------------------------------------------------

        void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            // Vol B 100%
            this.g_iVolumeB = 100;
            SetVolumes();
            UncheckVolB();
            toolStripMenuItem11.Checked = true;
        }
        //---------------------------------------------------------------------------

        private bool SetVolumeMenuFromValueA()
        {
            try
            {
                UncheckVolA();

                if (g_iVolumeA >= 100)
                {
                    g_iVolumeA = 100;
                    toolStripMenuItem6.Checked = true;
                }
                else if (g_iVolumeA >= 75)
                {
                    g_iVolumeA = 75;
                    toolStripMenuItem5.Checked = true;
                }
                else if (g_iVolumeA >= 50)
                {
                    g_iVolumeA = 50;
                    toolStripMenuItem4.Checked = true;
                }
                else if (g_iVolumeA >= 25)
                {
                    g_iVolumeA = 25;
                    toolStripMenuItem3.Checked = true;
                }
                else if (g_iVolumeA >= 10)
                {
                    g_iVolumeA = 10;
                    toolStripMenuItem2.Checked = true;
                }
                else
                {
                    g_iVolumeA = 50;
                    toolStripMenuItem4.Checked = true;
                }
                return true;
            }
            catch { return false; }
        }
        //---------------------------------------------------------------------------

        private bool SetVolumeMenuFromValueB()
        {
            try
            {
                UncheckVolB();

                if (g_iVolumeB >= 100)
                {
                    g_iVolumeB = 100;
                    toolStripMenuItem11.Checked = true;
                }
                else if (g_iVolumeB >= 75)
                {
                    g_iVolumeB = 75;
                    toolStripMenuItem10.Checked = true;
                }
                else if (g_iVolumeB >= 50)
                {
                    g_iVolumeB = 50;
                    toolStripMenuItem9.Checked = true;
                }
                else if (g_iVolumeB >= 25)
                {
                    g_iVolumeB = 25;
                    toolStripMenuItem8.Checked = true;
                }
                else if (g_iVolumeB >= 10)
                {
                    g_iVolumeB = 10;
                    toolStripMenuItem7.Checked = true;
                }
                else
                {
                    g_iVolumeB = 50;
                    toolStripMenuItem9.Checked = true;
                }
                return true;
            }
            catch { return false; }
        }
        //---------------------------------------------------------------------------
        void UncheckVolA()
        {
            toolStripMenuItem2.Checked = false;
            toolStripMenuItem3.Checked = false;
            toolStripMenuItem4.Checked = false;
            toolStripMenuItem5.Checked = false;
            toolStripMenuItem6.Checked = false;
        }
        //---------------------------------------------------------------------------
        void UncheckVolB()
        {
            toolStripMenuItem7.Checked = false;
            toolStripMenuItem8.Checked = false;
            toolStripMenuItem9.Checked = false;
            toolStripMenuItem10.Checked = false;
            toolStripMenuItem11.Checked = false;
        }
        //---------------------------------------------------------------------------
        void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(WEBSITE);
        }
        //---------------------------------------------------------------------------
        void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout f = new FormAbout(this);
            f.ShowDialog();
        }
        //---------------------------------------------------------------------------
        void exportSongFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (!FREEWARE && pk.ComputeDaysRemaining() <= 0)
            //{
            //    MessageBox.Show("License Expired: " + FormMain.WEBSITE.ToString());
            //    return;
            //}

            // Copy all song-list files to directory user selects
            if ((listA != null && listA.clbCount != 0) ||
              (listB != null && listB.clbCount != 0))
            {
                if (listA.PlayState == WMPLib.WMPPlayState.wmppsPlaying ||
                                   listB.PlayState == WMPLib.WMPPlayState.wmppsPlaying)
                {
                    MessageBox.Show("Please stop both players before attempting this operation!");
                    return;
                }

                FolderBrowserDialog browserDialog = new FolderBrowserDialog();

                browserDialog.ShowNewFolderButton = false;
                browserDialog.RootFolder = DefaultSpecialFolder;
                browserDialog.SelectedPath = "";

                if (browserDialog.ShowDialog() == DialogResult.OK)
                {
                    string path = browserDialog.SelectedPath + SWIFTMIX;

                    if (Directory.Exists(path))
                    {
                        MessageBox.Show("Old \"" + SWIFTMIX2 + " Export Files\" already exists, delete it first!");
                        return;
                    }
                    else
                        // Create base directory
                        Directory.CreateDirectory(path);

                    this.progressBar1.Value = 0;
                    this.progressBar1.Show();

                    this.optionsToolStripMenuItem.Enabled = false;
                    this.playerAToolStripMenuItem.Enabled = false;
                    this.playerBToolStripMenuItem.Enabled = false;
                    listA.MenuEnabled = false;
                    listB.MenuEnabled = false;

                    backgroundWorker1.RunWorkerAsync(path);
                }
            }
        }
        //---------------------------------------------------------------------------
        void dockToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Point AOldLocation = ListA.DesktopLocation;
            Point BOldLocation = ListB.DesktopLocation;
            bool ListsWereVisible = listA.Visible && listB.Visible;

            Point ALocation = ShowPlaylist(listA, true); // Force resize...
            Point BLocation = ShowPlaylist(listB, true); // Force resize...

            // Hide playlists if Dock pressed on menu and they are
            // already docked.
            if (ListsWereVisible && AOldLocation == ALocation && BOldLocation == BLocation)
            {
                listA.Hide();
                listB.Hide();
            }
        }
        //---------------------------------------------------------------------------
        void equalizePlaylistsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int diff = Math.Abs(ListA.clbCount - ListB.clbCount);

            if (diff <= 1)
                return;

            FormPlaylist s, d;

            if (ListA.clbCount > ListB.clbCount)
            {
                s = ListA;
                d = ListB;
            }
            else
            {
                s = ListB;
                d = ListA;
            }

            int moveCount = 0;

            while (s.clbCount - d.clbCount > 1)
            {
                int idx = s.clbCount - 1;
                d.clbAdd(s.clbItem(idx));

                if (d.clbCount == 1)
                    d.Queue(0);

                if (DeleteItem(idx, s))
                    moveCount++;
            }

            MessageBox.Show("Moved: " + moveCount + " items.");
            // Show the listboxes
            ShowPlaylist(listA);
            ShowPlaylist(listB);
        }
        //---------------------------------------------------------------------------
        void playerAToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        // Used to print the # of songs in the list next to the "Add Files" item...
        {
            string s = "Add Music Files";

            if (listA.clbCount != 0 && playerAToolStripMenuItem.DropDownItems.Count >= 1)
                s += " (" + listA.clbCount.ToString() + ")";

            playerAToolStripMenuItem.DropDownItems[0].Text = s;
        }
        //---------------------------------------------------------------------------
        void playerBToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        // Used to print the # of songs in the list next to the "Add Files" item...
        {
            string s = "Add Music Files";

            if (listB.clbCount != 0 && playerBToolStripMenuItem.DropDownItems.Count >= 1)
                s += " (" + listB.clbCount.ToString() + ")";

            playerBToolStripMenuItem.DropDownItems[0].Text = s;
        }
        //---------------------------------------------------------------------------
        #endregion

        #region Install/Uninstall/File Association

        //bool ProgramAssociation(bool bAdd)
        //{
        //  try
        //  {
        //    ProgramAssociationInfo pa = new ProgramAssociationInfo(PROG_ID);

        //    if (bAdd)
        //    {
        //      if (!pa.Exists)
        //      {
        //        pa.Create
        //        (
        //          //Description of program/file type
        //        "SwiftMiX audio file",

        //        new ProgramVerb
        //          (
        //          //Verb name
        //          "Open",
        //          //Path and arguments to use
        //          Application.ExecutablePath + " %1"
        //          )
        //        );

        //        //optional
        //        //pa.DefaultIcon = new ProgramIcon(@"C:\SomePath\SomeIcon.ico");
        //        pa.EditFlags = EditFlags.OpenIsSafe;
        //      }
        //    }
        //    else if (pa.Exists)
        //      pa.Delete();
        //  }
        //  catch
        //  {
        //    return false;
        //  }
        //  return true;
        //}
        //---------------------------------------------------------------------------
        ///// <summary>
        ///// Uses FileAssociation.dll to add/remove SwiftMiX from the
        ///// Open With list in Windows Explorer
        ///// </summary>
        ///// <param name="bAdd"><see cref="FileAssociationInfo"/> set this to add a file association</param>
        ///// <param name="Ext">file extension such as \".mp3\"</param>
        //private bool FileAssociation(bool bAdd, string Ext)
        //{
        //  try
        //  {
        //    // Add our file-types to the open-with list
        //    FileAssociationInfo fa = new FileAssociationInfo(Ext);

        //    if (bAdd)
        //    {
        //      if (!fa.Exists)
        //      {
        //        fa.Create(PROG_ID);
        //        fa.ContentType = "audio/mpeg"; // MIME Type
        //        fa.PerceivedType = PerceivedTypes.Audio;
        //        //fa.OpenWithList = new string[] { EXE_FILE };
        //        fa.OpenWithProgIds = new string[] { PROG_ID };
        //      }
        //      else
        //      {
        //        List<string> l = new List<string>();
        //        l.AddRange(fa.OpenWithProgIds);

        //        if (!l.Contains(PROG_ID))
        //        {
        //          l.Add(PROG_ID);
        //          fa.OpenWithProgIds = l.ToArray();
        //        }

        //        // Add the OpensWith (depricated) entry...
        //        //l.Clear();
        //        //l.AddRange(fa.OpenWithList);

        //        //if (!l.Contains(EXE_FILE))
        //        //{
        //        //  l.Add(EXE_FILE);
        //        //  fa.OpenWithList = l.ToArray();
        //        //}
        //      }
        //    }
        //    else // Remove
        //    {
        //      if (fa.Exists)
        //      {
        //        List<string> l = new List<string>();
        //        l.AddRange(fa.OpenWithProgIds);

        //        if (l.Contains(PROG_ID))
        //        {
        //          l.Remove(PROG_ID);
        //          fa.OpenWithProgIds = l.ToArray();
        //        }

        //        // Remove the OpensWith (depricated) entry...
        //        //l.Clear();
        //        //l.AddRange(fa.OpenWithList);

        //        //if (l.Contains(EXE_FILE))
        //        //{
        //        //  l.Remove(EXE_FILE);
        //        //  fa.OpenWithList = l.ToArray();
        //        //}
        //      }
        //    }
        //  }
        //  catch { return false; }
        //  return true;
        //}
        //---------------------------------------------------------------------------
        //private bool IsNewRevision()
        //{
        //  bool bNewVersion = false;

        //  using (RegistryKey parent = Registry.LocalMachine.OpenSubKey(UNINSTALL_KEY, true))
        //  {
        //    if (parent == null) return false;

        //    try
        //    {
        //      RegistryKey key = null;

        //      try
        //      {
        //        if ((key = parent.OpenSubKey(SWIFTMIX2, true)) == null) return true; // No uninstall key return true!

        //        string newVersion = GetType().Assembly.GetName().Version.ToString();
        //        string oldVersion = key.GetValue("DisplayVersion").ToString();

        //        //MessageBox.Show(newVersion + ", " + oldVersion);

        //        if (oldVersion != newVersion) bNewVersion = true;
        //      }
        //      finally { if (key != null) key.Close(); }
        //    }
        //    catch { return true; } // Can't read uninstall info...
        //  }
        //  return bNewVersion;
        //}
        //---------------------------------------------------------------------------
        void RemoveUninstallKey()
        {
            using (RegistryKey parent = Registry.LocalMachine.OpenSubKey(UNINSTALL_KEY, true))
            {
                if (parent == null) return;

                try { parent.DeleteSubKey(SWIFTMIX2, false); }
                catch { }
            }
        }

        //---------------------------------------------------------------------------
        //void CreateUninstaller()
        //{
        //  using (RegistryKey parent = Registry.LocalMachine.OpenSubKey(UNINSTALL_KEY, true))
        //  {
        //    if (parent == null) return;

        //    try
        //    {
        //      RegistryKey key = null;

        //      try
        //      {
        //        key = parent.OpenSubKey(SWIFTMIX2, true) ?? parent.CreateSubKey(SWIFTMIX2);
        //        if (key == null) return;

        //        Assembly asm = GetType().Assembly;
        //        Version v = asm.GetName().Version;
        //        string exe = "\"" + asm.CodeBase.Substring(8).Replace("/", "\\\\") + "\"";

        //        key.SetValue("DisplayName", "SwiftMiX");
        //        key.SetValue("ApplicationVersion", v.ToString());
        //        key.SetValue("Publisher", "Discrete-Time Systems");
        //        key.SetValue("DisplayIcon", exe);
        //        key.SetValue("DisplayVersion", v.ToString());
        //        key.SetValue("URLInfoAbout", "http://www.yahcolorize.com");
        //        key.SetValue("Contact", "dxzl@live.com");
        //        key.SetValue("InstallDate", DateTime.Now.ToString("yyyyMMdd"));
        //        key.SetValue("UninstallString", exe + " /uninstall");
        //      }
        //      finally {if (key != null) key.Close();}
        //    }
        //    catch {}
        //  }
        //}
        //---------------------------------------------------------------------------
        //    void uninstallToolStripMenuItem_Click(object sender, EventArgs e)
        //    // CSIDL_COMMON_STARTMENU
        //    // CSIDL_COMMON_PROGRAMS
        //    // CSIDL_COMMON_DESKTOPDIRECTORY
        //    // string shortcut = Path.Combine(startMenuDir, @"The Company\MyShortcut.lnk");
        //    {
        //      DialogResult result1 = MessageBox.Show("Uninstall SwiftMiX?",
        //                                 "Are you sure?",
        //                                  MessageBoxButtons.YesNo,
        //                                  MessageBoxIcon.Warning,
        //                                  MessageBoxDefaultButton.Button2);

        //      if ( result1 == DialogResult.No ) return;

        //      try
        //      {
        //        string startMenuDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
        //        string shortcut = Path.Combine(startMenuDir, SHORTCUT_FILE);
        //        if (File.Exists(shortcut)) File.Delete(shortcut);

        //        shortcut = Path.Combine(startMenuDir, @"Programs\" + SHORTCUT_FILE);
        //        if (File.Exists(shortcut)) File.Delete(shortcut);

        //        // Remove the C:\Program Data\Microsoft\Windows\Start Menu\Programs\SwiftMix folder (if any)
        //        shortcut = Path.Combine(startMenuDir, @"Programs\SwiftMiX");
        //        if (Directory.Exists(shortcut)) Directory.Delete(shortcut, true);
        //      }
        //      catch {}

        //      try
        //      {
        //        string startMenuDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
        //        string shortcut = Path.Combine(startMenuDir, SHORTCUT_FILE);
        //        if (File.Exists(shortcut)) File.Delete(shortcut);
        //      }
        //      catch {}

        //      // Remove file-associations
        //      // Song types
        //      FileAssociation(false, ".mp3");
        //      FileAssociation(false, ".wma");
        //      FileAssociation(false, ".wav");
        //      // Playlists
        //      FileAssociation(false, ".wpl");
        //      FileAssociation(false, ".m3u");
        //      FileAssociation(false, ".m3u8");
        //      FileAssociation(false, ".pls");

        //      // Remove program-association
        //      ProgramAssociation(false);

        //      // Remove Add/Remove Programs entry
        //      RemoveUninstallKey();

        //      // This works ok but I want to stay with .NET!
        //      //
        //      //StringBuilder path = new StringBuilder(MAX_PATH);
        //      //SHGetSpecialFolderPath(IntPtr.Zero, path, CSIDL_COMMON_PROGRAMS, false);
        //      //shortcut = path.ToString() + @"\" + SHORTCUT_FILE;
        //      //if (File.Exists(shortcut))
        //      //  File.Delete(shortcut);

        //      // Remove ourself and all files!
        //      string StartupDir = System.Windows.Forms.Application.StartupPath;
        //      string[] directories = StartupDir.Split(Path.DirectorySeparatorChar);

        //      //MessageBox.Show(directories[directories.Length - 1]);

        //      // if not the SwiftMiX subdirectory - its dangerous to delete files...
        //      if (directories.Length == 0 || directories[directories.Length - 1] != SWIFTMIX2)
        //      {
        //        // Abort program...
        //        MessageBox.Show(SWIFTMIX2 + " is not in the normal install directory." +
        //                            " As a precaution we won't delete any files..." + Environment.NewLine +
        //                                     Environment.NewLine +
        //                            "You can manually delete " + SWIFTMIX2 + " at: " + Environment.NewLine +
        //                                    "\"" + StartupDir + "\"");
        //        Application.Exit();
        //        return;
        //      }

        //      string ModuleName = System.Windows.Forms.Application.ExecutablePath;

        //      // Change to Discrete-Time Systems Directory first, then use "rd"
        //      // do completely remove TunesFiX and all subdirectories and files...
        //      string sCommand = "/c echo \"Removing " + SWIFTMIX2 +
        //       " Please Wait...\" & ping 1.1.1.1 -n 1 -w 3000 > nul & " + 
        //                           "cd ..\\ & rd /s /q \"" + StartupDir + "\"";

        ////      // Change to Discrete-Time Systems Directory first, then use "rd"
        ////      // do completely remove SwiftMiX and all subdirectories and files...
        ////      string sCommand = "/c echo \"Removing " + SWIFTMIX2 +
        ////       " Please Wait...\" & ping 1.1.1.1 -n 1 -w 3000 > nul & " +
        ////                           "cd ..\\ & rd /s /q \"" + StartupDir + "\"";

        //      Process process = new Process();
        //      process.StartInfo.FileName = "cmd.exe";
        //      process.StartInfo.Arguments = sCommand;
        //      process.Start();

        //      // Abort program...
        //      Application.Exit();
        //    }
        //---------------------------------------------------------------------------
        #endregion

        #region Background Worker For Exporting Songs/Lists

        void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            try
            {
                string path = e.Argument.ToString();
                int percentComplete = 0;

                // Copy listA files
                List<string> alA = new List<string>();

                for (int ii = 0; ii < listA.clbCount; ii++)
                {
                    percentComplete = (100 * (ii + 1)) / this.listA.clbCount;
                    worker.ReportProgress(percentComplete);

                    string temp = listA.clbItem(ii);

                    if (File.Exists(listA.clbItem(ii)) &&
                           !File.Exists(path + @"\" + Path.GetFileName(temp)))
                        File.Copy(temp, path + @"\" + Path.GetFileName(temp));

                    alA.Add(Path.GetFileName(temp));
                }

                // Copy listB files
                List<string> alB = new List<string>();

                percentComplete = 0;

                for (int ii = 0; ii < listB.clbCount; ii++)
                {
                    percentComplete = (100 * (ii + 1)) / this.listB.clbCount;
                    worker.ReportProgress(percentComplete);

                    string temp = this.listB.clbItem(ii);

                    if (File.Exists(this.listB.clbItem(ii)) &&
                           !File.Exists(path + @"\" + Path.GetFileName(temp)))
                        File.Copy(temp, path + @"\" + Path.GetFileName(temp));

                    alB.Add(Path.GetFileName(temp));
                }

                ExportClass ec = new ExportClass(this);
                ec.ExportNoDialog(alA, path + @"\SwiftMiX A.wpl");
                ec.ExportNoDialog(alB, path + @"\SwiftMiX B.wpl");

                e.Result = path;
            }
            catch
            {
                return;
            }
        }
        //---------------------------------------------------------------------------
        void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.progressBar1.Hide();

            this.Focus();

            MessageBox.Show("Both playlists and their song-files have been copied into: \n\n" +
                   e.Result.ToString() + "\n\n" +
                   "You can drag-drop this folder into your CD-Burner program to create a SwiftMiX!\n" +
                   "After the CD is burned, you can add it to your library of SwiftMiXes that can\n" +
                   "be changed on-the-fly in a live-DJ session.\n\n" +
                   "To Play a SwifTMiX CD, click PlayerA->Import Playlist and navigate to\n" +
                   "\"SwiftMiX Export Files\" on the new CD. Double-click \"SwiftMiX A.wpl\"\n" +
                   "to load PlayerA's list.  Repeat this to load \"SwiftMiX B.wpl\" into PlayerB.");

            this.optionsToolStripMenuItem.Enabled = true;
            this.playerAToolStripMenuItem.Enabled = true;
            this.playerBToolStripMenuItem.Enabled = true;
            listA.MenuEnabled = true;
            listB.MenuEnabled = true;
        }
        //---------------------------------------------------------------------------
        void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar1.Value = e.ProgressPercentage;
        }

        #endregion

        #region Numeric Up/Down Event Handlers

        void numericFadeSpeed_ValueChanged(object sender, EventArgs e)
        {
            // On Init:
            //AutoFadeTimer.Interval = AUTOFADE_TIMER_INTERVAL; (50ms)
            //trackBar1.Maximum = TRACKBAR_MAXIMUM; (100 steps)

            g_iFadeSpeed = (int)numericFadeSpeed.Value; // Value from 1-30

            if (g_iFadeSpeed == 0)
                AutoFadeTimer.Interval = 1;
            else
                AutoFadeTimer.Interval = g_iFadeSpeed * 1000 / trackBar1.Maximum;
        }
        //---------------------------------------------------------------------------
        void numericFadePoint_ValueChanged(object sender, EventArgs e)
        {

        }
        //---------------------------------------------------------------------------
        void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            RegistryRead(true);
        }
        #endregion

        #region Drag Drop

        void Form1_DragOver(object sender, DragEventArgs e)
        {
            // Determine whether string data exists in the drop data. If not, then
            // the drop effect reflects that the drop cannot occur.
            //
            // We can accept file-names or a text-directory-string from treeView
            if (e.Data.GetDataPresent(DataFormats.FileDrop) ||
                                  e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Copy;
            else
            {
                e.Effect = DragDropEffects.None;
                return;
            }
        }
        //---------------------------------------------------------------------------
        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Convert drop-coordinates to client
                Point cpDrop = this.PointToClient(new Point(e.X, e.Y));

                // Which player are we over?
                int left = axWindowsMP1.Left;
                int right = left + axWindowsMP1.Width;

                int topA = axWindowsMP1.Top;
                int bottomA = topA + axWindowsMP1.Height;

                int topB = axWindowsMP2.Top;
                int bottomB = topB + axWindowsMP2.Height;

                //MessageBox.Show("topA=" + topA.ToString() + " topB=" + topB.ToString() +
                //                   " botA=" + bottomA.ToString() + " botB=" + bottomB.ToString() +
                //                   " left=" + left.ToString() + " right=" + right.ToString() +
                //                   " e.X=" + clientPoint.X.ToString() + " e.Y=" + clientPoint.Y.ToString());

                bool bInPlayerA = false;
                bool bInPlayerB = false;

                if (cpDrop.X <= left || cpDrop.X >= right)
                    return;

                if (cpDrop.Y > topA && cpDrop.Y < bottomA)
                    bInPlayerA = true;
                else if (cpDrop.Y > topB && cpDrop.Y < bottomB)
                    bInPlayerB = true;
                else
                    return;

                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] filePaths = e.Data.GetData(DataFormats.FileDrop) as string[];

                    if (filePaths.Length == 0)
                        return;

                    if (bInPlayerA)
                        ListA.ExternAddFiles(filePaths);
                    else if (bInPlayerB)
                        ListB.ExternAddFiles(filePaths);
                }
            }
            catch
            {
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
        //---------------------------------------------------------------------------
        void setMusicFileTypesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormFileTypes ff = new FormFileTypes(this);
            ff.Show();
        }
        //---------------------------------------------------------------------------
        public bool IsUri(String sIn)
        {
            // Could have "file:/laptop/D:/path/file.wma" so the key to telling a URL from
            // a drive letter is that url preambles are more than one char!
            //
            // sIn should be trimmed but does not need to be lower-case...
            return sIn.IndexOf(":/") > 1; // > 1 means you must have more than 1 char before the : (like "file:/")
        }
        //---------------------------------------------------------------------------
        public bool IsFileUri(String sIn)
        {
            return sIn.Length >= 6 && sIn.ToLower().IndexOf("file:/") == 0;
        }
        //---------------------------------------------------------------------------
        #endregion
    }

    #endregion

    #region Native Methods and Marshalling Structures

    public class NativeMethods
    {
        public NativeMethods() { }

        [StructLayout(LayoutKind.Sequential)]
        public struct CopyDataStruct
        {
            public int dwData;
            public int cbData;
            public IntPtr data;
        }

        [DllImport("user32.dll")]
        public extern static System.IntPtr FindWindow(string lpClassName, string lpWindowName);

        public const int WM_COPYDATA = 0x004a;

        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        public static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public extern static int RegisterWindowMessage(string lpString);

        [DllImport("user32.dll")]
        public extern static int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public extern static int OpenProcess(int dwDesiredAccess, int bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public extern static int GetPriorityClass(int hProcess);

        [DllImport("kernel32.dll", SetLastError = true)]
        public extern static int SetPriorityClass(int hProcess, int dwPriorityClass);

        [DllImport("kernel32.dll", SetLastError = true)]
        public extern static int CloseHandle(int hObject);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SWIFTMIX_1
    {
        [MarshalAs(UnmanagedType.I4)]
        public int player;

        [MarshalAs(UnmanagedType.I4)]
        public int duration;

        [MarshalAs(UnmanagedType.I4)]
        public int current;

        [MarshalAs(UnmanagedType.I4)]
        public int color;

        [MarshalAs(UnmanagedType.I4)]
        public int redtime;

        [MarshalAs(UnmanagedType.I4)]
        public int yellowtime;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SWIFTMIX_2
    {
        [MarshalAs(UnmanagedType.I4)]
        public int player;

        [MarshalAs(UnmanagedType.I4)]
        public int duration;

        [MarshalAs(UnmanagedType.I4)]
        public int len_path;

        [MarshalAs(UnmanagedType.I4)]
        public int len_name;

        [MarshalAs(UnmanagedType.I4)]
        public int len_artist;

        [MarshalAs(UnmanagedType.I4)]
        public int len_album;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4096)]
        public string path;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        public string name;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        public string artist;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        public string album;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SWIFTMIX_3
    {
        [MarshalAs(UnmanagedType.I4)]
        public int player;

        [MarshalAs(UnmanagedType.I4)]
        public int state;
    }
    #endregion

    // Not Used!!!
    #region DLLImports

    // This works ok but I want to stay with .NET!
    //
    //[DllImport("shell32.dll")]
    //static extern bool SHGetSpecialFolderPath(IntPtr hwndOwner,
    //                    [Out] StringBuilder lpszPath, int nFolder, bool fCreate);

    //[DllImport("kernel32.dll")]
    //static extern bool CreateProcess(
    //  string lpApplicationName,
    //  string lpCommandLine,
    //  IntPtr lpProcessAttributes,
    //  IntPtr lpThreadAttributes,
    //  bool bInheritHandles,
    //  uint dwCreationFlags,
    //  IntPtr lpEnvironment,
    //  string lpCurrentDirectory,
    //  ref STARTUPINFO lpStartupInfo,
    //  out PROCESS_INFORMATION lpProcessInformation
    //  );
    #endregion
    #region STRUCTS

    //    public struct PROCESS_INFORMATION
    //    {
    //        public IntPtr hProcess;
    //        public IntPtr hThread;
    //        public uint dwProcessId;
    //        public uint dwThreadId;
    //    }

    //    public struct STARTUPINFO
    //    {
    //        public uint cb;
    //        public string lpReserved;
    //        public string lpDesktop;
    //        public string lpTitle;
    //        public uint dwX;
    //        public uint dwY;
    //        public uint dwXSize;
    //        public uint dwYSize;
    //        public uint dwXCountChars;
    //        public uint dwYCountChars;
    //        public uint dwFillAttribute;
    //        public uint dwFlags;
    //        public short wShowWindow;
    //        public short cbReserved2;
    //        public IntPtr lpReserved2;
    //        public IntPtr hStdInput;
    //        public IntPtr hStdOutput;
    //        public IntPtr hStdError;
    //    }


    //    public struct SECURITY_ATTRIBUTES
    //    {
    //        public int length;
    //        public IntPtr lpSecurityDescriptor;
    //        public bool bInheritHandle;
    //    }
    #endregion
    #region SpecialFolderConstants

    //    const int CSIDL_DESKTOP                             = 0x0000;  // <desktop> 
    //    const int CSIDL_INTERNET                            = 0x0001;  // Internet Explorer (icon on desktop) 
    //    const int CSIDL_PROGRAMS                            = 0x0002;  // Start Menu\Programs 
    //    const int CSIDL_CONTROLS                            = 0x0003;  // My Computer\Control Panel 
    //    const int CSIDL_PRINTERS                            = 0x0004;  // My Computer\Printers 
    //    const int CSIDL_PERSONAL                            = 0x0005;  // My Documents 
    //    const int CSIDL_FAVORITES                           = 0x0006;  // <user name>\Favorites 
    //    const int CSIDL_STARTUP                             = 0x0007;  // Start Menu\Programs\Startup 
    //    const int CSIDL_RECENT                              = 0x0008;  // <user name>\Recent 
    //    const int CSIDL_SENDTO                              = 0x0009;  // <user name>\SendTo 
    //    const int CSIDL_BITBUCKET                           = 0x000a;  // <desktop>\Recycle Bin 
    //    const int CSIDL_STARTMENU                           = 0x000b;  // <user name>\Start Menu 
    //    const int CSIDL_MYDOCUMENTS                         = CSIDL_PERSONAL; //  Personal was just a silly name for My Documents 
    //    const int CSIDL_MYMUSIC                             = 0x000d;  // "My Music" folder 
    //    const int CSIDL_MYVIDEO                             = 0x000e;  // "My Videos" folder 
    //    const int CSIDL_DESKTOPDIRECTORY                    = 0x0010;  // <user name>\Desktop 
    //    const int CSIDL_DRIVES                              = 0x0011;  // My Computer 
    //    const int CSIDL_NETWORK                             = 0x0012;  // Network Neighborhood (My Network Places) 
    //    const int CSIDL_NETHOOD                             = 0x0013;  // <user name>\nethood 
    //    const int CSIDL_FONTS                               = 0x0014;  // windows\fonts 
    //    const int CSIDL_TEMPLATES                           = 0x0015; 
    //    const int CSIDL_COMMON_STARTMENU                    = 0x0016;  // All Users\Start Menu 
    //    const int CSIDL_COMMON_PROGRAMS                     = 0x0017;  // All Users\Start Menu\Programs 
    //    const int CSIDL_COMMON_STARTUP                      = 0x0018;  // All Users\Startup 
    //    const int CSIDL_COMMON_DESKTOPDIRECTORY             = 0x0019;  // All Users\Desktop 
    //    const int CSIDL_APPDATA                             = 0x001a;  // <user name>\Application Data 
    //    const int CSIDL_PRINTHOOD                           = 0x001b;  // <user name>\PrintHood 
    //    const int CSIDL_LOCAL_APPDATA                       = 0x001c;  // <user name>\Local Settings\Applicaiton Data (non roaming) 
    //    const int CSIDL_ALTSTARTUP                          = 0x001d;  // non localized startup 
    //    const int CSIDL_COMMON_ALTSTARTUP                   = 0x001e;  // non localized common startup 
    //    const int CSIDL_COMMON_FAVORITES                    = 0x001f; 
    //    const int CSIDL_INTERNET_CACHE                      = 0x0020; 
    //    const int CSIDL_COOKIES                             = 0x0021; 
    //    const int CSIDL_HISTORY                             = 0x0022; 
    //    const int CSIDL_COMMON_APPDATA                      = 0x0023;   // All Users\Application Data 
    //    const int CSIDL_WINDOWS                             = 0x0024;   // GetWindowsDirectory() 
    //    const int CSIDL_SYSTEM                              = 0x0025;   // GetSystemDirectory() 
    //    const int CSIDL_PROGRAM_FILES                       = 0x0026;   // C:\Program Files 
    //    const int CSIDL_MYPICTURES                          = 0x0027;   // C:\Program Files\My Pictures 
    //    const int CSIDL_PROFILE                             = 0x0028;   // USERPROFILE 
    //    const int CSIDL_SYSTEMX86                           = 0x0029;   // x86 system directory on RISC 
    //    const int CSIDL_PROGRAM_FILESX86                    = 0x002a;   // x86 C:\Program Files on RISC 
    //    const int CSIDL_PROGRAM_FILES_COMMON                = 0x002b;   // C:\Program Files\Common 
    //    const int CSIDL_PROGRAM_FILES_COMMONX86             = 0x002c;   // x86 Program Files\Common on RISC 
    //    const int CSIDL_COMMON_TEMPLATES                    = 0x002d;   // All Users\Templates 
    //    const int CSIDL_COMMON_DOCUMENTS                    = 0x002e;   // All Users\Documents 
    //    const int CSIDL_COMMON_ADMINTOOLS                   = 0x002f;   // All Users\Start Menu\Programs\Administrative Tools 
    //    const int CSIDL_ADMINTOOLS                          = 0x0030;   // <user name>\Start Menu\Programs\Administrative Tools 
    //    const int CSIDL_CONNECTIONS                         = 0x0031;   // Network and Dial-up Connections 
    //    const int CSIDL_COMMON_MUSIC                        = 0x0035;   // All Users\My Music 
    //    const int CSIDL_COMMON_PICTURES                     = 0x0036;   // All Users\My Pictures 
    //    const int CSIDL_COMMON_VIDEO                        = 0x0037;   // All Users\My Video 
    //    const int CSIDL_RESOURCES                           = 0x0038;   // Resource Direcotry 
    //    const int CSIDL_RESOURCES_LOCALIZED                 = 0x0039;   // Localized Resource Direcotry 
    //    const int CSIDL_COMMON_OEM_LINKS                    = 0x003a;   // Links to All Users OEM specific apps 
    //    const int CSIDL_CDBURN_AREA                         = 0x003b;   // USERPROFILE\Local Settings\Application Data\Microsoft\CD Burning 
    //    const int CSIDL_COMPUTERSNEARME                     = 0x003d;   // Computers Near Me (computered from Workgroup membership)
    //    const int CSIDL_FLAG_CREATE                         = 0x8000;   // combine with CSIDL_ value to force folder creation in SHGetFolderPath() 
    //    const int CSIDL_FLAG_DONT_VERIFY                    = 0x4000;   // combine with CSIDL_ value to return an unverified folder path 
    //    const int CSIDL_FLAG_DONT_UNEXPAND                  = 0x2000;   // combine with CSIDL_ value to avoid unexpanding environment variables 
    //    const int CSIDL_FLAG_NO_ALIAS                       = 0x1000;   // combine with CSIDL_ value to insure non-alias versions of the pidl 
    //    const int CSIDL_FLAG_PER_USER_INIT                  = 0x0800;   // combine with CSIDL_ value to indicate per-user init (eg. upgrade)

    #endregion
}
