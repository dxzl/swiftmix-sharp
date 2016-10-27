//---------------------------------------------------------------------------
// SwiftMiX - Automatically fade songs between two playlists using
// dual Windows Media Player Active-X controls.
//
// Author: Scott Swift
//
// Released to GitHub under GPL v3 October, 2016
//
//---------------------------------------------------------------------------
namespace SwiftMiX
{
    partial class FormMain
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.playerAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
      this.viewPlaylistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.clearPlaylistStopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.importPlaylistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.exportPlaylistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
      this.playToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.nextTrackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
      this.repeatToolStripMenuItemA = new System.Windows.Forms.ToolStripMenuItem();
      this.randomToolStripMenuItemA = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
      this.volumeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
      this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.playerBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
      this.viewPlaylistToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.clearPlaylistStopToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.importPlaylistToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.exportPlaylistToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
      this.playToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.stopToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.pauseToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.nextTrackToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
      this.repeatToolStripMenuItemB = new System.Windows.Forms.ToolStripMenuItem();
      this.randomToolStripMenuItemB = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
      this.volumeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripMenuItem();
      this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
      this.faderModeAutoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.faderTypeNormalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.forceFadeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.exportSongFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.eliminateDuplicatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.equalizePlaylistsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
      this.highPriorityProcessingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.SendTelemetryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.setMusicFileTypesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripTextBox1 = new System.Windows.Forms.ToolStripMenuItem();
      this.dockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.dockToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.AutoFadeTimer = new System.Windows.Forms.Timer(this.components);
      this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
      this.toolTips = new System.Windows.Forms.ToolTip(this.components);
      this.axWindowsMP1 = new AxWMPLib.AxWindowsMediaPlayer();
      this.axWindowsMP2 = new AxWMPLib.AxWindowsMediaPlayer();
      this.panel1 = new System.Windows.Forms.Panel();
      this.trackBar1 = new System.Windows.Forms.TrackBar();
      this.numericFadeSpeed = new System.Windows.Forms.NumericUpDown();
      this.numericFadePoint = new System.Windows.Forms.NumericUpDown();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.panel2 = new System.Windows.Forms.Panel();
      this.statusStrip1 = new System.Windows.Forms.StatusStrip();
      this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
      this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
      this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
      this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
      this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
      this.progressBar1 = new System.Windows.Forms.ProgressBar();
      this.menuStrip1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.axWindowsMP1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.axWindowsMP2)).BeginInit();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericFadeSpeed)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericFadePoint)).BeginInit();
      this.panel2.SuspendLayout();
      this.statusStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // menuStrip1
      // 
      this.menuStrip1.AccessibleDescription = "SwiftMiX Menu";
      this.menuStrip1.AccessibleName = "SwiftMiX Menu";
      this.menuStrip1.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuBar;
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playerAToolStripMenuItem,
            this.playerBToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.toolStripTextBox1,
            this.dockToolStripMenuItem1});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(309, 24);
      this.menuStrip1.TabIndex = 3;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // playerAToolStripMenuItem
      // 
      this.playerAToolStripMenuItem.AccessibleDescription = "SwiftMiX Player A Menu";
      this.playerAToolStripMenuItem.AccessibleName = "SwiftMiX Player A Menu";
      this.playerAToolStripMenuItem.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuItem;
      this.playerAToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripSeparator12,
            this.viewPlaylistToolStripMenuItem,
            this.clearPlaylistStopToolStripMenuItem,
            this.importPlaylistToolStripMenuItem,
            this.exportPlaylistToolStripMenuItem,
            this.toolStripSeparator4,
            this.playToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.pauseToolStripMenuItem,
            this.nextTrackToolStripMenuItem,
            this.toolStripSeparator5,
            this.repeatToolStripMenuItemA,
            this.randomToolStripMenuItemA,
            this.toolStripSeparator6,
            this.volumeToolStripMenuItem,
            this.toolStripSeparator7,
            this.exitToolStripMenuItem});
      this.playerAToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
      this.playerAToolStripMenuItem.Name = "playerAToolStripMenuItem";
      this.playerAToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
      this.playerAToolStripMenuItem.Text = "Player A";
      this.playerAToolStripMenuItem.DropDownOpened += new System.EventHandler(this.playerAToolStripMenuItem_DropDownOpened);
      // 
      // toolStripMenuItem1
      // 
      this.toolStripMenuItem1.AccessibleDescription = "SwiftMiX Add Directory Player A";
      this.toolStripMenuItem1.AccessibleName = "SwiftMiX Add Directory Player A";
      this.toolStripMenuItem1.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuItem;
      this.toolStripMenuItem1.AutoToolTip = true;
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F2)));
      this.toolStripMenuItem1.Size = new System.Drawing.Size(216, 22);
      this.toolStripMenuItem1.Tag = "Add all songs in the directory tree you select";
      this.toolStripMenuItem1.Text = "Add Music";
      this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
      // 
      // toolStripSeparator12
      // 
      this.toolStripSeparator12.Name = "toolStripSeparator12";
      this.toolStripSeparator12.Size = new System.Drawing.Size(213, 6);
      // 
      // viewPlaylistToolStripMenuItem
      // 
      this.viewPlaylistToolStripMenuItem.Name = "viewPlaylistToolStripMenuItem";
      this.viewPlaylistToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F3)));
      this.viewPlaylistToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
      this.viewPlaylistToolStripMenuItem.Text = "View Playlist";
      this.viewPlaylistToolStripMenuItem.Click += new System.EventHandler(this.viewPlaylistToolStripMenuItem_Click);
      // 
      // clearPlaylistStopToolStripMenuItem
      // 
      this.clearPlaylistStopToolStripMenuItem.Name = "clearPlaylistStopToolStripMenuItem";
      this.clearPlaylistStopToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4)));
      this.clearPlaylistStopToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
      this.clearPlaylistStopToolStripMenuItem.Text = "Clear Playlist/Stop";
      this.clearPlaylistStopToolStripMenuItem.Click += new System.EventHandler(this.clearPlaylistStopToolStripMenuItem_Click);
      // 
      // importPlaylistToolStripMenuItem
      // 
      this.importPlaylistToolStripMenuItem.AutoToolTip = true;
      this.importPlaylistToolStripMenuItem.Name = "importPlaylistToolStripMenuItem";
      this.importPlaylistToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
      this.importPlaylistToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
      this.importPlaylistToolStripMenuItem.Text = "Import Playlist";
      this.importPlaylistToolStripMenuItem.ToolTipText = "Import a Windows Media Player play-list";
      this.importPlaylistToolStripMenuItem.Click += new System.EventHandler(this.importPlaylistToolStripMenuItem_Click);
      // 
      // exportPlaylistToolStripMenuItem
      // 
      this.exportPlaylistToolStripMenuItem.Name = "exportPlaylistToolStripMenuItem";
      this.exportPlaylistToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F6)));
      this.exportPlaylistToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
      this.exportPlaylistToolStripMenuItem.Text = "Export Playlist";
      this.exportPlaylistToolStripMenuItem.Click += new System.EventHandler(this.exportPlaylistToolStripMenuItem_Click);
      // 
      // toolStripSeparator4
      // 
      this.toolStripSeparator4.Name = "toolStripSeparator4";
      this.toolStripSeparator4.Size = new System.Drawing.Size(213, 6);
      // 
      // playToolStripMenuItem
      // 
      this.playToolStripMenuItem.AccessibleDescription = "SwiftMiX Play A";
      this.playToolStripMenuItem.AccessibleName = "SwiftMiX Play A";
      this.playToolStripMenuItem.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuItem;
      this.playToolStripMenuItem.Name = "playToolStripMenuItem";
      this.playToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F7)));
      this.playToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
      this.playToolStripMenuItem.Text = "Play";
      this.playToolStripMenuItem.Click += new System.EventHandler(this.playToolStripMenuItem_Click);
      // 
      // stopToolStripMenuItem
      // 
      this.stopToolStripMenuItem.AccessibleDescription = "SwiftMiX Stop A";
      this.stopToolStripMenuItem.AccessibleName = "SwiftMiX Stop A";
      this.stopToolStripMenuItem.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuItem;
      this.stopToolStripMenuItem.Checked = true;
      this.stopToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
      this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
      this.stopToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F8)));
      this.stopToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
      this.stopToolStripMenuItem.Text = "Stop";
      this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
      // 
      // pauseToolStripMenuItem
      // 
      this.pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
      this.pauseToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F9)));
      this.pauseToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
      this.pauseToolStripMenuItem.Text = "Pause";
      this.pauseToolStripMenuItem.Click += new System.EventHandler(this.pauseToolStripMenuItem_Click);
      // 
      // nextTrackToolStripMenuItem
      // 
      this.nextTrackToolStripMenuItem.AutoToolTip = true;
      this.nextTrackToolStripMenuItem.Name = "nextTrackToolStripMenuItem";
      this.nextTrackToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F10)));
      this.nextTrackToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
      this.nextTrackToolStripMenuItem.Text = "Next Track";
      this.nextTrackToolStripMenuItem.ToolTipText = "Go to next checked item on this player";
      this.nextTrackToolStripMenuItem.Click += new System.EventHandler(this.nextTrackToolStripMenuItem_Click);
      // 
      // toolStripSeparator5
      // 
      this.toolStripSeparator5.Name = "toolStripSeparator5";
      this.toolStripSeparator5.Size = new System.Drawing.Size(213, 6);
      // 
      // repeatToolStripMenuItemA
      // 
      this.repeatToolStripMenuItemA.AutoToolTip = true;
      this.repeatToolStripMenuItemA.Name = "repeatToolStripMenuItemA";
      this.repeatToolStripMenuItemA.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F11)));
      this.repeatToolStripMenuItemA.Size = new System.Drawing.Size(216, 22);
      this.repeatToolStripMenuItemA.Text = "Repeat";
      this.repeatToolStripMenuItemA.ToolTipText = "Prevents items from being unchecked (dequeued) after they play";
      this.repeatToolStripMenuItemA.Click += new System.EventHandler(this.repeatToolStripMenuItemA_Click);
      // 
      // randomToolStripMenuItemA
      // 
      this.randomToolStripMenuItemA.AutoToolTip = true;
      this.randomToolStripMenuItemA.Name = "randomToolStripMenuItemA";
      this.randomToolStripMenuItemA.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F12)));
      this.randomToolStripMenuItemA.Size = new System.Drawing.Size(216, 22);
      this.randomToolStripMenuItemA.Text = "Random";
      this.randomToolStripMenuItemA.ToolTipText = "Randomly choose the song from the checked items";
      this.randomToolStripMenuItemA.Click += new System.EventHandler(this.randomToolStripMenuItemA_Click);
      // 
      // toolStripSeparator6
      // 
      this.toolStripSeparator6.Name = "toolStripSeparator6";
      this.toolStripSeparator6.Size = new System.Drawing.Size(213, 6);
      // 
      // volumeToolStripMenuItem
      // 
      this.volumeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.toolStripMenuItem6});
      this.volumeToolStripMenuItem.Name = "volumeToolStripMenuItem";
      this.volumeToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
      this.volumeToolStripMenuItem.Text = "Volume";
      // 
      // toolStripMenuItem2
      // 
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      this.toolStripMenuItem2.Size = new System.Drawing.Size(114, 22);
      this.toolStripMenuItem2.Text = "10%";
      this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
      // 
      // toolStripMenuItem3
      // 
      this.toolStripMenuItem3.Name = "toolStripMenuItem3";
      this.toolStripMenuItem3.Size = new System.Drawing.Size(114, 22);
      this.toolStripMenuItem3.Text = "25%";
      this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
      // 
      // toolStripMenuItem4
      // 
      this.toolStripMenuItem4.Checked = true;
      this.toolStripMenuItem4.CheckState = System.Windows.Forms.CheckState.Checked;
      this.toolStripMenuItem4.Name = "toolStripMenuItem4";
      this.toolStripMenuItem4.Size = new System.Drawing.Size(114, 22);
      this.toolStripMenuItem4.Text = "50%";
      this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
      // 
      // toolStripMenuItem5
      // 
      this.toolStripMenuItem5.Name = "toolStripMenuItem5";
      this.toolStripMenuItem5.Size = new System.Drawing.Size(114, 22);
      this.toolStripMenuItem5.Text = "75%";
      this.toolStripMenuItem5.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
      // 
      // toolStripMenuItem6
      // 
      this.toolStripMenuItem6.Name = "toolStripMenuItem6";
      this.toolStripMenuItem6.Size = new System.Drawing.Size(114, 22);
      this.toolStripMenuItem6.Text = "100%";
      this.toolStripMenuItem6.Click += new System.EventHandler(this.toolStripMenuItem6_Click);
      // 
      // toolStripSeparator7
      // 
      this.toolStripSeparator7.Name = "toolStripSeparator7";
      this.toolStripSeparator7.Size = new System.Drawing.Size(213, 6);
      // 
      // exitToolStripMenuItem
      // 
      this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
      this.exitToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
      this.exitToolStripMenuItem.Text = "Exit";
      // 
      // playerBToolStripMenuItem
      // 
      this.playerBToolStripMenuItem.AccessibleDescription = "SwiftMiX Player B Menu";
      this.playerBToolStripMenuItem.AccessibleName = "SwiftMiX Player B Menu";
      this.playerBToolStripMenuItem.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuItem;
      this.playerBToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem12,
            this.toolStripSeparator13,
            this.viewPlaylistToolStripMenuItem1,
            this.clearPlaylistStopToolStripMenuItem1,
            this.importPlaylistToolStripMenuItem1,
            this.exportPlaylistToolStripMenuItem1,
            this.toolStripSeparator8,
            this.playToolStripMenuItem1,
            this.stopToolStripMenuItem1,
            this.pauseToolStripMenuItem1,
            this.nextTrackToolStripMenuItem1,
            this.toolStripSeparator9,
            this.repeatToolStripMenuItemB,
            this.randomToolStripMenuItemB,
            this.toolStripSeparator10,
            this.volumeToolStripMenuItem1});
      this.playerBToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
      this.playerBToolStripMenuItem.Name = "playerBToolStripMenuItem";
      this.playerBToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
      this.playerBToolStripMenuItem.Text = "Player B";
      this.playerBToolStripMenuItem.DropDownOpened += new System.EventHandler(this.playerBToolStripMenuItem_DropDownOpened);
      // 
      // toolStripMenuItem12
      // 
      this.toolStripMenuItem12.AccessibleDescription = "SwiftMiX Add Directory Player B";
      this.toolStripMenuItem12.AccessibleName = "SwiftMiX Add Directory Player B";
      this.toolStripMenuItem12.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuItem;
      this.toolStripMenuItem12.Name = "toolStripMenuItem12";
      this.toolStripMenuItem12.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F2)));
      this.toolStripMenuItem12.Size = new System.Drawing.Size(221, 22);
      this.toolStripMenuItem12.Text = "Add Music";
      this.toolStripMenuItem12.Click += new System.EventHandler(this.toolStripMenuItem12_Click);
      // 
      // toolStripSeparator13
      // 
      this.toolStripSeparator13.Name = "toolStripSeparator13";
      this.toolStripSeparator13.Size = new System.Drawing.Size(218, 6);
      // 
      // viewPlaylistToolStripMenuItem1
      // 
      this.viewPlaylistToolStripMenuItem1.Name = "viewPlaylistToolStripMenuItem1";
      this.viewPlaylistToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
      this.viewPlaylistToolStripMenuItem1.Size = new System.Drawing.Size(221, 22);
      this.viewPlaylistToolStripMenuItem1.Text = "View Playlist";
      this.viewPlaylistToolStripMenuItem1.Click += new System.EventHandler(this.viewPlaylistToolStripMenuItem1_Click);
      // 
      // clearPlaylistStopToolStripMenuItem1
      // 
      this.clearPlaylistStopToolStripMenuItem1.Name = "clearPlaylistStopToolStripMenuItem1";
      this.clearPlaylistStopToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F4)));
      this.clearPlaylistStopToolStripMenuItem1.Size = new System.Drawing.Size(221, 22);
      this.clearPlaylistStopToolStripMenuItem1.Text = "Clear Playlist/Stop";
      this.clearPlaylistStopToolStripMenuItem1.Click += new System.EventHandler(this.clearPlaylistStopToolStripMenuItem1_Click);
      // 
      // importPlaylistToolStripMenuItem1
      // 
      this.importPlaylistToolStripMenuItem1.Name = "importPlaylistToolStripMenuItem1";
      this.importPlaylistToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
      this.importPlaylistToolStripMenuItem1.Size = new System.Drawing.Size(221, 22);
      this.importPlaylistToolStripMenuItem1.Text = "Import Playlist";
      this.importPlaylistToolStripMenuItem1.Click += new System.EventHandler(this.importPlaylistToolStripMenuItem1_Click);
      // 
      // exportPlaylistToolStripMenuItem1
      // 
      this.exportPlaylistToolStripMenuItem1.Name = "exportPlaylistToolStripMenuItem1";
      this.exportPlaylistToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F6)));
      this.exportPlaylistToolStripMenuItem1.Size = new System.Drawing.Size(221, 22);
      this.exportPlaylistToolStripMenuItem1.Text = "Export Playlist";
      this.exportPlaylistToolStripMenuItem1.Click += new System.EventHandler(this.exportPlaylistToolStripMenuItem1_Click);
      // 
      // toolStripSeparator8
      // 
      this.toolStripSeparator8.Name = "toolStripSeparator8";
      this.toolStripSeparator8.Size = new System.Drawing.Size(218, 6);
      // 
      // playToolStripMenuItem1
      // 
      this.playToolStripMenuItem1.AccessibleDescription = "SwiftMiX Play B";
      this.playToolStripMenuItem1.AccessibleName = "SwiftMiX Play B";
      this.playToolStripMenuItem1.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuItem;
      this.playToolStripMenuItem1.Name = "playToolStripMenuItem1";
      this.playToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F7)));
      this.playToolStripMenuItem1.Size = new System.Drawing.Size(221, 22);
      this.playToolStripMenuItem1.Text = "Play";
      this.playToolStripMenuItem1.Click += new System.EventHandler(this.playToolStripMenuItem1_Click);
      // 
      // stopToolStripMenuItem1
      // 
      this.stopToolStripMenuItem1.AccessibleDescription = "SwiftMiX Stop B";
      this.stopToolStripMenuItem1.AccessibleName = "SwiftMiX Stop B";
      this.stopToolStripMenuItem1.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuItem;
      this.stopToolStripMenuItem1.Checked = true;
      this.stopToolStripMenuItem1.CheckState = System.Windows.Forms.CheckState.Checked;
      this.stopToolStripMenuItem1.Name = "stopToolStripMenuItem1";
      this.stopToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F8)));
      this.stopToolStripMenuItem1.Size = new System.Drawing.Size(221, 22);
      this.stopToolStripMenuItem1.Text = "Stop";
      this.stopToolStripMenuItem1.Click += new System.EventHandler(this.stopToolStripMenuItem1_Click);
      // 
      // pauseToolStripMenuItem1
      // 
      this.pauseToolStripMenuItem1.Name = "pauseToolStripMenuItem1";
      this.pauseToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F9)));
      this.pauseToolStripMenuItem1.Size = new System.Drawing.Size(221, 22);
      this.pauseToolStripMenuItem1.Text = "Pause";
      this.pauseToolStripMenuItem1.Click += new System.EventHandler(this.pauseToolStripMenuItem1_Click);
      // 
      // nextTrackToolStripMenuItem1
      // 
      this.nextTrackToolStripMenuItem1.Name = "nextTrackToolStripMenuItem1";
      this.nextTrackToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F10)));
      this.nextTrackToolStripMenuItem1.Size = new System.Drawing.Size(221, 22);
      this.nextTrackToolStripMenuItem1.Text = "Next Track";
      this.nextTrackToolStripMenuItem1.Click += new System.EventHandler(this.nextTrackToolStripMenuItem1_Click);
      // 
      // toolStripSeparator9
      // 
      this.toolStripSeparator9.Name = "toolStripSeparator9";
      this.toolStripSeparator9.Size = new System.Drawing.Size(218, 6);
      // 
      // repeatToolStripMenuItemB
      // 
      this.repeatToolStripMenuItemB.Name = "repeatToolStripMenuItemB";
      this.repeatToolStripMenuItemB.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F11)));
      this.repeatToolStripMenuItemB.Size = new System.Drawing.Size(221, 22);
      this.repeatToolStripMenuItemB.Text = "Repeat";
      this.repeatToolStripMenuItemB.Click += new System.EventHandler(this.repeatToolStripMenuItemB_Click);
      // 
      // randomToolStripMenuItemB
      // 
      this.randomToolStripMenuItemB.Name = "randomToolStripMenuItemB";
      this.randomToolStripMenuItemB.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F12)));
      this.randomToolStripMenuItemB.Size = new System.Drawing.Size(221, 22);
      this.randomToolStripMenuItemB.Text = "Random";
      this.randomToolStripMenuItemB.Click += new System.EventHandler(this.randomToolStripMenuItemB_Click);
      // 
      // toolStripSeparator10
      // 
      this.toolStripSeparator10.Name = "toolStripSeparator10";
      this.toolStripSeparator10.Size = new System.Drawing.Size(218, 6);
      // 
      // volumeToolStripMenuItem1
      // 
      this.volumeToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem7,
            this.toolStripMenuItem8,
            this.toolStripMenuItem9,
            this.toolStripMenuItem10,
            this.toolStripMenuItem11});
      this.volumeToolStripMenuItem1.Name = "volumeToolStripMenuItem1";
      this.volumeToolStripMenuItem1.Size = new System.Drawing.Size(221, 22);
      this.volumeToolStripMenuItem1.Text = "Volume";
      // 
      // toolStripMenuItem7
      // 
      this.toolStripMenuItem7.Name = "toolStripMenuItem7";
      this.toolStripMenuItem7.Size = new System.Drawing.Size(114, 22);
      this.toolStripMenuItem7.Text = "10%";
      this.toolStripMenuItem7.Click += new System.EventHandler(this.toolStripMenuItem7_Click);
      // 
      // toolStripMenuItem8
      // 
      this.toolStripMenuItem8.Name = "toolStripMenuItem8";
      this.toolStripMenuItem8.Size = new System.Drawing.Size(114, 22);
      this.toolStripMenuItem8.Text = "25%";
      this.toolStripMenuItem8.Click += new System.EventHandler(this.toolStripMenuItem8_Click);
      // 
      // toolStripMenuItem9
      // 
      this.toolStripMenuItem9.Checked = true;
      this.toolStripMenuItem9.CheckState = System.Windows.Forms.CheckState.Checked;
      this.toolStripMenuItem9.Name = "toolStripMenuItem9";
      this.toolStripMenuItem9.Size = new System.Drawing.Size(114, 22);
      this.toolStripMenuItem9.Text = "50%";
      this.toolStripMenuItem9.Click += new System.EventHandler(this.toolStripMenuItem9_Click);
      // 
      // toolStripMenuItem10
      // 
      this.toolStripMenuItem10.Name = "toolStripMenuItem10";
      this.toolStripMenuItem10.Size = new System.Drawing.Size(114, 22);
      this.toolStripMenuItem10.Text = "75%";
      this.toolStripMenuItem10.Click += new System.EventHandler(this.toolStripMenuItem10_Click);
      // 
      // toolStripMenuItem11
      // 
      this.toolStripMenuItem11.Name = "toolStripMenuItem11";
      this.toolStripMenuItem11.Size = new System.Drawing.Size(114, 22);
      this.toolStripMenuItem11.Text = "100%";
      this.toolStripMenuItem11.Click += new System.EventHandler(this.toolStripMenuItem11_Click);
      // 
      // optionsToolStripMenuItem
      // 
      this.optionsToolStripMenuItem.AccessibleDescription = "SwiftMiX Options Menu";
      this.optionsToolStripMenuItem.AccessibleName = "SwiftMiX Options Menu";
      this.optionsToolStripMenuItem.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuItem;
      this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem,
            this.toolStripSeparator14,
            this.faderModeAutoToolStripMenuItem,
            this.faderTypeNormalToolStripMenuItem,
            this.forceFadeToolStripMenuItem,
            this.toolStripSeparator2,
            this.exportSongFilesToolStripMenuItem,
            this.eliminateDuplicatesToolStripMenuItem,
            this.equalizePlaylistsToolStripMenuItem,
            this.toolStripSeparator11,
            this.highPriorityProcessingToolStripMenuItem,
            this.SendTelemetryToolStripMenuItem,
            this.setMusicFileTypesToolStripMenuItem,
            this.toolStripSeparator1,
            this.aboutToolStripMenuItem,
            this.toolStripMenuItem13});
      this.optionsToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
      this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
      this.optionsToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
      this.optionsToolStripMenuItem.Text = "Options";
      // 
      // helpToolStripMenuItem
      // 
      this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
      this.helpToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
      this.helpToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
      this.helpToolStripMenuItem.Text = "Help";
      this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
      // 
      // toolStripSeparator14
      // 
      this.toolStripSeparator14.Name = "toolStripSeparator14";
      this.toolStripSeparator14.Size = new System.Drawing.Size(227, 6);
      // 
      // faderModeAutoToolStripMenuItem
      // 
      this.faderModeAutoToolStripMenuItem.Checked = true;
      this.faderModeAutoToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
      this.faderModeAutoToolStripMenuItem.Name = "faderModeAutoToolStripMenuItem";
      this.faderModeAutoToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
      this.faderModeAutoToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
      this.faderModeAutoToolStripMenuItem.Text = "Fader Mode: Auto";
      this.faderModeAutoToolStripMenuItem.Click += new System.EventHandler(this.faderModeAutoToolStripMenuItem_Click);
      // 
      // faderTypeNormalToolStripMenuItem
      // 
      this.faderTypeNormalToolStripMenuItem.Checked = true;
      this.faderTypeNormalToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
      this.faderTypeNormalToolStripMenuItem.Name = "faderTypeNormalToolStripMenuItem";
      this.faderTypeNormalToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
      this.faderTypeNormalToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
      this.faderTypeNormalToolStripMenuItem.Text = "Fader Type: Normal";
      this.faderTypeNormalToolStripMenuItem.Click += new System.EventHandler(this.faderTypeNormalToolStripMenuItem_Click);
      // 
      // forceFadeToolStripMenuItem
      // 
      this.forceFadeToolStripMenuItem.AccessibleDescription = "SwiftMiX Force Fade";
      this.forceFadeToolStripMenuItem.AccessibleName = "SwiftMiX Force Fade";
      this.forceFadeToolStripMenuItem.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuItem;
      this.forceFadeToolStripMenuItem.Name = "forceFadeToolStripMenuItem";
      this.forceFadeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
      this.forceFadeToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
      this.forceFadeToolStripMenuItem.Text = "Force Fade";
      this.forceFadeToolStripMenuItem.Click += new System.EventHandler(this.forceFadeToolStripMenuItem_Click);
      // 
      // toolStripSeparator2
      // 
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new System.Drawing.Size(227, 6);
      // 
      // exportSongFilesToolStripMenuItem
      // 
      this.exportSongFilesToolStripMenuItem.AutoToolTip = true;
      this.exportSongFilesToolStripMenuItem.Name = "exportSongFilesToolStripMenuItem";
      this.exportSongFilesToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
      this.exportSongFilesToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
      this.exportSongFilesToolStripMenuItem.Text = "Export Song Files";
      this.exportSongFilesToolStripMenuItem.ToolTipText = "Creates a directory of song-files and play-lists that can be burned to CD/DVD";
      this.exportSongFilesToolStripMenuItem.Click += new System.EventHandler(this.exportSongFilesToolStripMenuItem_Click);
      // 
      // eliminateDuplicatesToolStripMenuItem
      // 
      this.eliminateDuplicatesToolStripMenuItem.Name = "eliminateDuplicatesToolStripMenuItem";
      this.eliminateDuplicatesToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
      this.eliminateDuplicatesToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
      this.eliminateDuplicatesToolStripMenuItem.Text = "Eliminate Duplicates";
      this.eliminateDuplicatesToolStripMenuItem.Click += new System.EventHandler(this.eliminateDuplicatesToolStripMenuItem_Click);
      // 
      // equalizePlaylistsToolStripMenuItem
      // 
      this.equalizePlaylistsToolStripMenuItem.Name = "equalizePlaylistsToolStripMenuItem";
      this.equalizePlaylistsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F7;
      this.equalizePlaylistsToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
      this.equalizePlaylistsToolStripMenuItem.Text = "Equalize Playlists";
      this.equalizePlaylistsToolStripMenuItem.Click += new System.EventHandler(this.equalizePlaylistsToolStripMenuItem_Click);
      // 
      // toolStripSeparator11
      // 
      this.toolStripSeparator11.Name = "toolStripSeparator11";
      this.toolStripSeparator11.Size = new System.Drawing.Size(227, 6);
      // 
      // highPriorityProcessingToolStripMenuItem
      // 
      this.highPriorityProcessingToolStripMenuItem.AutoToolTip = true;
      this.highPriorityProcessingToolStripMenuItem.Name = "highPriorityProcessingToolStripMenuItem";
      this.highPriorityProcessingToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F8;
      this.highPriorityProcessingToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
      this.highPriorityProcessingToolStripMenuItem.Text = "High Priority Processing";
      this.highPriorityProcessingToolStripMenuItem.ToolTipText = "Set this for a slower computer";
      this.highPriorityProcessingToolStripMenuItem.Click += new System.EventHandler(this.highPriorityProcessingToolStripMenuItem_Click);
      // 
      // SendTelemetryToolStripMenuItem
      // 
      this.SendTelemetryToolStripMenuItem.Name = "SendTelemetryToolStripMenuItem";
      this.SendTelemetryToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F9;
      this.SendTelemetryToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
      this.SendTelemetryToolStripMenuItem.Text = "Send Timing Telemetry";
      this.SendTelemetryToolStripMenuItem.Click += new System.EventHandler(this.SendTelemetryToolStripMenuItem_Click);
      // 
      // setMusicFileTypesToolStripMenuItem
      // 
      this.setMusicFileTypesToolStripMenuItem.Name = "setMusicFileTypesToolStripMenuItem";
      this.setMusicFileTypesToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F10;
      this.setMusicFileTypesToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
      this.setMusicFileTypesToolStripMenuItem.Text = "Set Music File-Types";
      this.setMusicFileTypesToolStripMenuItem.Click += new System.EventHandler(this.setMusicFileTypesToolStripMenuItem_Click);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(227, 6);
      // 
      // aboutToolStripMenuItem
      // 
      this.aboutToolStripMenuItem.AccessibleDescription = "SwiftMiX About";
      this.aboutToolStripMenuItem.AccessibleName = "SwiftMiX About";
      this.aboutToolStripMenuItem.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuItem;
      this.aboutToolStripMenuItem.AutoToolTip = true;
      this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
      this.aboutToolStripMenuItem.ShortcutKeyDisplayString = "";
      this.aboutToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F11;
      this.aboutToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
      this.aboutToolStripMenuItem.Text = "About";
      this.aboutToolStripMenuItem.ToolTipText = "License Key can be entered from here";
      this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
      // 
      // toolStripMenuItem13
      // 
      this.toolStripMenuItem13.Name = "toolStripMenuItem13";
      this.toolStripMenuItem13.ShortcutKeys = System.Windows.Forms.Keys.F12;
      this.toolStripMenuItem13.Size = new System.Drawing.Size(230, 22);
      this.toolStripMenuItem13.Text = "Restore Factory Settings";
      this.toolStripMenuItem13.Click += new System.EventHandler(this.toolStripMenuItem13_Click);
      // 
      // toolStripTextBox1
      // 
      this.toolStripTextBox1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dockToolStripMenuItem});
      this.toolStripTextBox1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
      this.toolStripTextBox1.Name = "toolStripTextBox1";
      this.toolStripTextBox1.Size = new System.Drawing.Size(12, 20);
      // 
      // dockToolStripMenuItem
      // 
      this.dockToolStripMenuItem.Name = "dockToolStripMenuItem";
      this.dockToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
      this.dockToolStripMenuItem.Text = "Dock";
      // 
      // dockToolStripMenuItem1
      // 
      this.dockToolStripMenuItem1.AutoToolTip = true;
      this.dockToolStripMenuItem1.ForeColor = System.Drawing.SystemColors.MenuHighlight;
      this.dockToolStripMenuItem1.Name = "dockToolStripMenuItem1";
      this.dockToolStripMenuItem1.Size = new System.Drawing.Size(83, 20);
      this.dockToolStripMenuItem1.Text = "Dock Playlists";
      this.dockToolStripMenuItem1.ToolTipText = "Re-attach Song-List windows";
      this.dockToolStripMenuItem1.Click += new System.EventHandler(this.dockToolStripMenuItem1_Click);
      // 
      // AutoFadeTimer
      // 
      this.AutoFadeTimer.Interval = 50;
      this.AutoFadeTimer.Tick += new System.EventHandler(this.AutoFadeTimer_Tick);
      // 
      // backgroundWorker1
      // 
      this.backgroundWorker1.WorkerReportsProgress = true;
      this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
      this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
      this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
      // 
      // axWindowsMP1
      // 
      this.axWindowsMP1.Enabled = true;
      this.axWindowsMP1.Location = new System.Drawing.Point(1, 27);
      this.axWindowsMP1.Name = "axWindowsMP1";
      this.axWindowsMP1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMP1.OcxState")));
      this.axWindowsMP1.Size = new System.Drawing.Size(308, 47);
      this.axWindowsMP1.TabIndex = 12;
      this.axWindowsMP1.MediaError += new AxWMPLib._WMPOCXEvents_MediaErrorEventHandler(this.axWindowsMP1_MediaError);
      this.axWindowsMP1.OpenStateChange += new AxWMPLib._WMPOCXEvents_OpenStateChangeEventHandler(this.axWindowsMP1_OpenStateChange);
      this.axWindowsMP1.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(this.axWindowsMP1_PlayStateChange);
      this.axWindowsMP1.PositionChange += new AxWMPLib._WMPOCXEvents_PositionChangeEventHandler(this.axWindowsMP1_PositionChange);
      // 
      // axWindowsMP2
      // 
      this.axWindowsMP2.Enabled = true;
      this.axWindowsMP2.Location = new System.Drawing.Point(1, 72);
      this.axWindowsMP2.Name = "axWindowsMP2";
      this.axWindowsMP2.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMP2.OcxState")));
      this.axWindowsMP2.Size = new System.Drawing.Size(308, 47);
      this.axWindowsMP2.TabIndex = 13;
      this.axWindowsMP2.MediaError += new AxWMPLib._WMPOCXEvents_MediaErrorEventHandler(this.axWindowsMP2_MediaError);
      this.axWindowsMP2.OpenStateChange += new AxWMPLib._WMPOCXEvents_OpenStateChangeEventHandler(this.axWindowsMP2_OpenStateChange);
      this.axWindowsMP2.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(this.axWindowsMP2_PlayStateChange);
      this.axWindowsMP2.PositionChange += new AxWMPLib._WMPOCXEvents_PositionChangeEventHandler(this.axWindowsMP2_PositionChange);
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.trackBar1);
      this.panel1.Controls.Add(this.numericFadeSpeed);
      this.panel1.Controls.Add(this.numericFadePoint);
      this.panel1.Controls.Add(this.label1);
      this.panel1.Controls.Add(this.label2);
      this.panel1.Location = new System.Drawing.Point(0, 125);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(294, 47);
      this.panel1.TabIndex = 14;
      // 
      // trackBar1
      // 
      this.trackBar1.AccessibleDescription = "SwiftMiX Slider";
      this.trackBar1.AccessibleName = "SwiftMiX Slider";
      this.trackBar1.AccessibleRole = System.Windows.Forms.AccessibleRole.Slider;
      this.trackBar1.Location = new System.Drawing.Point(73, 1);
      this.trackBar1.Maximum = 100;
      this.trackBar1.Name = "trackBar1";
      this.trackBar1.Size = new System.Drawing.Size(236, 32);
      this.trackBar1.TabIndex = 16;
      this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.Both;
      this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
      // 
      // numericFadeSpeed
      // 
      this.numericFadeSpeed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(207)))), ((int)(((byte)(245)))));
      this.numericFadeSpeed.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
      this.numericFadeSpeed.Location = new System.Drawing.Point(28, 3);
      this.numericFadeSpeed.Margin = new System.Windows.Forms.Padding(1);
      this.numericFadeSpeed.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
      this.numericFadeSpeed.Name = "numericFadeSpeed";
      this.numericFadeSpeed.ReadOnly = true;
      this.numericFadeSpeed.Size = new System.Drawing.Size(41, 20);
      this.numericFadeSpeed.TabIndex = 15;
      this.numericFadeSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      this.numericFadeSpeed.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
      this.numericFadeSpeed.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
      this.numericFadeSpeed.ValueChanged += new System.EventHandler(this.numericFadeSpeed_ValueChanged);
      // 
      // numericFadePoint
      // 
      this.numericFadePoint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(207)))), ((int)(((byte)(245)))));
      this.numericFadePoint.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
      this.numericFadePoint.Location = new System.Drawing.Point(28, 24);
      this.numericFadePoint.Margin = new System.Windows.Forms.Padding(1);
      this.numericFadePoint.Maximum = new decimal(new int[] {
            240,
            0,
            0,
            0});
      this.numericFadePoint.Minimum = new decimal(new int[] {
            240,
            0,
            0,
            -2147483648});
      this.numericFadePoint.Name = "numericFadePoint";
      this.numericFadePoint.ReadOnly = true;
      this.numericFadePoint.Size = new System.Drawing.Size(41, 20);
      this.numericFadePoint.TabIndex = 14;
      this.numericFadePoint.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      this.numericFadePoint.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
      this.numericFadePoint.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
      this.numericFadePoint.ValueChanged += new System.EventHandler(this.numericFadePoint_ValueChanged);
      // 
      // label1
      // 
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
      this.label1.Location = new System.Drawing.Point(1, 1);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(20, 20);
      this.label1.TabIndex = 13;
      this.label1.Text = "S";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // label2
      // 
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
      this.label2.Location = new System.Drawing.Point(1, 22);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(20, 20);
      this.label2.TabIndex = 12;
      this.label2.Text = "T";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.statusStrip1);
      this.panel2.Controls.Add(this.progressBar1);
      this.panel2.Location = new System.Drawing.Point(0, 173);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(309, 28);
      this.panel2.TabIndex = 15;
      // 
      // statusStrip1
      // 
      this.statusStrip1.AllowMerge = false;
      this.statusStrip1.AutoSize = false;
      this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
      this.statusStrip1.GripMargin = new System.Windows.Forms.Padding(0);
      this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel3,
            this.toolStripStatusLabel4,
            this.toolStripStatusLabel5});
      this.statusStrip1.Location = new System.Drawing.Point(4, 3);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new System.Drawing.Size(318, 22);
      this.statusStrip1.SizingGrip = false;
      this.statusStrip1.TabIndex = 8;
      this.statusStrip1.Text = "statusStrip1";
      // 
      // toolStripStatusLabel1
      // 
      this.toolStripStatusLabel1.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
      this.toolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.toolStripStatusLabel1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
      this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
      this.toolStripStatusLabel1.Size = new System.Drawing.Size(60, 17);
      this.toolStripStatusLabel1.Spring = true;
      // 
      // toolStripStatusLabel2
      // 
      this.toolStripStatusLabel2.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
      this.toolStripStatusLabel2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.toolStripStatusLabel2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
      this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
      this.toolStripStatusLabel2.Size = new System.Drawing.Size(60, 17);
      this.toolStripStatusLabel2.Spring = true;
      // 
      // toolStripStatusLabel3
      // 
      this.toolStripStatusLabel3.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
      this.toolStripStatusLabel3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.toolStripStatusLabel3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
      this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
      this.toolStripStatusLabel3.Size = new System.Drawing.Size(60, 17);
      this.toolStripStatusLabel3.Spring = true;
      this.toolStripStatusLabel3.Text = "Auto";
      // 
      // toolStripStatusLabel4
      // 
      this.toolStripStatusLabel4.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
      this.toolStripStatusLabel4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.toolStripStatusLabel4.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
      this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
      this.toolStripStatusLabel4.Size = new System.Drawing.Size(60, 17);
      this.toolStripStatusLabel4.Spring = true;
      // 
      // toolStripStatusLabel5
      // 
      this.toolStripStatusLabel5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.toolStripStatusLabel5.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
      this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
      this.toolStripStatusLabel5.Size = new System.Drawing.Size(60, 17);
      this.toolStripStatusLabel5.Spring = true;
      // 
      // progressBar1
      // 
      this.progressBar1.AccessibleDescription = "SwiftMiX Progress Bar";
      this.progressBar1.AccessibleName = "SwiftMiX Progress Bar";
      this.progressBar1.AccessibleRole = System.Windows.Forms.AccessibleRole.ProgressBar;
      this.progressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.progressBar1.ForeColor = System.Drawing.Color.SteelBlue;
      this.progressBar1.Location = new System.Drawing.Point(0, 0);
      this.progressBar1.Name = "progressBar1";
      this.progressBar1.Size = new System.Drawing.Size(309, 28);
      this.progressBar1.TabIndex = 7;
      this.progressBar1.Visible = false;
      // 
      // FormMain
      // 
      this.AccessibleDescription = "SwiftMiX Main Window";
      this.AccessibleName = "SwiftMiX Main Window";
      this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
      this.AllowDrop = true;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(207)))), ((int)(((byte)(245)))));
      this.ClientSize = new System.Drawing.Size(309, 203);
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.axWindowsMP2);
      this.Controls.Add(this.axWindowsMP1);
      this.Controls.Add(this.menuStrip1);
      this.DoubleBuffered = true;
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.HelpButton = true;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.KeyPreview = true;
      this.MainMenuStrip = this.menuStrip1;
      this.MaximizeBox = false;
      this.Name = "FormMain";
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "SwiftMiX";
      this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.Form1_HelpButtonClicked);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
      this.Load += new System.EventHandler(this.Form1_Load);
      this.Shown += new System.EventHandler(this.Form1_Shown);
      this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
      this.DragOver += new System.Windows.Forms.DragEventHandler(this.Form1_DragOver);
      this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.axWindowsMP1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.axWindowsMP2)).EndInit();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericFadeSpeed)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericFadePoint)).EndInit();
      this.panel2.ResumeLayout(false);
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.MenuStrip menuStrip1;
      private System.Windows.Forms.ToolStripMenuItem playerAToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem playerBToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem faderModeAutoToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem forceFadeToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem viewPlaylistToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem clearPlaylistStopToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem importPlaylistToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem exportPlaylistToolStripMenuItem;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
      private System.Windows.Forms.ToolStripMenuItem playToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem nextTrackToolStripMenuItem;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
      private System.Windows.Forms.ToolStripMenuItem repeatToolStripMenuItemA;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
      private System.Windows.Forms.ToolStripMenuItem volumeToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
      private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
      private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
      private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
      private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
      private System.Windows.Forms.ToolStripMenuItem highPriorityProcessingToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
      private System.Windows.Forms.ToolStripMenuItem viewPlaylistToolStripMenuItem1;
      private System.Windows.Forms.ToolStripMenuItem clearPlaylistStopToolStripMenuItem1;
      private System.Windows.Forms.ToolStripMenuItem importPlaylistToolStripMenuItem1;
      private System.Windows.Forms.ToolStripMenuItem exportPlaylistToolStripMenuItem1;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
      private System.Windows.Forms.ToolStripMenuItem playToolStripMenuItem1;
      private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem1;
      private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem1;
      private System.Windows.Forms.ToolStripMenuItem nextTrackToolStripMenuItem1;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
      private System.Windows.Forms.ToolStripMenuItem repeatToolStripMenuItemB;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
      private System.Windows.Forms.ToolStripMenuItem volumeToolStripMenuItem1;
      private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
      private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8;
      private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem9;
      private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem10;
      private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem11;
      private System.Windows.Forms.Timer AutoFadeTimer;
      private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
      private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem12;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
      private System.Windows.Forms.ToolStripMenuItem exportSongFilesToolStripMenuItem;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
      private System.ComponentModel.BackgroundWorker backgroundWorker1;
      private System.Windows.Forms.ToolStripMenuItem randomToolStripMenuItemA;
      private System.Windows.Forms.ToolStripMenuItem randomToolStripMenuItemB;
      private System.Windows.Forms.ToolStripMenuItem eliminateDuplicatesToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem toolStripTextBox1;
      private System.Windows.Forms.ToolStripMenuItem dockToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem dockToolStripMenuItem1;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
      private System.Windows.Forms.ToolStripMenuItem equalizePlaylistsToolStripMenuItem;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
      private System.Windows.Forms.ToolStripMenuItem faderTypeNormalToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem SendTelemetryToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem13;
      private System.Windows.Forms.ToolTip toolTips;
      private System.Windows.Forms.ToolStripMenuItem setMusicFileTypesToolStripMenuItem;
      private AxWMPLib.AxWindowsMediaPlayer axWindowsMP1;
      private AxWMPLib.AxWindowsMediaPlayer axWindowsMP2;
      private System.Windows.Forms.Panel panel1;
      private System.Windows.Forms.TrackBar trackBar1;
      private System.Windows.Forms.NumericUpDown numericFadeSpeed;
      private System.Windows.Forms.NumericUpDown numericFadePoint;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Panel panel2;
      private System.Windows.Forms.StatusStrip statusStrip1;
      private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
      private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
      private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
      private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
      private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;
      public System.Windows.Forms.ProgressBar progressBar1;
    }
}

