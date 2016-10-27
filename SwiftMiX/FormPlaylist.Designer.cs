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
    partial class FormPlaylist
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPlaylist));
            this.listBox = new System.Windows.Forms.CheckedListBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.playToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextTrackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.coToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.removeFromListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.songInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sizeOnDiskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.advancedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllUncheckedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeOddToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeEvenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.flipListMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alphabatizeListMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.randomizeListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.checkAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uncheckAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.PlayerControlTimer = new System.Windows.Forms.Timer(this.components);
            this.PlayerPositionTimer = new System.Windows.Forms.Timer(this.components);
            this.FlashTimer = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox
            // 
            this.listBox.AccessibleDescription = "List of songs for SwiftMiX";
            this.listBox.AccessibleName = "SwiftMiX Song List";
            this.listBox.AccessibleRole = System.Windows.Forms.AccessibleRole.DropList;
            this.listBox.AllowDrop = true;
            this.listBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(207)))), ((int)(((byte)(245)))));
            this.listBox.ContextMenuStrip = this.contextMenuStrip1;
            this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox.FormattingEnabled = true;
            this.listBox.HorizontalScrollbar = true;
            this.listBox.Location = new System.Drawing.Point(0, 0);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(577, 124);
            this.listBox.TabIndex = 0;
            this.listBox.Tag = "-1";
            this.listBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listBox_ItemCheck);
            this.listBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listBox_MouseClick);
            this.listBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBox_DragDrop);
            this.listBox.DragOver += new System.Windows.Forms.DragEventHandler(this.listBox_DragOver);
            this.listBox.Leave += new System.EventHandler(this.listBox_Leave);
            this.listBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBox_MouseDoubleClick);
            this.listBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBox_MouseDown);
            this.listBox.MouseEnter += new System.EventHandler(this.listBox_MouseEnter);
            this.listBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listBox_MouseMove);
            this.listBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listBox_MouseUp);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.pauseToolStripMenuItem,
            this.nextTrackToolStripMenuItem,
            this.toolStripSeparator1,
            this.cutToolStripMenuItem,
            this.coToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripSeparator5,
            this.removeFromListToolStripMenuItem,
            this.toolStripSeparator6,
            this.songInfoToolStripMenuItem,
            this.sizeOnDiskToolStripMenuItem,
            this.findToolStripMenuItem,
            this.toolStripSeparator3,
            this.advancedToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(194, 292);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // playToolStripMenuItem
            // 
            this.playToolStripMenuItem.Name = "playToolStripMenuItem";
            this.playToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F7;
            this.playToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.playToolStripMenuItem.Text = "Play";
            this.playToolStripMenuItem.Click += new System.EventHandler(this.playToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F8;
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.stopToolStripMenuItem.Text = "Clear";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // pauseToolStripMenuItem
            // 
            this.pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
            this.pauseToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F9;
            this.pauseToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.pauseToolStripMenuItem.Text = "Pause";
            this.pauseToolStripMenuItem.Click += new System.EventHandler(this.pauseToolStripMenuItem_Click);
            // 
            // nextTrackToolStripMenuItem
            // 
            this.nextTrackToolStripMenuItem.Name = "nextTrackToolStripMenuItem";
            this.nextTrackToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F10;
            this.nextTrackToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.nextTrackToolStripMenuItem.Text = "Next Track";
            this.nextTrackToolStripMenuItem.Click += new System.EventHandler(this.nextTrackToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(190, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // coToolStripMenuItem
            // 
            this.coToolStripMenuItem.Name = "coToolStripMenuItem";
            this.coToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.coToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.coToolStripMenuItem.Text = "Copy";
            this.coToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(190, 6);
            // 
            // removeFromListToolStripMenuItem
            // 
            this.removeFromListToolStripMenuItem.Name = "removeFromListToolStripMenuItem";
            this.removeFromListToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.removeFromListToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.removeFromListToolStripMenuItem.Text = "Remove From List";
            this.removeFromListToolStripMenuItem.Click += new System.EventHandler(this.removeFromListToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(190, 6);
            // 
            // songInfoToolStripMenuItem
            // 
            this.songInfoToolStripMenuItem.Name = "songInfoToolStripMenuItem";
            this.songInfoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.songInfoToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.songInfoToolStripMenuItem.Text = "Song Info";
            this.songInfoToolStripMenuItem.Click += new System.EventHandler(this.songInfoToolStripMenuItem_Click);
            // 
            // sizeOnDiskToolStripMenuItem
            // 
            this.sizeOnDiskToolStripMenuItem.AutoToolTip = true;
            this.sizeOnDiskToolStripMenuItem.Name = "sizeOnDiskToolStripMenuItem";
            this.sizeOnDiskToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.sizeOnDiskToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.sizeOnDiskToolStripMenuItem.Text = "Size on Disk";
            this.sizeOnDiskToolStripMenuItem.ToolTipText = "Reports the size the list occupies on a disk... (useful for CD burning!)";
            this.sizeOnDiskToolStripMenuItem.Click += new System.EventHandler(this.sizeOnDiskinMegabytesToolStripMenuItem_Click);
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.findToolStripMenuItem.Text = "Find";
            this.findToolStripMenuItem.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(190, 6);
            // 
            // advancedToolStripMenuItem
            // 
            this.advancedToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeAllUncheckedToolStripMenuItem,
            this.removeOddToolStripMenuItem,
            this.removeEvenToolStripMenuItem,
            this.toolStripSeparator2,
            this.flipListMenuItem,
            this.alphabatizeListMenuItem,
            this.randomizeListToolStripMenuItem,
            this.toolStripSeparator7,
            this.checkAllToolStripMenuItem,
            this.uncheckAllToolStripMenuItem,
            this.toolStripSeparator8});
            this.advancedToolStripMenuItem.Name = "advancedToolStripMenuItem";
            this.advancedToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.advancedToolStripMenuItem.Text = "(More)";
            // 
            // removeAllUncheckedToolStripMenuItem
            // 
            this.removeAllUncheckedToolStripMenuItem.Name = "removeAllUncheckedToolStripMenuItem";
            this.removeAllUncheckedToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.Delete)));
            this.removeAllUncheckedToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
            this.removeAllUncheckedToolStripMenuItem.Text = "Remove All Unchecked";
            this.removeAllUncheckedToolStripMenuItem.Click += new System.EventHandler(this.removeAllUncheckedToolStripMenuItem_Click);
            // 
            // removeOddToolStripMenuItem
            // 
            this.removeOddToolStripMenuItem.Name = "removeOddToolStripMenuItem";
            this.removeOddToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
            this.removeOddToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
            this.removeOddToolStripMenuItem.Text = "Remove Odd Indices";
            this.removeOddToolStripMenuItem.Click += new System.EventHandler(this.removeOddToolStripMenuItem_Click);
            // 
            // removeEvenToolStripMenuItem
            // 
            this.removeEvenToolStripMenuItem.Name = "removeEvenToolStripMenuItem";
            this.removeEvenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Delete)));
            this.removeEvenToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
            this.removeEvenToolStripMenuItem.Text = "Remove Even Indices";
            this.removeEvenToolStripMenuItem.Click += new System.EventHandler(this.removeEvenToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(249, 6);
            // 
            // flipListMenuItem
            // 
            this.flipListMenuItem.Name = "flipListMenuItem";
            this.flipListMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.flipListMenuItem.Size = new System.Drawing.Size(252, 22);
            this.flipListMenuItem.Text = "Flip List";
            this.flipListMenuItem.ToolTipText = "Invert (flip) the list upside-down.";
            this.flipListMenuItem.Click += new System.EventHandler(this.flipListMenuItem_Click);
            // 
            // alphabatizeListMenuItem
            // 
            this.alphabatizeListMenuItem.Name = "alphabatizeListMenuItem";
            this.alphabatizeListMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.alphabatizeListMenuItem.Size = new System.Drawing.Size(252, 22);
            this.alphabatizeListMenuItem.Text = "Alphebatize List";
            this.alphabatizeListMenuItem.Click += new System.EventHandler(this.alphabatizeListMenuItem_Click);
            // 
            // randomizeListToolStripMenuItem
            // 
            this.randomizeListToolStripMenuItem.Name = "randomizeListToolStripMenuItem";
            this.randomizeListToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.randomizeListToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
            this.randomizeListToolStripMenuItem.Text = "Randomize List";
            this.randomizeListToolStripMenuItem.Click += new System.EventHandler(this.randomizeListToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(249, 6);
            // 
            // checkAllToolStripMenuItem
            // 
            this.checkAllToolStripMenuItem.Name = "checkAllToolStripMenuItem";
            this.checkAllToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.checkAllToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
            this.checkAllToolStripMenuItem.Text = "Check All";
            this.checkAllToolStripMenuItem.Click += new System.EventHandler(this.checkAllToolStripMenuItem_Click);
            // 
            // uncheckAllToolStripMenuItem
            // 
            this.uncheckAllToolStripMenuItem.Name = "uncheckAllToolStripMenuItem";
            this.uncheckAllToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.uncheckAllToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
            this.uncheckAllToolStripMenuItem.Text = "Uncheck All";
            this.uncheckAllToolStripMenuItem.Click += new System.EventHandler(this.uncheckAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(249, 6);
            // 
            // PlayerControlTimer
            // 
            this.PlayerControlTimer.Tick += new System.EventHandler(this.PlayerControlTimer_Tick);
            // 
            // PlayerPositionTimer
            // 
            this.PlayerPositionTimer.Interval = 1000;
            this.PlayerPositionTimer.Tick += new System.EventHandler(this.PlayerPositionTimer_Tick);
            // 
            // FlashTimer
            // 
            this.FlashTimer.Interval = 500;
            this.FlashTimer.Tick += new System.EventHandler(this.FlashTimer_Tick);
            // 
            // FormPlaylist
            // 
            this.AccessibleDescription = "Window for listing SwiftMiX song files";
            this.AccessibleName = "SwiftMiX List Window";
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(207)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(577, 124);
            this.Controls.Add(this.listBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "FormPlaylist";
            this.ShowInTaskbar = false;
            this.Tag = "";
            this.Text = "Player A";
            this.Activated += new System.EventHandler(this.Form2_Activated);
            this.Deactivate += new System.EventHandler(this.Form2_Deactivate);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form2_KeyDown);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

      //private SortListBox listBox;
      private System.Windows.Forms.CheckedListBox listBox;
      private System.Windows.Forms.Timer PlayerControlTimer;
      private System.Windows.Forms.Timer PlayerPositionTimer;
      private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
      private System.Windows.Forms.Timer FlashTimer;
      private System.Windows.Forms.ToolStripMenuItem playToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem nextTrackToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem removeFromListToolStripMenuItem;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
      private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem coToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
      private System.Windows.Forms.ToolStripMenuItem songInfoToolStripMenuItem;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
      private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem advancedToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem removeAllUncheckedToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem removeOddToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem removeEvenToolStripMenuItem;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
      private System.Windows.Forms.ToolStripMenuItem uncheckAllToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem checkAllToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem flipListMenuItem;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
      private System.Windows.Forms.ToolStripMenuItem sizeOnDiskToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem alphabatizeListMenuItem;
      private System.Windows.Forms.ToolStripMenuItem randomizeListToolStripMenuItem;
    }
}