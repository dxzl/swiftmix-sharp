using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.IO;
using AxWMPLib;
using TorboSS;
using IWshRuntimeLibrary; // "Process" needs this...
using MediaTags;
using System.Runtime.InteropServices;

namespace SwiftMiX
{
  public partial class FormPlaylist : DockableForm
  {
    #region Constants
    //private const int WM_NCMOUSEMOVE = 0xa0;
    //private const int WM_NCLBUTTONDOWN = 0xa1;
    //private const int WM_NCLBUTTONUP = 0xa2;
    //private const int HTCAPTION = 2;
    private const int SC_MOVE = 0xF010, SC_CLOSE = 0xF060;
    private const int WM_SYSCOMMAND = 0x0112;
    #endregion

    #region Local Vars

    // Drag-Drop
    static private int sTarget, sPlay;
    static private int dragSourceIndex, dragTargetIndex;
    static private Rectangle dragBox;
    static private FormPlaylist dragSource = null;
    static private FormPlaylist dragTarget = null;

    static private Point mousePoint;

    private FormMain f1 = null;
    private int oldPlayIndex = -1;
    private bool bFlashOn = false;
    private ToolTip checkListBoxTooltip;
    private FormStatus fs;

    #endregion

    #region Properties

    // READ ONLY
    private AxWindowsMediaPlayer p = null;
    public AxWindowsMediaPlayer P
    {
      get { return this.p; }
    }

    private string title = "";
    public string Title
    {
      get { return this.title; }
    }

    private int findIndex = 0;
    public int FindIndex
    {
      get { return this.findIndex; }
    }

    // Listbox property access
    public void clbClear()
    {
      this.Clear();
    }

    public void clbStop()
    {
      this.Stop();
    }

    public bool clbAddRange(List<string> list)
    {
      return clbAddRange(list.ToArray());
    }

    public bool clbAddRange(string[] list)
    {
      try
      {
        int oldCount = this.listBox.Items.Count;

        this.listBox.Items.AddRange(list);

        // Set the new songs as "ready to play"
        for (int ii = 0; ii < list.Length; ii++)
          SetCheck(this, ii + oldCount, CheckState.Indeterminate);
      }
      catch
      {
        return false;
      }
      return true;
    }

    public bool clbAdd(string s)
    {
      if (String.IsNullOrEmpty(s)) return false;

      // Need to be able to add an unchecked URL
      try
      {
        Uri uri = new Uri(s);

        if (uri.IsFile)
        {
          // Check the file's extension against the file-filter list
          string ext = Path.GetExtension(s).ToLower();

          // is it an allowed extension?
          int index = 0;

          if (this.FileFilterList.Count > 0)
            index = this.FileFilterList.FindIndex(item => item.ToLower() == ext);

          if (index < 0)
            return false;
          this.listBox.Items.Add(uri.OriginalString, CheckState.Indeterminate);
        }
        else
          this.listBox.Items.Add(uri.AbsoluteUri, CheckState.Indeterminate);
      }
      catch
      {
        return false;
      }

      return true;
    }

    public CheckState clbGetCheckState(int ii)
    {
      return (this.listBox.GetItemCheckState(ii));
    }

    public void clbRemove(int ii)
    {
      this.listBox.Items.RemoveAt(ii);
    }

    public string clbItem(int ii)
    {
      return (this.listBox.Items[ii].ToString());
    }

    public bool IsPlayOrPause()
    {
      return (bPlaySong || bPauseSong);
    }

    public int clbSelectedIndex
    {
      get { return this.listBox.SelectedIndex; }
      set { this.listBox.SelectedIndex = value; }
    }

    // Here the calling program sets the file-filters
    // such as .mp3|.wma|.wav
    private List<string> fileFilterList;
    public List<string> FileFilterList
    {
      get { return this.fileFilterList; }
      set { this.fileFilterList = value; }
    }

    public CheckedListBox.ObjectCollection clbItems
    {
      get { return this.listBox.Items; }
    }

    public int clbCount
    {
      get { return this.listBox.Items.Count; }
    }

    public int clbCheckedCount
    {
      get { return this.listBox.CheckedItems.Count; }
    }

    public Color clbColor
    {
      set { this.listBox.BackColor = value; }
    }

    // WRITE
    public bool FlashTimerEnabled
    {
      set { this.FlashTimer.Enabled = value; }
    }

    public bool PlayerPositionTimerEnabled
    {
      set { this.PlayerPositionTimer.Enabled = value; }
    }

    public bool PlayerControlTimerEnabled
    {
      set { this.PlayerControlTimer.Enabled = value; }
    }

    // READ/WRITE
    public bool MenuEnabled
    {
      get { return this.contextMenuStrip1.Enabled; }
      set { this.contextMenuStrip1.Enabled = value; }
    }

    private bool bCheckedStateChanged = false;
    public bool CheckedStateChanged
    {
      get { return this.bCheckedStateChanged; }
      set { this.bCheckedStateChanged = value; }
    }

    private bool bInhibitCheckEvent = false;
    public bool InhibitCheckEvent
    {
      get { return this.bInhibitCheckEvent; }
      set { this.bInhibitCheckEvent = value; }
    }

    private string findText = "";
    public string FindText
    {
      get { return this.findText; }
      set { this.findText = value; }
    }

    private WMPLib.WMPOpenState openState = WMPLib.WMPOpenState.wmposUndefined;
    public WMPLib.WMPOpenState OpenState
    {
      get { return this.openState; }
      set { this.openState = value; }
    }

    private WMPLib.WMPPlayState playState = WMPLib.WMPPlayState.wmppsUndefined;
    public WMPLib.WMPPlayState PlayState
    {
      get { return this.playState; }
      set { this.playState = value; }
    }

    private int playIndex = -1;
    public int PlayIndex
    {
      get { return this.playIndex; }
      set { this.playIndex = value; }
    }

    private int targetIndex = -1;
    public int TargetIndex
    {
      get { return this.targetIndex; }
      set { this.targetIndex = value; }
    }

    private int nextIndex = -1;
    public int NextIndex
    {
      get { return this.nextIndex; }
      set { this.nextIndex = value; }
    }

    private int playerControlMode = 0;
    public int PlayerControlMode
    {
      get { return this.playerControlMode; }
      set { this.playerControlMode = value; }
    }

    private int duration = 0;
    public int Duration
    {
      get { return this.duration; }
      set { this.duration = value; }
    }

    private bool bForceNextPlay = false;
    public bool ForceNextPlay
    {
      get { return this.bForceNextPlay; }
      set { this.bForceNextPlay = value; }
    }

    private bool bExactCase = false;
    public bool ExactCase
    {
      get { return this.bExactCase; }
      set { this.bExactCase = value; }
    }

    private bool bRepeatMode = false;
    public bool RepeatMode
    {
      get { return this.bRepeatMode; }
      set { this.bRepeatMode = value; }
    }

    private bool bRandomMode = false;
    public bool RandomMode
    {
      get { return this.bRandomMode; }
      set { this.bRandomMode = value; }
    }

    private bool bPauseSong = false;
    public bool PauseSong
    {
      get { return this.bPauseSong; }
      set { this.bPauseSong = value; }
    }

    private bool bPlaySong = false;
    public bool PlaySong
    {
      get { return this.bPlaySong; }
      set { this.bPlaySong = value; }
    }

    private bool bStopSong = false;
    public bool StopSong
    {
      get { return this.bStopSong; }
      set { this.bStopSong = value; }
    }
    //---------------------------------------------------------------------------
    #endregion

    #region Form Event Handlers (Including F1/F3 keys)

    public FormPlaylist(FormMain f, AxWindowsMediaPlayer player, string title)
    {
      InitializeComponent();

      this.f1 = f;
      this.p = player;
      this.title = title;
      this.Owner = f1;

      player.settings.autoStart = false;

      checkListBoxTooltip = new System.Windows.Forms.ToolTip();
      checkListBoxTooltip.SetToolTip(this.listBox, "");
      checkListBoxTooltip.InitialDelay = 1000; // time before a tooltip shows the first time
      checkListBoxTooltip.ReshowDelay = 500; // time before the tooltip shows subsequent times
      checkListBoxTooltip.AutoPopDelay = 2000; // time tooltip remains visible

      //checkListBoxTooltip.AutomaticDelay = 500; // auto sets all the other parameters
      //checkListBoxTooltip.Popup += checkListBoxTip_OnPopup;
      checkListBoxTooltip.Active = true;

      fs = new FormStatus(); // Displays progress and a wait message
    }
    //---------------------------------------------------------------------------
    private void Form2_KeyDown(object sender, KeyEventArgs e)
    {
      // Used to display debugging info
      if (e.KeyCode == Keys.F1)
      {
        MessageBox.Show("targetIndex:" + targetIndex.ToString() + '\n' +
            "nextIndex:" + nextIndex.ToString() + '\n' +
            "SelectedIndex:" + listBox.SelectedIndex.ToString() + '\n' +
            "playIndex:" + playIndex);

        e.Handled = true;
      }
      else if (e.KeyCode == Keys.F3)
      {
        if (++findIndex > listBox.Items.Count)
          findIndex = 0;

        Search();

        if (findIndex < 0)
          MessageBox.Show("No Matches");
        else
        {
          listBox.SelectedIndex = findIndex;
          Queue(listBox.SelectedIndex);
        }

        e.Handled = true;
      }
    }
    //---------------------------------------------------------------------------
    private void Search()
    {
      if (findIndex < 0)
        return;

      for (; findIndex < listBox.Items.Count; findIndex++)
      {
        string item = listBox.Items[findIndex].ToString();
        string ft = findText;

        if (!bExactCase)
        {
          item = item.ToLower();
          ft = ft.ToLower();
        }

        if (item != "" && item.Contains(ft))
          return;
      }

      findIndex = -1;
    }
    //---------------------------------------------------------------------------
    private void Form2_Activated(object sender, EventArgs e)
    {
      // disable highlight-scroll and flasher
      FlashTimer.Enabled = false;
      f1.GetNext(this, true); // Set targetIndex instead of playIndex...
      SetSelectedToTarget();
    }
    //---------------------------------------------------------------------------
    private void Form2_Deactivate(object sender, EventArgs e)
    {
      SetSelectedToTarget(true);
    }
    //---------------------------------------------------------------------------
    #endregion

    #region Listbox Event Handlers

    private void listBox_ItemCheck(object sender, ItemCheckEventArgs e)
    {
      PlayerControlMode = 4; // time-delay update check-box color
      PlayerControlTimer.Enabled = true;

      bCheckedStateChanged = true;

      if (bInhibitCheckEvent)
      {
        bInhibitCheckEvent = false;
        return;
      }

      if (f1.AutoFadeTimerEnabled)
        return;

      if (e.CurrentValue == CheckState.Checked) // Playing?
        NextPlayer(); // Go to next song
      else if (e.CurrentValue == CheckState.Unchecked)
      {
        e.NewValue = CheckState.Indeterminate; // Enable this song
        Queue(listBox.SelectedIndex);
      }
      else if (e.CurrentValue == CheckState.Indeterminate) // indeterminate going to unchecked... queue next
        Queue(listBox.SelectedIndex);
    }
    //---------------------------------------------------------------------------
    private void listBox_MouseClick(object sender, MouseEventArgs e)
    {
      //if (bCheckedStateChanged)
      //{
      //  bCheckedStateChanged = false;
      //  return;
      //}

      if (listBox.SelectedIndex != targetIndex)
        Queue(listBox.SelectedIndex);
    }
    //---------------------------------------------------------------------------
    private void listBox_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (listBox.Items.Count > 0)
      {
        // Don't allow double-click of a checkbox
        if (e.X > listBox.GetItemHeight(0))
        {
          int index = this.listBox.IndexFromPoint(e.Location);
          if (InRange(this, index)) DoPlay(index);
        }
      }
    }
    //---------------------------------------------------------------------------
    #endregion

    #region Overridden Message Handler

    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    protected override void WndProc(ref Message m)
    {
      switch (m.Msg)
      {
        case WM_SYSCOMMAND:
          if (m.WParam == (IntPtr)SC_CLOSE)
          {
            this.Hide();
            m.LParam = IntPtr.Zero;
            return;
          }
          break;
      }
      base.WndProc(ref m);
    }
    //---------------------------------------------------------------------------
    #endregion

    #region Play/Stop/Queue/NextPlayer

    public void SetSelectedToTarget()
    {
      SetSelectedToTarget(false);  // Show "Q" in title, no flashing
    }

    private void SetSelectedToTarget(bool bP)
    {
      if (this.clbCheckedCount == 0)
      {
        FlashTimer.Enabled = false;
        listBox.SelectedIndex = -1;
        return;
      }

      if (targetIndex >= listBox.Items.Count)
      {
        listBox.SelectedIndex = -1;
        Text = "(targetIndex exceeds array-bounds)";
      }
      else
      {
        listBox.SelectedIndex = targetIndex;

        Text = f1.GetTitle(this, !bP);
      }

      if (bP && listBox.SelectedIndex >= 0)
        FlashTimer.Enabled = true;
    }
    //---------------------------------------------------------------------------
    public bool Queue(int Index)
    {
      return (Queue(Index, false));  // Show "Q" in title, no flashing
    }

    public bool Queue(int Index, bool bQ)
    // Set bQ true to permit the "P" in the title 
    // and to permit flashing if player is playing
    // Return true if we set the URL on this player...
    {
      if (!InRange(this, Index))
        return (false);

      bool bURLset = false;

      try
      {
        if (listBox.GetItemCheckState(Index) == CheckState.Unchecked)
          SetCheck(this, Index, CheckState.Indeterminate);

        nextIndex = Index;

        // If player is not playing or paused... set URL.
        // Setting it otherwise would stop the current song
        if (!bPlaySong && !bPauseSong)
        {
          f1.SetURL(p, f1.GetNext(this));
          bURLset = true;
        }

        targetIndex = Index;

        // Update title
        SetSelectedToTarget(bQ);
      }
      catch
      {
      }

      return (bURLset);
    }
    //---------------------------------------------------------------------------
    private void DoPlay(int index)
    {
      if (p == null || !contextMenuStrip1.Enabled) return;

      // If no items, prompt for items
      if (listBox.Items.Count == 0)
      {
        if (FileDialog() && Queue(0))
          Play();
      }
      else
      {
        Queue(index);

        // player is playing or is paused...
        if (bPlaySong || bPauseSong)
        {
          oldPlayIndex = playIndex;

          f1.SetURL(p, listBox.Items[index].ToString());
          playIndex = index;

          // Old listbox checkbox needs to be cleared...
          if (oldPlayIndex != index && InRange(this, oldPlayIndex))
          {
            if (bRepeatMode)
              SetCheck(this, oldPlayIndex, CheckState.Indeterminate);
            else
              SetCheck(this, oldPlayIndex, CheckState.Unchecked);
          }
        }

        // Initiate a force-fade to this player
        if (this.Play() && f1.AutoFadePlayerLists)
        {
          if (this == f1.ListA) f1.FadeRight = false;
          else f1.FadeRight = true;

          f1.AutoFadeTimerEnabled = true; // Start a fade to this player
        }
      }
    }
    //---------------------------------------------------------------------------
    public bool FileDialog()
    {
      try
      {
        OpenFileDialog od = new OpenFileDialog();

        od.Title = this.title;
        od.InitialDirectory = f1.RootFolder;
        od.FilterIndex = 4;
        od.Multiselect = true;
        od.Filter = "Windows Media (*.wma)|*.wma|" +
                       "MP3 (*.mp3)|*.mp3|" +
                          "WAV (*.wav)|*.wav|" +
                          "AIF (*.aif)|*.aif|" +
                          "AIFF (*.aiff)|*.aiff|" +
                             "All Files (*.*)|*.*";

        Application.DoEvents();
        if (od.ShowDialog() == DialogResult.OK)
        {
          string[] files = od.FileNames;

          foreach (string file in files)
            this.clbAdd(file);

          f1.SetVolumes();

          // Show the listbox
          f1.SetPlaylistColor(this);
          f1.ShowPlaylist(this);

          return (true);
        }
      }
      catch
      {
      }

      return (false);
    }
    //---------------------------------------------------------------------------
    public bool Play()
    {
      try
      {
        if (this.p == null || !this.MenuEnabled || !InRange(this, this.PlayIndex)) return false;

        // Queue next song
        if (this.clbGetCheckState(this.PlayIndex) == CheckState.Unchecked) f1.SetURL(p, f1.GetNext(this));

        this.p.settings.mute = true;
        this.p.Ctlcontrols.play();
        this.p.settings.mute = false;
      }
      catch { return false; }

      return true;
    }
    //---------------------------------------------------------------------------
    private void Clear()
    {
      if (bPlaySong) Stop();

      listBox.Items.Clear();
    }
    //---------------------------------------------------------------------------
    private void Stop()
    {
      this.p.settings.mute = true;
      this.p.Ctlcontrols.stop();
      this.p.settings.mute = false;
    }
    //---------------------------------------------------------------------------
    public void NextPlayer()
    {
      NextPlayer(false);
    }

    public void NextPlayer(bool bNoUncheck)
    {
      if (p == null) return;

      oldPlayIndex = playIndex;

      if (bNoUncheck) nextIndex = oldPlayIndex;
      else if (nextIndex < 0) nextIndex = oldPlayIndex + 1;

      string fileName = f1.GetNext(this);

      if (oldPlayIndex >= 0)
      {
        f1.SetURL(p, fileName);
        Text = f1.GetTitle(this);

        // Old listbox checkbox needs to be cleared...
        if (oldPlayIndex < listBox.Items.Count && !bNoUncheck)
        {
          if (bRepeatMode) SetCheck(this, oldPlayIndex, CheckState.Indeterminate);
          else SetCheck(this, oldPlayIndex, CheckState.Unchecked);
        }

        this.Play();
      }
      else // no more checked items
      {
        if (!f1.ForceFade()) Stop();
      }
    }
    //---------------------------------------------------------------------------
    public void SetCheck(FormPlaylist f, int i, CheckState c)
    {
      f.bInhibitCheckEvent = true; // don't process check-change event
      f.listBox.SetItemCheckState(i, c);
      f.bInhibitCheckEvent = false;
    }
    //---------------------------------------------------------------------------
    #endregion

    #region Cut/Paste

    private bool AddSongToPlaylist(int idx, FormPlaylist f, string fileName, bool bSourcePlaying, bool bLocal)
    {
      // Check the file's extension against the file-filter list
      string ext = Path.GetExtension(fileName).ToLower();

      int index = 0;

      if (this.FileFilterList.Count > 0)
        index = this.FileFilterList.FindIndex(item => item.ToLower() == ext);

      if (index >= 0 && System.IO.File.Exists(fileName))
      {
        InsertItem(idx, f, fileName, bSourcePlaying, bLocal);
        Queue(targetIndex);
        return true;
      }

      // filePath might be a playlist... (this might be a .jpg or desktop.ini also!)
      using (ImportClass ic = new ImportClass(f1))
      {
        if (ic.ProcessSongListFile(-1, fileName, this) > 0) // specify "Auto" encoding
        {
          if (clbCount > 0)
            Queue(0);
        }
      }

      //        if (f.listBox.TopIndex != f.listBox.SelectedIndex)
      //          // Make the currently selected item the top item in the ListBox.
      //          f.listBox.TopIndex = f.listBox.SelectedIndex;

      return false;
    }
    //---------------------------------------------------------------------------
    private void InsertItem(int idx, FormPlaylist f, string fileName, bool bSourcePlaying, bool bLocal)
    {
      try
      {
        // Insert the item.
        if (InRange(this, idx))
        {
          f.listBox.Items.Insert(idx, fileName);

          f.targetIndex = idx;

          if (f.playIndex >= idx)
            f.playIndex++;
        }
        else
        {
          f.listBox.Items.Add(fileName);
          f.targetIndex = f.listBox.Items.Count - 1;
        }

        if (bSourcePlaying && bLocal)
        {
          SetCheck(this, idx, CheckState.Checked);
          f.playIndex = f.targetIndex;
        }
        else
          SetCheck(f, f.targetIndex, CheckState.Indeterminate);

        SetSelectedToTarget();
        f1.SetPlaylistColor(f);
      }
      catch
      {
      }
    }
    //---------------------------------------------------------------------------

    private void DoRemove(int idx, FormPlaylist f, bool bLocal)
    // idx is set to the new targetIndex on return
    {
      if (idx != CheckedListBox.NoMatches)
      {
        // Save CheckState
        CheckState cs = f.listBox.GetItemCheckState(idx);

        f1.DeleteItem(idx, f);

        if (f.listBox.Items.Count != 0 && !bLocal)
        {
          if (cs == CheckState.Checked) // was deleted item playing?
            f.NextPlayer(true);  // don't uncheck item, it was deleted
          else if (idx != CheckedListBox.NoMatches)
          {
            f.nextIndex = idx; // start search here
            f1.GetNext(f, true); // search for next checked item (returned in targetIndex)
            idx = f.targetIndex;
            f.Queue(idx);
          }
          else
            idx = -1;
        }
        else
          idx = -1;
      }
      else
        idx = -1;

      // Stop the music!
      if (idx == -1)
        f.Stop();
    }
    //---------------------------------------------------------------------------
    #endregion

    #region Timer Handlers

    private void PlayerControlTimer_Tick(object sender, EventArgs e)
    {
      // Do this first because if we do it last, calling the media-player's
      // controls may stick on that call for longer than this timer's re-trigger
      // interval... so this hook re-triggers falsely causing a loop...
      this.PlayerControlTimer.Enabled = false;

      if (this.playerControlMode == 4) // update checkbox background color
        f1.SetPlaylistColor(this);
      else if (this.playerControlMode == 3) // time-delay start player
        this.Play();
      else if (this.playerControlMode == 1) // time-delay stop player
      {
        this.bForceNextPlay = true;

        Stop();

        // this is a way to periodically return focus to the
        // main form...
        //RestoreFocus();
      }

    }
    //---------------------------------------------------------------------------

    private void PlayerPositionTimer_Tick(object sender, EventArgs e)
    {
      FormPlaylist fOther = null;

      if (this == f1.ListA)
        fOther = f1.ListB;
      else
        fOther = f1.ListA;

      // Return if this player is not playing...
      if (this.playState != WMPLib.WMPPlayState.wmppsPlaying)
        return;

      try
      {
        int elapsed = (int)this.p.Ctlcontrols.currentPosition;
        int remaining = this.Duration - elapsed;

        // Set color of status Yellow/Red as song plays toward a fade...
        // and time readout
        f1.UpdatePlayerStatus(this, remaining);


        if (f1.AutoFadePlayerLists && fOther.clbCheckedCount > 0)
        {
          if ((f1.StartFadeRemainingTime < 0 && elapsed >= -f1.StartFadeRemainingTime) ||
            (f1.StartFadeRemainingTime >= 0 && remaining <= f1.StartFadeRemainingTime))
          {
            // Start other player
            if (fOther.PlayState != WMPLib.WMPPlayState.wmppsPlaying) // Song has 10 seconds (fade)
              fOther.Play();

            // Fade to other player
            if (this == f1.ListA)
            {
              if (f1.TrackBarValue != f1.TrackBarMax)
              {
                f1.FadeRight = true; // Fade to B
                f1.AutoFadeTimerEnabled = true; // Start a fade
              }
            }
            else
            {
              if (f1.TrackBarValue != 0)
              {
                f1.FadeRight = false; // Fade to A
                f1.AutoFadeTimerEnabled = true; // Start a fade
              }
            }
          }
          else if (remaining < FormMain.NEXT_SONG_TIME) // Song about to end
                                                        // Force next song to begin play when this song ends
            this.bForceNextPlay = true;
        }
        else if (remaining < FormMain.NEXT_SONG_TIME) // Song about to end
                                                      // Force next song to begin play when this song ends
          this.bForceNextPlay = true;
      }
      catch
      {
        this.Text = "Position1Timer event-hook threw an exception...";
      }
    }
    //---------------------------------------------------------------------------

    private void FlashTimer_Tick(object sender, EventArgs e)
    {
      // 10=ready/stop
      // 1=stopped
      // 2=paused
      // 3=playing
      if (!FlashTimer.Enabled)
        return;

      // if player is in pause or in play-mode
      if (bPlaySong || bPauseSong)
      {
        // Flash faster if in pause
        if (bPauseSong)
        {
          if (FlashTimer.Interval != 250)
            FlashTimer.Interval = 250;
        }
        else if (FlashTimer.Interval != 500)
          FlashTimer.Interval = 500;

        if (bFlashOn)
        {
          listBox.SelectedIndex = -1;
          bFlashOn = false;
        }
        else
        {
          listBox.SelectedIndex = playIndex;
          bFlashOn = true;
        }
      }
      else
        FlashTimer.Enabled = false;
    }
    //---------------------------------------------------------------------------
    #endregion

    #region Drag/Drop Handlers

    //---------------------------------------------------------------------------
    // START DRAG-DROP
    //---------------------------------------------------------------------------

    private void listBox_MouseDown(object sender, MouseEventArgs e)
    {
      mousePoint = e.Location;

      try
      {
        // Remember the point where the mouse down occurred. The DragSize indicates
        // the size that the mouse can move before a drag event should be started.                
        Size dragSize = SystemInformation.DragSize;

        // Create a rectangle using the DragSize, with the mouse position being
        // at the center of the rectangle.
        dragBox = new Rectangle(new Point(e.X - (dragSize.Width / 2),
                                  e.Y - (dragSize.Height / 2)), dragSize);
      }
      catch
      {
        dragBox = Rectangle.Empty;
      }
    }
    //---------------------------------------------------------------------------
    static int oldHoverIndex = -1;
    private void listBox_MouseMove(object sender, MouseEventArgs e)
    {
      // Save song selection
      sTarget = targetIndex;
      sPlay = playIndex;

      try
      {
        // If the mouse moves outside the rectangle, start the drag.
        if ((e.Button & MouseButtons.Left) == MouseButtons.Left &&
          dragBox != Rectangle.Empty && !dragBox.Contains(e.X, e.Y))
        {
          dragSource = this;

          // Get the index of the item the mouse is below.
          dragSourceIndex = dragSource.listBox.SelectedIndex;
          dragTargetIndex = -1;

          if (dragSourceIndex != ListBox.NoMatches)
          {
            string[] array = { dragSource.listBox.Items[dragSourceIndex].ToString() };
            var data = new DataObject(DataFormats.FileDrop, array);
            dragSource.listBox.DoDragDrop(data, DragDropEffects.Copy);
          }
          else
          {
            dragBox = Rectangle.Empty;
            dragSource = null;
          }
        }
      }
      catch
      {
        dragBox = Rectangle.Empty;
        dragSource = null;
      }


      // handle toolTip
      int hoverIndex = listBox.IndexFromPoint(e.X, e.Y);
      if (hoverIndex != oldHoverIndex && hoverIndex >= 0 && hoverIndex < listBox.Items.Count)
      {
        checkListBoxTooltip.Active = false;
        ShowToolTip(hoverIndex);
        checkListBoxTooltip.Active = true;
        oldHoverIndex = hoverIndex;
      }
    }
    //---------------------------------------------------------------------------
    private void listBox_MouseEnter(object sender, EventArgs e)
    {
      checkListBoxTooltip.Active = true;
    }
    //---------------------------------------------------------------------------
    private void listBox_Leave(object sender, EventArgs e)
    {
      checkListBoxTooltip.Active = false;
    }
    //---------------------------------------------------------------------------
    private void listBox_MouseUp(object sender, MouseEventArgs e)
    {
      // NOTE: this event hook won't fire for normal drag/drop... event
      // is purged i suppose...

      // Reset the drag rectangle when the mouse button is raised.
      dragBox = Rectangle.Empty;
    }
    //---------------------------------------------------------------------------
    private void ShowToolTip(int index)
    {
      if (index > -1)
      {
        string rPath = listBox.Items[index].ToString();

        if (!System.IO.File.Exists(rPath)) return;

        string sDuration; // not used (out parameter)
        string song = f1.GetSongInfo(P, rPath, out sDuration);

        if (!String.IsNullOrEmpty(song))
        {
          Point p = PointToClient(MousePosition);
          checkListBoxTooltip.Show(song, this, p.X, p.Y, 4000);

          //checkListBoxTooltip.ToolTipTitle = "Tooltip Title";
          checkListBoxTooltip.SetToolTip(listBox, song);
        }
      }
    }
    //---------------------------------------------------------------------------
    private void listBox_DragOver(object sender, DragEventArgs e)
    {
      if (this != f1.ListA && this != f1.ListB)
      {
        dragTarget = null;
        return;
      }

      try
      {
        // Determine whether string data exists in the drop data. If not, then
        // the drop effect reflects that the drop cannot occur.
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
          e.Effect = DragDropEffects.Copy;
        else if (e.Data.GetDataPresent(DataFormats.StringFormat) && (e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
          e.Effect = DragDropEffects.Move;
        else
        {
          e.Effect = DragDropEffects.None;
          return;
        }

        dragTarget = this;

        // Get the index of the item the mouse is below. 
        // The mouse locations are relative to the screen, so they must be 
        // converted to client coordinates.
        dragTargetIndex = dragTarget.listBox.IndexFromPoint(dragTarget.listBox.PointToClient(new Point(e.X, e.Y)));
      }
      catch
      {
        dragTarget = null;
      }
    }
    //---------------------------------------------------------------------------
    // Called from the main form when items are dropped on a player
    // filePaths is a string array of filenames and/or directories
    public void ExternAddFiles(string[] filePaths)
    {
      listBox.ItemCheck -= listBox_ItemCheck;

      try
      {
        f1.ShowPlaylist(this);
        this.Cursor = Cursors.WaitCursor;
        this.listBox.BeginUpdate(); // Freeze listbox while we add files
        Application.DoEvents();

        int oldCount = this.listBox.Items.Count;

        // Get the filenames and add to the listView
        foreach (string filePath in filePaths)
        {
          // Code to read the contents of the text file
          if (Directory.Exists(filePath))
          {
            // Get files and subdir files - even shortcuts are expanded...
            List<string> allFiles = new List<string>();
            allFiles = GetAllFilesRecurse(filePath);

            //Add files from folder
            foreach (string file in allFiles)
            {
              AddSongToPlaylist(listBox.Items.Count, this, file, false, true);
              Application.DoEvents();
            }
          }
          else
            AddSongToPlaylist(listBox.Items.Count, this, filePath, false, true);

          Application.DoEvents();
        }

        int newCount = this.listBox.Items.Count;

        if (oldCount == 0 && newCount > oldCount) // List was empty and we added?
          Queue(0);
      }
      catch
      {
      }
      finally
      {
        listBox.EndUpdate();
        Cursor = Cursors.Default;
        listBox.ItemCheck += listBox_ItemCheck;
      }
    }
    //---------------------------------------------------------------------------
    private void listBox_DragDrop(object sender, DragEventArgs e)
    {
      if (dragTarget == null)
        return;

      listBox.ItemCheck -= listBox_ItemCheck;

      try
      {
        this.Cursor = Cursors.WaitCursor;
        this.listBox.BeginUpdate(); // Freeze listbox while we add files

        int oldCount = this.listBox.Items.Count;

        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
          string[] filePaths = e.Data.GetData(DataFormats.FileDrop) as string[];

          // Get the filenames and add to the listView
          foreach (string filePath in filePaths)
          {
            String Ext = Path.GetExtension(filePath).ToLower();

            // Code to read the contents of the text file
            if (String.IsNullOrEmpty(Ext) && Directory.Exists(filePath))
            {
              // filePath is a Directory - recursively add all of the songs in it and its subfolders...

              // Get files and subdir files - even shortcuts are expanded...
              List<string> allFiles = new List<string>();
              allFiles = GetAllFilesRecurse(filePath);

              // Reverse unless list empty or paste at end...
              if (dragTargetIndex >= 0)
                allFiles.Reverse(0, allFiles.Count); // Reverse order for paste

              //Add files from folder
              foreach (string file in allFiles)
                AddSongToPlaylist(dragTargetIndex, dragTarget, file, false, true);
            }
            else // filePath is a single file
              AddSongToPlaylist(dragTargetIndex, dragTarget, filePath, false, true);
          }
        }
        // Ensure that the list item index is contained in the data.
        else if (e.Data.GetDataPresent(DataFormats.StringFormat) &&
          e.Effect == DragDropEffects.Move &&
              System.IO.File.Exists(e.Data.GetData(DataFormats.StringFormat) as string))
        {
          // Source list-box
          // Restore song selection
          dragSource.targetIndex = sTarget;
          dragSource.playIndex = sPlay;
          dragSource.listBox.SelectedIndex = sTarget;
          dragSource.Text = f1.GetTitle(dragSource);

          bool bLocal = (dragSource == dragTarget) ? true : false;

          // Drag-drop of playing song?
          bool bSourcePlaying = (dragSource.listBox.GetItemCheckState(dragSourceIndex) ==
              CheckState.Checked) ? true : false;

          DoRemove(dragSourceIndex, dragSource, bLocal);

          // Target list-box
          string s = e.Data.GetData(DataFormats.StringFormat) as string;

          AddSongToPlaylist(dragTargetIndex, dragTarget, s, bSourcePlaying, bLocal);
        }

        int newCount = listBox.Items.Count;

        if (oldCount == 0 && newCount > oldCount) // List was empty and we added?
          Queue(0);
      }
      catch
      {
        dragTarget = null;
      }
      finally
      {
        listBox.EndUpdate();
        Cursor = Cursors.Default;
        listBox.ItemCheck += listBox_ItemCheck;
      }
    }
    //---------------------------------------------------------------------------
    // Called after user clicks
    // right on a treenode or clicks the ">>" button.
    private List<string> GetAllFilesRecurse(string path)
    {
      string[] allfiles = System.IO.Directory.GetFiles(path, "*.*",
                                    System.IO.SearchOption.AllDirectories);

      List<string> list = new List<string>();

      foreach (string file in allfiles)
      {
        if (System.IO.Path.GetExtension(file).ToLower() == ".lnk")
        {
          // Get shortcut's real path and add it if valid
          //Create a new WshShell Interface
          WshShell shell = new WshShell();

          //Link the interface to our shortcut
          WshShortcut link = (WshShortcut)shell.CreateShortcut(file);

          //string s = file.Name + ", " + file.Directory + ", " + file.DirectoryName + ", " +
          //file.FullName + ", " + file.Extension + ", " + file.Exists;
          //MessageBox.Show(s);
          //string s = link.FullName + ", " + link.TargetPath + ", " + link.WorkingDirectory;
          //MessageBox.Show(s);

          if (Directory.Exists(link.TargetPath) == true)
            list.AddRange(GetAllFilesRecurse(link.TargetPath));
          else if (System.IO.File.Exists(link.TargetPath) == true)
            list.Add(link.TargetPath);
        }
        else
          list.Add(file);
      }

      return list;
    }
    //---------------------------------------------------------------------------
    // END DRAG-DROP
    //---------------------------------------------------------------------------
    #endregion

    #region Popup Menu Event Handlers

    private void playToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (playState == WMPLib.WMPPlayState.wmppsPaused) // pause?
        p.Ctlcontrols.play();
      else
        DoPlay(listBox.SelectedIndex);
    }
    //---------------------------------------------------------------------------
    private void stopToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (bPlaySong || bPauseSong)
        Stop();
      else
        Clear();
    }
    //---------------------------------------------------------------------------
    private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (playState == WMPLib.WMPPlayState.wmppsPaused) // pause?
        p.Ctlcontrols.play();
      else
        p.Ctlcontrols.pause();
    }
    //---------------------------------------------------------------------------
    private void nextTrackToolStripMenuItem_Click(object sender, EventArgs e)
    {
      NextPlayer();
    }
    //---------------------------------------------------------------------------
    private void removeAllUncheckedToolStripMenuItem_Click(object sender, EventArgs e)
    {
      // Delete all Unchecked items...
      for (int ii = listBox.Items.Count - 1; ii >= 0; ii--)
        if (listBox.GetItemCheckState(ii) == CheckState.Unchecked)
          f1.DeleteItem(ii, this);

      // nothing playing?
      if (playIndex < 0)
        Queue(0);

      SetSelectedToTarget();
    }
    //---------------------------------------------------------------------------
    private void checkAllToolStripMenuItem_Click(object sender, EventArgs e)
    {
      // set all items to "queued"
      for (int i = 0; i < listBox.Items.Count; i++)
        if (listBox.GetItemCheckState(i) == CheckState.Unchecked)
          SetCheck(this, i, CheckState.Indeterminate);

      // nothing playing?
      if (playIndex < 0)
        Queue(0);
    }
    //---------------------------------------------------------------------------
    private void flipListMenuItem_Click(object sender, EventArgs e)
    {
      int topIdx = 0, bottomIdx = listBox.Items.Count - 1;
      object temp;

      while (topIdx <= bottomIdx)
      {
        temp = listBox.Items[topIdx];
        listBox.Items[topIdx] = listBox.Items[bottomIdx];
        listBox.Items[bottomIdx] = temp;

        if (targetIndex == topIdx)
          targetIndex = bottomIdx;
        else if (targetIndex == bottomIdx)
          targetIndex = topIdx;

        if (playIndex == topIdx)
          playIndex = bottomIdx;
        else if (targetIndex == bottomIdx)
          playIndex = topIdx;

        topIdx++;
        bottomIdx--;
      }
    }
    //---------------------------------------------------------------------------
    private void randomizeListToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (listBox.Items.Count == 0)
        return;

      ArrayList sl = new ArrayList();
      ArrayList cs = new ArrayList();
      Random r = new Random();

      int newPlayIndex = -1;
      int newTargetIndex = -1;
      int newSelectedIndex = -1;

      while (listBox.Items.Count > 0)
      {
        int idx = r.Next(listBox.Items.Count);
        sl.Add(listBox.Items[idx].ToString());
        cs.Add(listBox.GetItemCheckState(idx));

        if (newPlayIndex == -1 && playIndex == idx)
          newPlayIndex = cs.Count - 1;
        if (newTargetIndex == -1 && targetIndex == idx)
          newTargetIndex = cs.Count - 1;
        if (newSelectedIndex == -1 && listBox.SelectedIndex == idx)
          newSelectedIndex = cs.Count - 1;

        listBox.Items.RemoveAt(idx);
      }

      for (int ii = 0; ii < sl.Count && ii < cs.Count; ii++)
        listBox.Items.Add(sl[ii].ToString(), (CheckState)cs[ii]);

      nextIndex = -1;
      targetIndex = newTargetIndex;
      playIndex = newPlayIndex;
      listBox.SelectedIndex = newSelectedIndex;

      listBox.TopIndex = listBox.SelectedIndex;
    }
    //---------------------------------------------------------------------------
    private void alphabatizeListMenuItem_Click(object sender, EventArgs e)
    {
      listBox.Sorted = true;  // Call Sort() method
      listBox.Sorted = false;
    }
    //---------------------------------------------------------------------------
    private void uncheckAllToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Stop();

      // set all items to "unqueued"
      for (int i = 0; i < listBox.Items.Count; i++)
        SetCheck(this, i, CheckState.Unchecked);

      playIndex = -1;
      targetIndex = -1;
      nextIndex = -1;
      listBox.SelectedIndex = -1;
    }
    //---------------------------------------------------------------------------
    private void removeOddToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Stop();

      // Delete all Odd-numbered items...
      for (int ii = listBox.Items.Count - 1; ii >= 0; ii--)
        if ((ii & 1) != 0)
          f1.DeleteItem(ii, this);
    }
    //---------------------------------------------------------------------------
    private void removeEvenToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Stop();

      // Delete all Even-numbered items...
      for (int ii = listBox.Items.Count - 1; ii >= 0; ii--)
        if ((ii & 1) == 0)
          f1.DeleteItem(ii, this);
    }
    //---------------------------------------------------------------------------
    private void removeFromListToolStripMenuItem_Click(object sender, EventArgs e)
    {
      DoRemove(listBox.SelectedIndex, this, false);
    }
    //---------------------------------------------------------------------------
    private void cutToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (InRange(this, dragSourceIndex) &&
        listBox.GetItemCheckState(dragSourceIndex) != CheckState.Checked)
      {
        Clipboard.SetText(listBox.Items[dragSourceIndex].ToString());

        DoRemove(dragSourceIndex, this, false);
      }
    }
    //---------------------------------------------------------------------------
    private void copyToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (InRange(this, dragSourceIndex))
        Clipboard.SetText(listBox.Items[dragSourceIndex].ToString());
    }
    //---------------------------------------------------------------------------
    private bool InRange(FormPlaylist f, int idx)
    {
      return (f != null && f.listBox.Items.Count != 0 &&
        idx >= 0 && idx < f.listBox.Items.Count);
    }
    //---------------------------------------------------------------------------
    private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
    {
      try
      {
        if (Clipboard.ContainsData(DataFormats.FileDrop))
        {
          string[] sa = Clipboard.GetData(DataFormats.FileDrop) as string[];

          if (sa.Length == 0)
            return;

          List<string> filePaths = new List<string>();
          filePaths.AddRange(sa);

          filePaths.Reverse(0, filePaths.Count); // Reverse order

          int idx = listBox.IndexFromPoint(mousePoint); // paste index

          if (!InRange(this, idx))
            idx = listBox.Items.Count;

          bool bSourcePlaying = false;

          if (InRange(this, idx) && listBox.GetItemCheckState(idx) == CheckState.Checked)
            // Paste above playing song?
            bSourcePlaying = true;

          foreach (string fileLoc in filePaths)
          {
            // Code to read the contents of the text file
            if (Directory.Exists(fileLoc))
            {
              // Get files and subdir files - even shortcuts are expanded...
              List<string> allfiles = new List<string>();
              allfiles = GetAllFilesRecurse(fileLoc);

              // Reverse unless list is empty or paste at end
              if (idx >= 0)
                allfiles.Reverse(0, allfiles.Count); // Reverse order

              //Add files from folder
              foreach (string s in allfiles)
                AddSongToPlaylist(idx, this, s, bSourcePlaying, true);
            }
            else if (System.IO.File.Exists(fileLoc))
              AddSongToPlaylist(idx, this, fileLoc, bSourcePlaying, true);
          }
        }
        else if (Clipboard.ContainsText())
        {
          string input = Clipboard.GetText();

          // Split string of cr/lf into a list of strings...
          List<string> list = new List<string>(input.Split(new string[] { "\r\n" },
                         StringSplitOptions.RemoveEmptyEntries));

          if (list.Count == 0)
          {
            MessageBox.Show("Clipboard has no items...");
            return;
          }

          int idx = listBox.IndexFromPoint(mousePoint); // paste index
          if (!InRange(this, idx))
            idx = listBox.Items.Count;

          bool bSourcePlaying = false;

          if (InRange(this, idx) && listBox.GetItemCheckState(idx) == CheckState.Checked)
            // Paste above playing song?
            bSourcePlaying = true;

          foreach (string s in list)
            AddSongToPlaylist(idx, this, s, bSourcePlaying, true);
        }
      }
      catch { return; }
    }
    //---------------------------------------------------------------------------
    private void songInfoToolStripMenuItem_Click(object sender, EventArgs e)
    //
    // Returns a list of KeyValuePair objects with name:value ofsong attributes
    //
    //  Title
    //  Author
    //  Description
    //  Bitrate
    //  Copyright
    //  CopyrightURL
    //  Duration
    //  FileSize
    //  Is_Protected
    //  WM/AlbumArtist
    //  WM/AlbumTitle
    //  WM/Genre
    //  WM/Lyrics
    //  WM/Publisher
    //  WM/TrackNumber
    //  WM/BeatsPerMinute
    //  WM/ContentDistributor
    //  WM/AlbumSortOrder
    //  WM/AudioFileURL
    //  WM/Provider
    {
      if (listBox.SelectedItems.Count <= 0)
      {
        MessageBox.Show("(No Items selected!)");
        return;
      }

      string file = listBox.SelectedItem.ToString();

      // Get the path of the currently selected song...
      if (!System.IO.File.Exists(file))
        return;

      List<string> s = new List<string>();

      // Using new library now... could still access this in WinMediaLib.dll, FYI
      //        if (Path.GetExtension(file).ToLower() == ".wma")
      //        {
      //          List<KeyValuePair<string, string>> kvl = new List<KeyValuePair<string, string>>();
      //
      //          try
      //          {
      //            MediaTags.MediaTags mtr = new MediaTags.MediaTags();
      //
      //            kvl = mtr.GetSongAttributesWMA(file);
      //
      //            foreach (KeyValuePair<string, string> kv in kvl)
      //              s.Add(kv.Key + ": " + kv.Value); //Environment.NewLine
      //          }
      //          catch
      //          {
      //          }
      //        }
      //        else
      //        {
      try
      {
        var mtr = new MediaTags.MediaTags();
        SongInfo ti = mtr.Read(file);

        s.Add("Song info:");

        if (ti.Artist != "") s.Add("Artist: " + ti.Artist);
        if (ti.Album != "") s.Add("Album: " + ti.Album);
        if (ti.Title != "") s.Add("Title: " + ti.Title);
        if (ti.Performer != "") s.Add("Performer: " + ti.Performer);
        if (ti.Genre != "") s.Add("Genre: " + ti.Genre);
        if (ti.Publisher != "") s.Add("Publisher: " + ti.Publisher);
        if (ti.Composer != "") s.Add("Composer: " + ti.Composer);
        if (ti.Conductor != "") s.Add("Conductor: " + ti.Conductor);
        if (ti.FileSize > 0) s.Add("File Size: " + ti.FileSize);
        if (ti.Track != "") s.Add("Track: " + ti.Track);
        if (ti.Duration.TotalSeconds > 0) s.Add("Duration: " + ti.Duration.ToString("hh':'mm':'ss"));
        if (ti.TrackCount > 0) s.Add("Total Tracks: " + ti.TrackCount);
        if (ti.Year != "") s.Add("Year: " + ti.Year);
        if (ti.Disc > 0) s.Add("Disc: " + ti.Disc);
        if (ti.DiscCount > 0) s.Add("Total Discs: " + ti.DiscCount);
        if (ti.Channels > 0) s.Add("Channels: " + ti.Channels);
        if (ti.Codec != "") s.Add("Codec: " + ti.Codec);
        if (ti.SampleRate > 0) s.Add("Sample Freq.: " + ti.SampleRate);
        if (ti.BitsPerSample > 0) s.Add("Quantization (BPS): " + ti.BitsPerSample);
        if (ti.BitRate > 0) s.Add("Bit-rate: " + ti.BitRate);
        if (ti.Description != "") s.Add("Description: " + ti.Description);
        if (ti.Comments != "") s.Add("Comments: " + ti.Comments);
        if (ti.Lyrics != "") s.Add("Lyrics: " + ti.Lyrics);

        if (ti.bProtected != false) s.Add("Protection: (protected)");
        else s.Add("Protection: (unprotected)");

        s.Add("--------");
      }
      catch { }
      //        }

      if (s.Count == 0) s.Add("(Current song info is not available)"); ;

      string title = file.ToString();
      if (title.Length > 40) title = "..." + title.Substring(title.Length - 40);

      FormInfo f = new FormInfo();
      f.InfoList = s;
      f.TitleStr = title;
      f.Show();
    }
    //---------------------------------------------------------------------------
    private void sizeOnDiskinMegabytesToolStripMenuItem_Click(object sender, EventArgs e)
    {
      // Report the size the list takes on disk

      long size = 0;

      foreach (string s in listBox.Items)
      {
        try
        {
          FileInfo fi = new FileInfo(s);
          size += fi.Length;
        }
        catch { }
      }

      string l;
      if (this == f1.ListA)
        l = "ListA";
      else
        l = "ListB";

      long KB = size / (2 << 9);

      if ((size % (2 << 9)) > 0) // round up (err on the cautious side)
        KB++;

      string t;
      if (KB < 700000 / 2)
        t = "(fits 1/2 of a 700MB CD)";
      else if (KB < 700000)
        t = "(fits 700MB CD)";
      else if (KB < 4700000 / 2)
        t = "(fits 1/2 of a 4.7G DVD)";
      else if (KB < 4700000)
        t = "(fits 4.7G DVD)";
      else
        t = "(need > 4.7G media!)";

      string str = l + " total size: " + KB.ToString("N0") + "KB " + t;
      MessageBox.Show(str);
    }
    //---------------------------------------------------------------------------
    private void findToolStripMenuItem_Click(object sender, EventArgs e)
    {
      FormFind f = new FormFind(this);

      f.ShowDialog();

      if (f.DialogResult == DialogResult.OK)
      {
        findIndex = 0;

        Search();

        if (findIndex < 0)
          MessageBox.Show("No Matches");
        else
        {
          listBox.SelectedIndex = findIndex;
          Queue(listBox.SelectedIndex);
        }
      }
    }
    //---------------------------------------------------------------------------
    private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
    {
      // Dual-function item depending on player's state...
      if (bPauseSong || bPlaySong)
        stopToolStripMenuItem.Text = "Stop";
      else
        stopToolStripMenuItem.Text = "Clear";
    }
    //---------------------------------------------------------------------------
    #endregion

  } // End Class

  #region Code Snippets

  /*
  // This class inherits from CheckedListBox and implements a different 
  // sorting method. Sort will be called by setting the class's Sorted
  // property to True.
  public class SortListBox : CheckedListBox
  {
    public SortListBox() : base()
    {
    }

    // Overrides the parent class Sort to perform a simple
    // bubble sort on the length of the string contained in each item.
    protected override void Sort()
    {
      if (Items.Count > 1)
      {
        bool swapped;
        do
        {
          int counter = Items.Count - 1;
          swapped = false;

          while (counter - 1 > 0)
          {
            // Compare the items' length. 
            if (Items[counter].ToString().Length < Items[counter - 1].ToString().Length)
            {
              // Swap the items.
              object temp = Items[counter];
              Items[counter] = Items[counter - 1];
              Items[counter - 1] = temp;
              swapped = true;
            }
            // Decrement the counter.
            counter -= 1;
          }
        }
        while ((swapped == true));
      }
    }
  } // End Class
  */
  #endregion
}
