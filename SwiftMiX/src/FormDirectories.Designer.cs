namespace SwiftMiX
{
  partial class FormDirectories
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDirectories));
            this.imageListTreeView = new System.Windows.Forms.ImageList(this.components);
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeRootMusicFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewPerformerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewComposerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewGenreMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewPublisherMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewConductorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewYearMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewTrackMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewDurationMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewCommentsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewLyricsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewPathMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBoxUrl = new System.Windows.Forms.TextBox();
            this.buttonAddUrl = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageListTreeView
            // 
            this.imageListTreeView.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTreeView.ImageStream")));
            this.imageListTreeView.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListTreeView.Images.SetKeyName(0, "MyComputer.bmp");
            this.imageListTreeView.Images.SetKeyName(1, "FloppyDrive.bmp");
            this.imageListTreeView.Images.SetKeyName(2, "FolderClose.bmp");
            this.imageListTreeView.Images.SetKeyName(3, "FolderOpen.bmp");
            this.imageListTreeView.Images.SetKeyName(4, "File.bmp");
            this.imageListTreeView.Images.SetKeyName(5, "35FLOPPY.ICO");
            this.imageListTreeView.Images.SetKeyName(6, "525FLOP1.ICO");
            this.imageListTreeView.Images.SetKeyName(7, "CDDRIVE.ICO");
            this.imageListTreeView.Images.SetKeyName(8, "DRIVENET.ICO");
            this.imageListTreeView.Images.SetKeyName(9, "ShortcutClosed.bmp");
            this.imageListTreeView.Images.SetKeyName(10, "ScortcutOpen.bmp");
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(857, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeRootMusicFolderToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // changeRootMusicFolderToolStripMenuItem
            // 
            this.changeRootMusicFolderToolStripMenuItem.Name = "changeRootMusicFolderToolStripMenuItem";
            this.changeRootMusicFolderToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.changeRootMusicFolderToolStripMenuItem.Text = "&Change Root Music Folder";
            this.changeRootMusicFolderToolStripMenuItem.Click += new System.EventHandler(this.changeRootMusicFolderToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewPerformerMenuItem,
            this.viewComposerMenuItem,
            this.viewGenreMenuItem,
            this.viewPublisherMenuItem,
            this.viewConductorMenuItem,
            this.viewYearMenuItem,
            this.viewTrackMenuItem,
            this.viewDurationMenuItem,
            this.viewCommentsMenuItem,
            this.viewLyricsMenuItem,
            this.viewPathMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // viewPerformerMenuItem
            // 
            this.viewPerformerMenuItem.CheckOnClick = true;
            this.viewPerformerMenuItem.Name = "viewPerformerMenuItem";
            this.viewPerformerMenuItem.Size = new System.Drawing.Size(133, 22);
            this.viewPerformerMenuItem.Text = "Performer";
            this.viewPerformerMenuItem.Click += new System.EventHandler(this.menuViewItem_Click);
            // 
            // viewComposerMenuItem
            // 
            this.viewComposerMenuItem.CheckOnClick = true;
            this.viewComposerMenuItem.Name = "viewComposerMenuItem";
            this.viewComposerMenuItem.Size = new System.Drawing.Size(133, 22);
            this.viewComposerMenuItem.Text = "Composer";
            this.viewComposerMenuItem.Click += new System.EventHandler(this.menuViewItem_Click);
            // 
            // viewGenreMenuItem
            // 
            this.viewGenreMenuItem.CheckOnClick = true;
            this.viewGenreMenuItem.Name = "viewGenreMenuItem";
            this.viewGenreMenuItem.Size = new System.Drawing.Size(133, 22);
            this.viewGenreMenuItem.Text = "Genre";
            this.viewGenreMenuItem.Click += new System.EventHandler(this.menuViewItem_Click);
            // 
            // viewPublisherMenuItem
            // 
            this.viewPublisherMenuItem.CheckOnClick = true;
            this.viewPublisherMenuItem.Name = "viewPublisherMenuItem";
            this.viewPublisherMenuItem.Size = new System.Drawing.Size(133, 22);
            this.viewPublisherMenuItem.Text = "Publisher";
            this.viewPublisherMenuItem.Click += new System.EventHandler(this.menuViewItem_Click);
            // 
            // viewConductorMenuItem
            // 
            this.viewConductorMenuItem.CheckOnClick = true;
            this.viewConductorMenuItem.Name = "viewConductorMenuItem";
            this.viewConductorMenuItem.Size = new System.Drawing.Size(133, 22);
            this.viewConductorMenuItem.Text = "Conductor";
            this.viewConductorMenuItem.Click += new System.EventHandler(this.menuViewItem_Click);
            // 
            // viewYearMenuItem
            // 
            this.viewYearMenuItem.CheckOnClick = true;
            this.viewYearMenuItem.Name = "viewYearMenuItem";
            this.viewYearMenuItem.Size = new System.Drawing.Size(133, 22);
            this.viewYearMenuItem.Text = "Year";
            this.viewYearMenuItem.Click += new System.EventHandler(this.menuViewItem_Click);
            // 
            // viewTrackMenuItem
            // 
            this.viewTrackMenuItem.CheckOnClick = true;
            this.viewTrackMenuItem.Name = "viewTrackMenuItem";
            this.viewTrackMenuItem.Size = new System.Drawing.Size(133, 22);
            this.viewTrackMenuItem.Text = "Track";
            this.viewTrackMenuItem.Click += new System.EventHandler(this.menuViewItem_Click);
            // 
            // viewDurationMenuItem
            // 
            this.viewDurationMenuItem.CheckOnClick = true;
            this.viewDurationMenuItem.Name = "viewDurationMenuItem";
            this.viewDurationMenuItem.Size = new System.Drawing.Size(133, 22);
            this.viewDurationMenuItem.Text = "Time";
            this.viewDurationMenuItem.Click += new System.EventHandler(this.menuViewItem_Click);
            // 
            // viewCommentsMenuItem
            // 
            this.viewCommentsMenuItem.CheckOnClick = true;
            this.viewCommentsMenuItem.Name = "viewCommentsMenuItem";
            this.viewCommentsMenuItem.Size = new System.Drawing.Size(133, 22);
            this.viewCommentsMenuItem.Text = "Comments";
            this.viewCommentsMenuItem.Click += new System.EventHandler(this.menuViewItem_Click);
            // 
            // viewLyricsMenuItem
            // 
            this.viewLyricsMenuItem.CheckOnClick = true;
            this.viewLyricsMenuItem.Name = "viewLyricsMenuItem";
            this.viewLyricsMenuItem.Size = new System.Drawing.Size(133, 22);
            this.viewLyricsMenuItem.Text = "Lyrics";
            this.viewLyricsMenuItem.Click += new System.EventHandler(this.menuViewItem_Click);
            // 
            // viewPathMenuItem
            // 
            this.viewPathMenuItem.CheckOnClick = true;
            this.viewPathMenuItem.Name = "viewPathMenuItem";
            this.viewPathMenuItem.Size = new System.Drawing.Size(133, 22);
            this.viewPathMenuItem.Text = "Path";
            this.viewPathMenuItem.Click += new System.EventHandler(this.menuViewItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // textBoxUrl
            // 
            this.textBoxUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxUrl.Location = new System.Drawing.Point(202, 1);
            this.textBoxUrl.Name = "textBoxUrl";
            this.textBoxUrl.Size = new System.Drawing.Size(643, 20);
            this.textBoxUrl.TabIndex = 1;
            this.textBoxUrl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxUrl_KeyDown);
            // 
            // buttonAddUrl
            // 
            this.buttonAddUrl.Location = new System.Drawing.Point(149, 1);
            this.buttonAddUrl.Name = "buttonAddUrl";
            this.buttonAddUrl.Size = new System.Drawing.Size(47, 20);
            this.buttonAddUrl.TabIndex = 2;
            this.buttonAddUrl.Text = "Add";
            this.buttonAddUrl.UseVisualStyleBackColor = true;
            this.buttonAddUrl.Click += new System.EventHandler(this.buttonAddUrl_Click);
            // 
            // FormDirectories
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(857, 378);
            this.Controls.Add(this.buttonAddUrl);
            this.Controls.Add(this.textBoxUrl);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormDirectories";
            this.Text = "FormDirectories";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDirectories_FormClosing);
            this.Load += new System.EventHandler(this.FormDirectories_Load);
            this.Shown += new System.EventHandler(this.FormDirectories_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormDirectories_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ImageList imageListTreeView;
    private System.Windows.Forms.ToolTip toolTips;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewPerformerMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewComposerMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewGenreMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewPublisherMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewConductorMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewYearMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewTrackMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewDurationMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewCommentsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewLyricsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewPathMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeRootMusicFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.TextBox textBoxUrl;
        private System.Windows.Forms.Button buttonAddUrl;
    }
}