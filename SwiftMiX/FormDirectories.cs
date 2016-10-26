using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Management;
using System.Windows.Forms;
using ImageComboBox;
using MediaTags;
using IWshRuntimeLibrary; // "Process" needs this... used to access shortcut file-paths...
using Microsoft.Win32; // used for registry access...
using System.Net;

namespace SwiftMiX
{
    public interface IMainForm
    {
        List<string> FileFilterList { get; set; }
    }

    #region TreeTagInfo Struct

    public struct TreeTagInfo
    {
        // t_shortcut is a .lnk file, t_scTarget is the directory
        // pointed to by a .lnk shortcut...
        public enum TagTypes
        {
            t_root, t_drive, t_special, t_directory,
            t_shortcut, t_scTarget, t_file
        };

        private TagTypes tagType;
        public TagTypes TagType
        {
            get { return tagType; }
            set { tagType = value; }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private int fileCount; // Not set
        public int FileCount
        {
            get { return fileCount; }
            set { fileCount = value; }
        }
    };
    //public class TreeTagInfo
    //{
    //  // t_shortcut is a .lnk file, t_scTarget is the directory
    //  // pointed to by a .lnk shortcut...
    //  public enum TagTypes
    //  {
    //    t_root, t_drive, t_special, t_directory,
    //    t_shortcut, t_scTarget, t_file
    //  };

    //  private TagTypes tagType = TagTypes.t_root;
    //  public TagTypes TagType
    //  {
    //    get { return tagType; }
    //    set { tagType = value; }
    //  }

    //  private string name = String.Empty;
    //  public string Name
    //  {
    //    get { return name; }
    //    set { name = value; }
    //  }

    //  private int fileCount = -1; // Not set
    //  public int FileCount
    //  {
    //    get { return fileCount; }
    //    set { fileCount = value; }
    //  }
    //};
    //---------------------------------------------------------------------------
    #endregion

    public partial class FormDirectories : Form
    {
        #region Constants

        internal const string SWIFTMIX = "SwiftMiX";

        internal const string REGKEY = @"Software\Discrete-Time Systems\SwiftMiX";

        internal const int LV_FIELD_COUNT = 14; // # of fields in SongView

        // Check the uint sFieldVisible to see which fields to show Title Artist and Album are always visible
        // the integer assignments below also represent the actual flag-bit in sFieldVisible
        internal const int LV_FIELD_TITLE = 0;
        internal const int LV_FIELD_ARTIST = 1;
        internal const int LV_FIELD_ALBUM = 2;
        internal const int LV_FIELD_PERFORMER = 3;
        internal const int LV_FIELD_COMPOSER = 4;
        internal const int LV_FIELD_GENRE = 5;
        internal const int LV_FIELD_PUBLISHER = 6;
        internal const int LV_FIELD_CONDUCTOR = 7;
        internal const int LV_FIELD_YEAR = 8;
        internal const int LV_FIELD_TRACK = 9;
        internal const int LV_FIELD_DURATION = 10;
        internal const int LV_FIELD_COMMENTS = 11;
        internal const int LV_FIELD_LYRICS = 12;
        internal const int LV_FIELD_PATH = 13;

        const int IMAGE_MY_COMPUTER = 0;
        const int IMAGE_FILE_CLOSED = 2;
        const int IMAGE_FILE_OPEN = 3;
        const int IMAGE_REMOVABLE_DRIVE = 5;
        const int IMAGE_LOCAL_DRIVE = 6;
        const int IMAGE_OPTICAL_DRIVE = 7;
        const int IMAGE_NETWORK_DRIVE = 8;
        const int IMAGE_SHORTCUT_CLOSED = 9;
        const int IMAGE_SHORTCUT_OPEN = 10;

        const int IMAGE_DEFAULT = IMAGE_FILE_OPEN;

        public static readonly Color DEFAULT_COLOR = Color.FromArgb(184, 207, 245);
        public static readonly Color DEFAULT_BORDER_COLOR = Color.FromArgb(171, 194, 240);

        #endregion

        #region Variables

        private bool g_bRootFolderHasBeenSet;

        // These flags are used when we load songView with info and one of these fields was
        // empty and we are able to fill it out by parsing the file-path. We need a flag to
        // determine what color to display in the field.
        private bool g_pathTitle;
        private bool g_pathArtist;
        private bool g_pathAlbum;

        private FormPlaylist playListFormA, playListFormB;
        private FormMain parentForm; // Parent object...

        private Button buttonDirUp, buttonDirDn, buttonAddRight, buttonClear;
        private Button buttonAddA, buttonAddB, buttonClose, buttonWAV;
        private ImageComboBox.ImageComboBox comboBox1;
        private Panel buttonPanel;
        private Panel statusPanel1, statusPanel2, statusPanel3;
        private TableLayoutPanel rootPanel;
        private SplitContainer splitContainer;
        private Label labelSetRootFolder;
        private ListView songView, fileView;
        //private RelativeDirectory rd;
        private FormStatus fs;
        private TreeView treeView;
        private ContextMenu songViewContextMenu;
        private ContextMenu fileViewContextMenu;
        //private ContextMenu treeViewContextMenu;
        private Control focusedControl;

        // Used for splitter with buttons code...
        private float originalWidth;
        private int splitPointX;
        private int globalCount;

        // Used dragging an item out to another app
        private Rectangle dragBox;
        private List<string> dragSourcePaths;

        // Save previously selected node so we can collapse it
        private TreeNode nodePrevious = null;

        private ListViewColumnSorter lvwColumnSorter = null;

        #endregion

        #region Properties

        // Here the calling program sets the file-filters
        // such as .mp3|.wma|.wav
        private List<string> g_fileFilterList;
        public List<string> FileFilterList
        {
            get { return g_fileFilterList; }
            set { g_fileFilterList = value; }
        }

        // Here the calling program sets the file-extensions
        // we don't want to display in the fileList
        // such as .db|.ini|.jpg
        private List<string> g_excludeFilterList;
        public List<string> ExcludeFilterList
        {
            get { return g_excludeFilterList; }
            set { g_excludeFilterList = value; }
        }

        private string g_rootTitle;
        public string RootTitle
        {
            get { return g_rootTitle; }
            set { g_rootTitle = value; }
        }

        private string g_rootFolder;
        public string RootFolder
        {
            get { return g_rootFolder; }
            set { g_rootFolder = value; }
        }

        private int g_keyPressed = -1;
        public int KeyPressed // Calling Form should poll this...
        {
            get { return g_keyPressed; }
        }

        private int g_fieldVisible = FormMain.FIELD_VISIBLE;
        public int FieldVisible // Calling Form should poll this...
        {
            get { return g_fieldVisible; }
            set { g_fieldVisible = value; }
        }

        // holds our size!
        private Size g_pFormDirectoriesSize = new Size(FormMain.FDSIZE_X, FormMain.FDSIZE_Y);
        public Size FormDirectoriesSize
        {
            get { return g_pFormDirectoriesSize; }
            set { g_pFormDirectoriesSize = value; }
        }

        public string TitleBar
        {
            set { Text = value; }
        }
        #endregion

        #region Constructor and load

        public FormDirectories(FormMain ParentForm, FormPlaylist PlayListFormA, FormPlaylist PlayListFormB)
        {
            InitializeComponent();

            // Create containers
            rootPanel = new TableLayoutPanel();
            splitContainer = new SplitContainer();

            // Create TreeView and ListView controls
            treeView = new TreeView();
            songView = new ListView();
            fileView = new ListView();
            comboBox1 = new ImageComboBox.ImageComboBox();
            songViewContextMenu = new ContextMenu();
            fileViewContextMenu = new ContextMenu();
            //treeViewContextMenu = new ContextMenu();

            // Create buttonPanel and buttons
            buttonPanel = new Panel();
            statusPanel1 = new Panel();
            statusPanel2 = new Panel();
            statusPanel3 = new Panel();
            buttonDirUp = new Button();
            buttonDirDn = new Button();
            buttonAddRight = new Button();
            buttonAddA = new Button();
            buttonAddB = new Button();
            buttonClose = new Button();
            buttonClear = new Button();
            buttonWAV = new Button();
            labelSetRootFolder = new Label();

            //DialogResult = DialogResult.Cancel; // value to return...

            // Create a RelativeDirectory (custom) class
            // With this the user can click-right to move up one level
            // (or click a button...?)
            //rd = new RelativeDirectory(f.RootFolder);

            playListFormA = PlayListFormA; // Form2
            playListFormB = PlayListFormB; // Form2
            parentForm = ParentForm; // FileList

            fs = new FormStatus(); // Displays progress and a wait message

            // Create an instance of a ListView column sorter and assign it 
            // to the ListView control.
            lvwColumnSorter = new ListViewColumnSorter();
            dragSourcePaths = new List<string>();
        }
        //---------------------------------------------------------------------------
        private void FormDirectories_Load(object sender, EventArgs e)
        {
            PanelsInit();
            ToolTipsInit();
            ContextMenusInit();
            FileViewInit();
            SongViewInit();

            //This did not work because of having to add the event handler
            //delegates in a thread... not cool...
            //Task populateDrives = new Task(delegate { PopulateDriveList(); });
            //Then... Populate TreeView with Drive list in the background
            //populateDrives.ContinueWith(delegate { SetTreeViewToSelectedComboBoxItem(); });
            //populateDrives.Start();

            // Get Drives-list
            PopulateDriveList();
        }
        //---------------------------------------------------------------------------
        private void FormDirectories_Shown(object sender, EventArgs e)
        {
            // set our size...
            if (Left + g_pFormDirectoriesSize.Width > Screen.PrimaryScreen.WorkingArea.Width)
                g_pFormDirectoriesSize.Width = Screen.PrimaryScreen.WorkingArea.Width - Left;
            if (Top + g_pFormDirectoriesSize.Height > Screen.PrimaryScreen.WorkingArea.Height)
                g_pFormDirectoriesSize.Height = Screen.PrimaryScreen.WorkingArea.Height - Top;
            Size = g_pFormDirectoriesSize;

            // Show the filter-list in the title-bar
            if (g_fileFilterList.Count > 0)
            {
                string s = "(Filters: ";
                foreach (string ext in g_fileFilterList)
                    s += ext + ", ";

                if (s.Length > 1)
                    s = s.Substring(0, s.Length - 2); // Remove last comma-space

                this.Text += s + ")";
            }

            SetTreeViewToSelectedComboBoxItem();

            SetMenuChecksFromFieldVisible(); // Set the default view-menu checkmarks
        }
        //---------------------------------------------------------------------------
        #endregion

        #region Control Initialization

        private void PanelsInit()
        {
            rootPanel.SuspendLayout();
            splitContainer.SuspendLayout();
            SuspendLayout();

            // Set all the properties of controls
            SplitterWithButtonsInit();
            SplitContainerInit();
            TreeAndListViewInit();

            // Set up the top status panels
            statusPanel1.Dock = DockStyle.Fill;
            statusPanel1.BackColor = DEFAULT_COLOR;

            // Button is a panel
            statusPanel2.Dock = DockStyle.None;
            statusPanel2.Width = buttonPanel.Width; ;
            statusPanel2.BackColor = DEFAULT_COLOR;
            statusPanel2.BorderStyle = BorderStyle.FixedSingle;

            labelSetRootFolder.Dock = DockStyle.Fill;
            labelSetRootFolder.TextAlign = ContentAlignment.MiddleCenter;
            labelSetRootFolder.AutoSize = false;
            labelSetRootFolder.BorderStyle = BorderStyle.None;
            labelSetRootFolder.UseCompatibleTextRendering = true;
            labelSetRootFolder.Font = new Font("Courier New", 9, buttonDirUp.Font.Style | FontStyle.Regular);

            if (parentForm.RootFolderHasBeenSet)
            {
                labelSetRootFolder.Text = g_rootTitle; // This is set by main form that created us
                labelSetRootFolder.BackColor = Color.GreenYellow;
                g_bRootFolderHasBeenSet = true;
            }
            else
            {
                labelSetRootFolder.Text = "Click Me!";
                labelSetRootFolder.BackColor = Color.IndianRed;
                g_bRootFolderHasBeenSet = false;
            }

            labelSetRootFolder.Enabled = true;
            labelSetRootFolder.Click += (s, e) => labelSetRootFolder_Click(s, e);
            statusPanel2.Controls.Add(labelSetRootFolder);

            statusPanel3.Dock = DockStyle.Fill;
            statusPanel3.BackColor = DEFAULT_COLOR;

            // Add all the controls
            statusPanel1.Controls.Add(comboBox1);

            splitContainer.Panel1.Controls.Add(treeView);
            splitContainer.Panel2.Controls.Add(fileView);

            rootPanel.Controls.Add(statusPanel1);
            rootPanel.Controls.Add(statusPanel2);
            rootPanel.Controls.Add(statusPanel3);
            rootPanel.Controls.Add(splitContainer);
            rootPanel.Controls.Add(buttonPanel);
            rootPanel.Controls.Add(songView);
            Controls.Add(rootPanel);

            rootPanel.ResumeLayout(false);
            splitContainer.ResumeLayout(false);
            ResumeLayout(false);
        }
        //---------------------------------------------------------------------------
        private void TreeAndListViewInit()
        {
            // Init our directories TreeViewMS
            treeView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            treeView.Dock = DockStyle.Fill;
            treeView.ShowNodeToolTips = true;
            //treeView.CheckBoxes = true;
            treeView.BackColor = SystemColors.Window;
            treeView.AllowDrop = false; // we can drag out but not in
                                        //treeView.ContextMenu = treeViewContextMenu;
            treeView.HideSelection = true; // Don't selection when focus lost...
            treeView.ImageList = this.imageListTreeView;
            treeView.ImageIndex = IMAGE_DEFAULT;
            treeView.AfterExpand += this.treeView_AfterExpand;
            treeView.MouseClick += this.treeView_MouseClick;
            treeView.GotFocus += this.treeView_GotFocus;
            treeView.AfterSelect += this.treeView_SelectedIndexChanged;
            treeView.MouseMove += this.treeView_MouseMove;
            treeView.MouseUp += this.treeView_MouseUp;
            treeView.MouseDown += this.treeView_MouseDown;
            //treeView.NodeMouseClick += this.treeView_NodeMouseClick;
            //treeView.AfterSelect += this.treeView_AfterSelect;
            //treeView.NodeMouseDoubleClick += this.treeView_NodeMouseDoubleClick;
            //treeView.BeforeSelect += this.treeView_BeforeSelect;

            // Init our file ListBox2
            fileView.Dock = DockStyle.Fill;
            fileView.BackColor = SystemColors.Window;
            fileView.FullRowSelect = true;
            fileView.AllowColumnReorder = true;
            fileView.MultiSelect = true;
            fileView.TabIndex = 4;
            fileView.HideSelection = true;
            fileView.AllowDrop = false; // we can drag out but not in
            fileView.View = System.Windows.Forms.View.Details;
            fileView.ContextMenu = fileViewContextMenu;
            fileView.ListViewItemSorter = lvwColumnSorter;
            fileView.MouseDoubleClick += this.fileView_MouseDoubleClick;
            fileView.SelectedIndexChanged += this.fileView_SelectedIndexChanged;
            fileView.MouseMove += this.listView_MouseMove;
            fileView.MouseUp += this.listView_MouseUp;
            fileView.MouseDown += this.listView_MouseDown;
            fileView.GotFocus += this.fileView_GotFocus;
            fileView.ColumnClick += this.fileView_ColumnClick;
            //fileView.MouseClick += this.fileView_MouseClick;

            // Init our songs ListBox
            //songView.Parent = splitContainer.Panel2;
            songView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left;
            songView.BackColor = SystemColors.Window;
            songView.FullRowSelect = true;
            songView.AllowColumnReorder = true;
            songView.MultiSelect = true;
            songView.TabIndex = 4;
            songView.HideSelection = false;
            songView.AllowDrop = true; // we can drag out and in
            songView.View = System.Windows.Forms.View.Details;
            songView.ContextMenu = songViewContextMenu;
            songView.ListViewItemSorter = lvwColumnSorter;
            songView.KeyDown += this.songView_KeyDown;
            songView.SelectedIndexChanged += this.songView_SelectedIndexChanged;
            songView.DragDrop += this.songView_DragDrop;
            songView.DragOver += this.songView_DragOver;
            songView.GotFocus += this.songView_GotFocus;
            songView.ColumnClick += this.songView_ColumnClick;
            // Use same handlers as for fileView below:
            songView.MouseMove += this.listView_MouseMove;
            songView.MouseUp += this.listView_MouseUp;
            songView.MouseDown += this.listView_MouseDown;

            comboBox1.ImageList = imageListTreeView;
            comboBox1.Indent = 10; // # of pixels to indent per level
                                   //comboBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom |
                                   //                         AnchorStyles.Left | AnchorStyles.Right;
            comboBox1.Dock = DockStyle.Fill;
            comboBox1.BackColor = SystemColors.Window;
            comboBox1.SelectedValueChanged += this.comboBox1_SelectedValueChanged;
        }
        //---------------------------------------------------------------------------
        private void SplitContainerInit()
        {
            // You can drag the splitter no nearer than 20 pixels
            // from the left edge of the container.
            splitContainer.Panel1MinSize = 10;
            splitContainer.Panel1.BackColor = System.Drawing.SystemColors.Control;
            splitContainer.Panel1.Name = "TreeView";
            splitContainer.Panel1.Size = new Size(splitContainer.Width / 2, 0);

            splitContainer.Panel2MinSize = 10;
            splitContainer.Panel2.BackColor = System.Drawing.SystemColors.Control;
            splitContainer.Panel2.Name = "ListView";
            splitContainer.Panel2.Size = new Size(splitContainer.Width / 2, 0);

            // Basic SplitContainer properties. 
            // This is a vertical splitter that moves in 10-pixel increments. 
            // This splitter needs no explicit Orientation property because Vertical is the default.
            splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer.ForeColor = System.Drawing.SystemColors.Control;
            splitContainer.BackColor = DEFAULT_COLOR;
            splitContainer.Location = new System.Drawing.Point(0, 0);
            splitContainer.Name = "SplitContainer";
            splitContainer.SplitterDistance = 79;
            // This splitter moves in 10-pixel increments.
            //splitContainer.SplitterIncrement = 10;
            splitContainer.SplitterWidth = 3;
            // splitContainer1 is the first control in the tab order.
            splitContainer.TabIndex = 0;
            //splitContainer.Text = "splitContainer";
            // When the splitter moves, the cursor changes shape.
            //splitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(splitContainer_SplitterMoved);
            //splitContainer.SplitterMoving += new System.Windows.Forms.SplitterCancelEventHandler(splitContainer_SplitterMoving);
        }

        //private void splitContainer1_SplitterMoving(System.Object sender, System.Windows.Forms.SplitterCancelEventArgs e)
        //{
        //  // As the splitter moves, change the cursor type.
        //  Cursor.Current = System.Windows.Forms.Cursors.NoMoveVert;
        //}

        //private void splitContainer1_SplitterMoved(System.Object sender, System.Windows.Forms.SplitterEventArgs e)
        //{
        //  // When the splitter stops moving, change the cursor back to the default.
        //  Cursor.Current = System.Windows.Forms.Cursors.Default;
        //}
        //---------------------------------------------------------------------------
        public void SplitterWithButtonsInit()
        {
            buttonPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            //buttonPanel.BackColor = SystemColors.ControlDark;
            buttonPanel.BackColor = DEFAULT_COLOR;
            buttonPanel.Margin = new Padding(0);
            buttonPanel.MinimumSize = new Size(80, 0);
            buttonPanel.Size = new Size(80, 0);
            buttonPanel.MouseEnter += (s, e) => { this.Cursor = Cursors.VSplit; };
            buttonPanel.MouseLeave += (s, e) => { this.Cursor = Cursors.Default; };

            // Old Font: buttonDirUp.Font.FontFamily
            buttonDirUp.Font = new Font("Courier New", 14,
                                buttonDirUp.Font.Style | FontStyle.Regular);
            buttonDirUp.Text = "\u2191"; // up-arrow
            buttonDirUp.Location = new Point(5, 10);
            buttonDirUp.Size = new Size(20, 30);
            buttonDirUp.UseCompatibleTextRendering = true;
            buttonDirUp.UseVisualStyleBackColor = true;
            buttonDirUp.BackColor = DEFAULT_COLOR;
            //buttonDirUp.AutoSize = true;
            buttonDirUp.Click += (s, e) => DirUpButton_Click(s, e);
            //buttonDirUp.Paint += (s, e) => DirUpButton_Paint(s, e);
            buttonPanel.Controls.Add(buttonDirUp);

            buttonClear.Text = "Clear";
            buttonClear.Location = new Point(14, 63);
            buttonClear.Size = new Size(48, 25);
            buttonClear.UseVisualStyleBackColor = true;
            buttonClear.Enabled = false;
            buttonClear.BackColor = DEFAULT_COLOR;
            buttonClear.Click += (s, e) => buttonClear_Click(s, e);
            buttonPanel.Controls.Add(buttonClear);

            buttonAddRight.Font = new Font(buttonAddRight.Font, FontStyle.Bold);
            buttonAddRight.Text = ">>";
            buttonAddRight.Location = new Point(19, 105);
            buttonAddRight.Size = new Size(38, 25);
            buttonAddRight.UseVisualStyleBackColor = true;
            buttonAddRight.Enabled = false;
            buttonAddRight.BackColor = DEFAULT_COLOR;
            buttonAddRight.Click += (s, e) => buttonAddRight_Click(s, e);
            buttonPanel.Controls.Add(buttonAddRight);

            buttonAddA.Text = "Add A";
            buttonAddA.Location = new Point(14, 148);
            buttonAddA.Size = new Size(48, 25);
            buttonAddA.UseVisualStyleBackColor = true;
            buttonAddA.Enabled = false;
            buttonAddA.BackColor = DEFAULT_COLOR;
            buttonAddA.Click += (s, e) => buttonAddA_Click(s, e);
            buttonPanel.Controls.Add(buttonAddA);

            buttonAddB.Text = "Add B";
            buttonAddB.Location = new Point(14, 175);
            buttonAddB.Size = new Size(48, 25);
            buttonAddB.UseVisualStyleBackColor = true;
            buttonAddB.Enabled = false;
            buttonAddB.BackColor = DEFAULT_COLOR;
            buttonAddB.Click += (s, e) => buttonAddB_Click(s, e);
            buttonPanel.Controls.Add(buttonAddB);

            buttonClose.Text = "Close";
            buttonClose.Location = new Point(14, 216);
            buttonClose.Size = new Size(48, 25);
            buttonClose.UseVisualStyleBackColor = true;
            buttonClose.BackColor = DEFAULT_COLOR;
            buttonClose.Click += (s, e) => buttonClose_Click(s, e);
            buttonPanel.Controls.Add(buttonClose);

            buttonPanel.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    // Capture mouse so that all mouse move messages go to this control
                    (s as Control).Capture = true;

                    // Record original column width
                    originalWidth = rootPanel.ColumnStyles[0].Width;

                    // Record first clicked point
                    // Convert to screen coordinates because this window will be a moving target
                    Point windowPoint = (s as Control).PointToScreen(e.Location);
                    splitPointX = windowPoint.X;
                }
            };

            buttonPanel.MouseMove += (s, e) =>
            {
                if ((s as Control).Capture)
                {
                    Point windowPoint = (s as Control).PointToScreen(e.Location);

                    // Calculate distance of mouse from splitPoint
                    int offset = windowPoint.X - splitPointX;

                    // Apply to originalWidth
                    float newWidth = originalWidth + offset;

                    // Clamp it.
                    // The control in the left pane's MinimumSize.Width would be more appropriate than zero
                    newWidth = Math.Max(0, newWidth);

                    // Update column width
                    if (Math.Abs(newWidth - rootPanel.ColumnStyles[0].Width) >= 1)
                        rootPanel.ColumnStyles[0].Width = newWidth;
                }
            };

            buttonPanel.MouseUp += (s, e) =>
            {
                if (e.Button == MouseButtons.Left && (s as Control).Capture)
                    (s as Control).Capture = false;
            };

            rootPanel.ColumnCount = 3;

            // Notice the use of Absolute here as this will control the 'splitting'
            //new ColumnStyle(SizeType.Absolute, 120F),
            rootPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, this.Width / 2 - buttonPanel.Width / 2));
            // Size of button panel
            rootPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            // Remaining size
            rootPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            rootPanel.Dock = DockStyle.Fill;

            // Adjust the top row's width...
            rootPanel.RowCount = 2;
            rootPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 27f));
            rootPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            rootPanel.BackColor = DEFAULT_BORDER_COLOR;

            buttonClose.Focus();
        }
        //---------------------------------------------------------------------------
        private void ContextMenusInit()
        {
            // songView
            MenuItem removeAllSV = new MenuItem("Remove All");
            removeAllSV.Click += songView_RemoveAllMenuClick;
            songViewContextMenu.MenuItems.Add(removeAllSV);

            MenuItem removeSelectedSV = new MenuItem("Remove Selected");
            removeSelectedSV.Click += songView_RemoveSelectedMenuClick;
            songViewContextMenu.MenuItems.Add(removeSelectedSV);

            MenuItem clearSelectedSV = new MenuItem("Clear Selected");
            clearSelectedSV.Click += songView_ClearSelectedMenuClick;
            songViewContextMenu.MenuItems.Add(clearSelectedSV);

            MenuItem copyToClipboardSV = new MenuItem("Copy All");
            copyToClipboardSV.Click += songView_CopyToClipboardMenuClick;
            songViewContextMenu.MenuItems.Add(copyToClipboardSV);

            MenuItem pasteFromClipboardSV = new MenuItem("Paste");
            pasteFromClipboardSV.Click += songView_PasteFromClipboardMenuClick;
            songViewContextMenu.MenuItems.Add(pasteFromClipboardSV);

            // fileView
            MenuItem addAllFV = new MenuItem("Add All");
            addAllFV.Click += fileView_AddAllMenuClick;
            fileViewContextMenu.MenuItems.Add(addAllFV);

            MenuItem addSelectedFV = new MenuItem("Add Selected");
            addSelectedFV.Click += fileView_AddSelectedMenuClick;
            fileViewContextMenu.MenuItems.Add(addSelectedFV);

            MenuItem clearSelectedFV = new MenuItem("Clear Selected");
            clearSelectedFV.Click += fileView_ClearSelectedMenuClick;
            fileViewContextMenu.MenuItems.Add(clearSelectedFV);

            MenuItem copyToClipboardFV = new MenuItem("Copy All");
            copyToClipboardFV.Click += fileView_CopyToClipboardMenuClick;
            fileViewContextMenu.MenuItems.Add(copyToClipboardFV);

            // treeView
            //MenuItem copyToClipboardTV = new MenuItem("Copy");
            //copyToClipboardTV.Click += treeView_CopyToClipboardMenuClick;
            //treeViewContextMenu.MenuItems.Add(copyToClipboardTV);
        }
        //---------------------------------------------------------------------------
        protected void FileViewInit()
        {
            try
            {
                if (fileView.Focused)
                {
                    if (songView.Items.Count == 0)
                        buttonClear.Enabled = false;

                    buttonAddRight.Enabled = false;
                }

                //init fileView control
                fileView.Clear();       //clear control
                                        //create column header for fileView
                fileView.Columns.Add("Name", 140, System.Windows.Forms.HorizontalAlignment.Left);
                fileView.Columns.Add("Size", 50, System.Windows.Forms.HorizontalAlignment.Right);
                fileView.Columns.Add("Created", 50, System.Windows.Forms.HorizontalAlignment.Left);
                fileView.Columns.Add("Modified", 50, System.Windows.Forms.HorizontalAlignment.Left);
                fileView.Columns.Add("Path", 1000, System.Windows.Forms.HorizontalAlignment.Left);
                //fileView.SmallImageList = imageListTreeView;
                //fileView.Columns[4].ImageIndex = IMAGE_FILE_OPEN; //Path has open-folder icon
            }
            catch
            {
                MessageBox.Show("Error in FileViewInit()");
            }
        }
        //---------------------------------------------------------------------------
        protected void SongViewInit()
        {
            // Check g_fieldVisible to see which fields to show Title Artist and Album are always visible
            // the integer assignments below also represent the actual flag-bit in sFieldVisible
            // LV_FIELD_TITLE = 0;
            // LV_FIELD_ARTIST = 1;
            // LV_FIELD_ALBUM = 2;
            // LV_FIELD_PERFORMER = 3;
            // LV_FIELD_COMPOSER = 4;
            // LV_FIELD_GENRE = 5;
            // LV_FIELD_PUBLISHER = 6;
            // LV_FIELD_CONDUCTOR = 7;
            // LV_FIELD_YEAR = 8;
            // LV_FIELD_TRACK = 9;
            // LV_FIELD_DURATION = 10;
            // LV_FIELD_COMMENTS = 11;
            // LV_FIELD_LYRICS = 12;
            // LV_FIELD_PATH = 13;
            try
            {
                this.songView.ListViewItemSorter = null; // disable sorting

                buttonAddA.Enabled = false;
                buttonAddB.Enabled = false;
                buttonClear.Enabled = false;

                //init songView control
                songView.Clear();       //clear control
                songView.Columns.Add("Song", 140, System.Windows.Forms.HorizontalAlignment.Left);
                songView.Columns.Add("Artist", 110, System.Windows.Forms.HorizontalAlignment.Left);
                songView.Columns.Add("Album", 140, System.Windows.Forms.HorizontalAlignment.Left);

                // Fields the user can choose to be visible or not...
                int iWidth = (g_fieldVisible & (1 << LV_FIELD_PERFORMER)) != 0 ? 110 : 0;
                songView.Columns.Add("Performer", iWidth, System.Windows.Forms.HorizontalAlignment.Left);
                iWidth = (g_fieldVisible & (1 << LV_FIELD_COMPOSER)) != 0 ? 110 : 0;
                songView.Columns.Add("Composer", iWidth, System.Windows.Forms.HorizontalAlignment.Left);
                iWidth = (g_fieldVisible & (1 << LV_FIELD_GENRE)) != 0 ? 110 : 0;
                songView.Columns.Add("Genre", iWidth, System.Windows.Forms.HorizontalAlignment.Left);
                iWidth = (g_fieldVisible & (1 << LV_FIELD_PUBLISHER)) != 0 ? 110 : 0;
                songView.Columns.Add("Publisher", iWidth, System.Windows.Forms.HorizontalAlignment.Left);
                iWidth = (g_fieldVisible & (1 << LV_FIELD_CONDUCTOR)) != 0 ? 110 : 0;
                songView.Columns.Add("Conductor", iWidth, System.Windows.Forms.HorizontalAlignment.Left);
                iWidth = (g_fieldVisible & (1 << LV_FIELD_YEAR)) != 0 ? 50 : 0;
                songView.Columns.Add("Year", iWidth, System.Windows.Forms.HorizontalAlignment.Left);
                iWidth = (g_fieldVisible & (1 << LV_FIELD_TRACK)) != 0 ? 50 : 0;
                songView.Columns.Add("Track", iWidth, System.Windows.Forms.HorizontalAlignment.Left);
                iWidth = (g_fieldVisible & (1 << LV_FIELD_DURATION)) != 0 ? 50 : 0;
                songView.Columns.Add("Time", iWidth, System.Windows.Forms.HorizontalAlignment.Left);
                iWidth = (g_fieldVisible & (1 << LV_FIELD_COMMENTS)) != 0 ? 140 : 0;
                songView.Columns.Add("Comments", iWidth, System.Windows.Forms.HorizontalAlignment.Left);
                iWidth = (g_fieldVisible & (1 << LV_FIELD_LYRICS)) != 0 ? 140 : 0;
                songView.Columns.Add("Lyrics", iWidth, System.Windows.Forms.HorizontalAlignment.Left);
                iWidth = (g_fieldVisible & (1 << LV_FIELD_PATH)) != 0 ? 3000 : 0;
                songView.Columns.Add("Path", iWidth, System.Windows.Forms.HorizontalAlignment.Left);

                songView.UseWaitCursor = false;
                g_keyPressed = -1;
            }
            catch
            {
                MessageBox.Show("Error in SongViewInit()", SWIFTMIX);
            }
        }
        #endregion

        #region ToolTips

        private void ToolTipsInit()
        {
            // Set ToolTips
            //toolTips.ReshowDelay = 1000;
            toolTips.SetToolTip(buttonDirUp,
              "Click to explore other" + Environment.NewLine +
              "disk-drives and directories...");
            toolTips.SetToolTip(buttonAddRight, "Click to ADD Songs");
            toolTips.SetToolTip(buttonAddA, "Click to ADD Songs to Player A's List");
            toolTips.SetToolTip(buttonAddB, "Click to ADD Songs to Player B's List");
            //toolTips.SetToolTip(buttonPanel, "Click left and drag to move");
            toolTips.SetToolTip(splitContainer, "Click left and drag to move");
            toolTips.SetToolTip(labelSetRootFolder, "Click to set your music folder");
            toolTips.SetToolTip(buttonClear, "Click once to remove selected items,\r\n" +
                                               "click again to remove all items (no\r\n" +
                                               "files will be deleted!)");
        }
        //---------------------------------------------------------------------------
        #endregion

        #region Populate Directory And File Lists

        //This procedure populates the TreeView with the Drive list
        private bool PopulateDriveList()
        {
            try
            {
                int imageIndex = 0;

                const int Removable = 2;
                const int LocalDisk = 3;
                const int Network = 4;
                const int CD = 5;
                //const int RAMDrive = 6;

                //this.Cursor = Cursors.WaitCursor;

                //clear ComboBox
                comboBox1.Items.Clear();

                AddComboBoxItem("My Computer", Environment.SpecialFolder.MyComputer, IMAGE_MY_COMPUTER, 0, TreeTagInfo.TagTypes.t_root);

                //Get Drive list
                ManagementObjectCollection queryCollection = getDrives();
                foreach (ManagementObject mo in queryCollection)
                {
                    // Win32_LocicalDisk class...
                    int driveType = int.Parse(mo["DriveType"].ToString());
                    String name = mo["Name"].ToString(); // C:\ Etc.
                    String description = mo["Description"].ToString(); // "OK" Etc.

                    switch (driveType)
                    {
                        case Removable: imageIndex = IMAGE_REMOVABLE_DRIVE; break; //removable drives
                        case LocalDisk: imageIndex = IMAGE_LOCAL_DRIVE; break; //Local drives
                        case CD: imageIndex = IMAGE_OPTICAL_DRIVE; break; //CD rom drives
                        case Network: imageIndex = IMAGE_NETWORK_DRIVE; break; //Network drives
                        default: imageIndex = IMAGE_FILE_CLOSED; break; //defalut to folder
                    }

                    //create new drive item
                    AddComboBoxItem(description + " (" + name + ")", name + "\\", imageIndex, 1, TreeTagInfo.TagTypes.t_drive);
                }

                AddComboBoxItem(g_rootTitle + " (" + Path.GetFileName(g_rootFolder) + ")", g_rootFolder, IMAGE_FILE_CLOSED, 0, TreeTagInfo.TagTypes.t_directory);
                AddComboBoxItem("My Desktop", Environment.SpecialFolder.DesktopDirectory, IMAGE_FILE_CLOSED, 0, TreeTagInfo.TagTypes.t_special);
                AddComboBoxItem("My Documents", Environment.SpecialFolder.MyDocuments, IMAGE_FILE_CLOSED, 0, TreeTagInfo.TagTypes.t_special);
                AddComboBoxItem("My Network", Environment.SpecialFolder.NetworkShortcuts, IMAGE_FILE_CLOSED, 0, TreeTagInfo.TagTypes.t_special);
                AddComboBoxItem("Shared Documents", Environment.SpecialFolder.CommonMusic, IMAGE_FILE_CLOSED, 0, TreeTagInfo.TagTypes.t_special);

                // Drop down to fit items
                comboBox1.DropDownHeight = comboBox1.ItemHeight * comboBox1.Items.Count;
                comboBox1.SelectedIndex = comboBox1.FindString(g_rootTitle, 0);

                //this.Cursor = Cursors.Default;
                FileViewInit(); //clear file's list control
                treeView.Nodes.Clear(); //clear tree
                nodePrevious = null;
            }
            catch
            {
                MessageBox.Show("Error: PopulateDriveList()");
                return false;
            }

            return true;
        }
        //---------------------------------------------------------------------------
        // Overloaded...

        private bool AddComboBoxItem(string label, Environment.SpecialFolder sf, int imgIdx, int indentLevel, TreeTagInfo.TagTypes tagType)
        {
            string path = Environment.GetFolderPath(sf);
            return AddComboBoxItem(label, path, imgIdx, indentLevel, tagType);
        }

        private bool AddComboBoxItem(string label, string path, int imgIdx, int indentLevel, TreeTagInfo.TagTypes tagType)
        {
            try
            {
                ImageComboBoxItem itemComboBox = new ImageComboBoxItem(label);
                itemComboBox.ImageIndex = imgIdx; // My Computer
                itemComboBox.IndentLevel = indentLevel;

                // Add our custom TreeTagInfo Object
                TreeTagInfo tti = new TreeTagInfo();
                tti.Name = path; // My Computer has nothing above it...
                tti.TagType = tagType;
                tti.FileCount = -1;
                itemComboBox.Tag = tti;

                comboBox1.Items.Add(itemComboBox);

                Application.DoEvents(); // Keep API humming along...
            }
            catch { return false; }
            return true;
        }
        //---------------------------------------------------------------------------

        protected ManagementObjectCollection getDrives()
        {
            //get drive collection
            ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT * From Win32_LogicalDisk ");
            ManagementObjectCollection queryCollection = query.Get();

            // Another method - gets just the drives that are ready...
            //foreach (var Drives in Environment.GetLogicalDrives())
            //{
            //  DriveInfo DriveInf = new DriveInfo(Drives);
            //  if (DriveInf.IsReady == true)
            //  {
            //    comboBox1.Items.Add(DriveInf.Name);
            //  }
            //}

            // Still More...
            //ManagementClass diskClass = new ManagementClass("Win32_LogicalDisk");
            //ManagementObjectCollection disks = diskClass.GetInstances(); 
            //foreach (ManagementObject disk in disks) { 
            //    Console.WriteLine("Disk = " + disk["deviceid"]); 

            return queryCollection;
        }
        //---------------------------------------------------------------------------

        protected void PopulateDirectory(TreeNode nodeCurrent, TreeNodeCollection nodeCurrentCollection)
        {
            //populate treeview with folders
            try
            {
                //check path
                TreeTagInfo tti = (TreeTagInfo)nodeCurrent.Tag;
                string fp = tti.Name; // full path is in Tag object's Name member

                if (Directory.Exists(fp))
                {
                    TreeNode nodeDir;
                    int imageIndex = 2;     //unselected image index (folder closed)
                    int selectIndex = 3;    //selected image index (folder open)

                    //populate files, if any .lnk files we have an array of paths
                    string[] shortcutDirectories = PopulateFiles(nodeCurrent);

                    string[] stringDirectories = Directory.GetDirectories(fp);

                    //loop through all directories
                    foreach (string stringDir in stringDirectories)
                    {
                        string stringPathName = GetPathName(stringDir);

                        //create node for directories
                        nodeDir = new TreeNode(stringPathName, imageIndex, selectIndex);

                        tti.Name = stringDir; // Save path in our TagInfo
                        tti.TagType = TreeTagInfo.TagTypes.t_directory;
                        tti.FileCount = -1;
                        nodeDir.Tag = tti;

                        nodeCurrentCollection.Add(nodeDir);
                    }

                    imageIndex = 9;     //unselected image index (shortcut folder closed)
                    selectIndex = 10;   //selected image index (shortcut folder open)

                    //loop through all shortcut directories
                    foreach (string shortcutDir in shortcutDirectories)
                    {
                        string shortcutPathName = GetPathName(shortcutDir);

                        //create node for directories
                        nodeDir = new TreeNode(shortcutPathName, imageIndex, selectIndex);
                        tti.Name = shortcutDir; // Save path in our TagInfo
                        tti.TagType = TreeTagInfo.TagTypes.t_scTarget;
                        tti.FileCount = -1;
                        nodeDir.Tag = tti;

                        nodeCurrentCollection.Add(nodeDir);
                    }
                }
            }
            catch (IOException e) { MessageBox.Show("Error: Drive not ready or directory does not exist: " + e); }
            catch (UnauthorizedAccessException e) { MessageBox.Show("Error: Drive or directory access denied: " + e); }
            catch (Exception e) { MessageBox.Show("Error: " + e); }
        }
        //---------------------------------------------------------------------------

        protected string GetPathName(string stringPath)
        {
            //Get Name of folder
            string[] stringSplit = stringPath.Split('\\');
            int _maxIndex = stringSplit.Length;
            return stringSplit[_maxIndex - 1];
        }
        //---------------------------------------------------------------------------

        protected string getFullPath(string stringPath)
        {
            //Get Full path
            string stringParse = "";
            //remove My Computer from path.
            stringParse = stringPath.Replace(@"My Computer\", "");
            //remove double backslash from path.
            stringParse = stringParse.Replace(@"\\", @"\"); // S.S.

            return stringParse;
        }

        //---------------------------------------------------------------------------

        //        foreach (var file in directoryInfo.GetFiles())
        //        {
        //          string extension = Path.GetExtension(file.Name);

        //          if (extension.ToLower() != ".lnk")
        //            directoryNode.Nodes.Add(new TreeNode(file.Name));
        //          else // Get shortcut's real path and add it if valid
        //          {
        //            //Create a new WshShell Interface
        //            WshShell shell = new WshShell();

        //            //Link the interface to our shortcut
        //            WshShortcut link = (WshShortcut)shell.CreateShortcut(file.FullName);

        ////            string s = file.Name + ", " +
        ////              file.Directory + ", " +
        ////              file.DirectoryName + ", " +
        ////              file.FullName + ", " +
        ////              file.Extension + ", " +
        ////              file.Exists;
        ////            MessageBox.Show(s);
        ////            s = link.FullName + ", " +
        ////              link.TargetPath + ", " +
        ////              link.WorkingDirectory;
        ////            MessageBox.Show(s);

        //            //directoryNode.Nodes.Add(CreateDirectoryNode(directory));
        //            //            directoryNode.Nodes.Add(new TreeNode(link.TargetPath));
        //            if (System.IO.Directory.Exists(link.TargetPath))
        //            {
        //              var rootDirectoryInfo = new DirectoryInfo(link.TargetPath);
        //              directoryNode.Nodes.Add(CreateDirectoryNode(rootDirectoryInfo));
        //            }
        //          }
        protected string[] PopulateFiles(TreeNode nodeCurrent)
        {
            //Populate fileView with files

            //We will convert and return this list of full paths
            //derrived from .lnk "shortcut" files (if any).
            List<string> scPaths = new List<string>();

            string[] lvData = new string[5];

            //clear list
            FileViewInit();

            //check path
            TreeTagInfo tti = (TreeTagInfo)nodeCurrent.Tag;
            string fp = tti.Name;

            if (Directory.Exists(fp) == false) MessageBox.Show("Directory or path " + fp + " does not exist.");
            else
            {
                try
                {
                    string[] stringFiles = Directory.GetFiles(fp);
                    DateTime dtCreateDate, dtModifyDate;
                    Int64 lFileSize = 0;

                    //loop through all files
                    foreach (string stringFile in stringFiles)
                    {
                        string sTemp = stringFile;  // Need to assign to sTemp later so need a temp...

                        FileInfo objFileInfo = new FileInfo(stringFile);

                        string ext = objFileInfo.Extension.ToLower();

                        // Don't want these system-files...
                        bool continueLoop = false;
                        foreach (string s in g_excludeFilterList)
                        {
                            if (ext == s)
                            {
                                continueLoop = true;
                                break;
                            }
                        }
                        if (continueLoop) continue;

                        if (ext == ".lnk")
                        {
                            // Get shortcut's real path and add it if valid
                            //Create a new WshShell Interface
                            WshShell shell = new WshShell();

                            //Link the interface to our shortcut
                            WshShortcut link = (WshShortcut)shell.CreateShortcut(objFileInfo.FullName);

                            //string s = file.Name + ", " + file.Directory + ", " + file.DirectoryName + ", " +
                            //file.FullName + ", " + file.Extension + ", " + file.Exists;
                            //MessageBox.Show(s);
                            //string s = link.FullName + ", " + link.TargetPath + ", " + link.WorkingDirectory;
                            //MessageBox.Show(s);

                            if (Directory.Exists(link.TargetPath) == true)
                            {
                                scPaths.Add(link.TargetPath); // Add to list we will return
                                continue;
                            }

                            if (System.IO.File.Exists(link.TargetPath) == true)
                            {
                                objFileInfo = new FileInfo(link.TargetPath);
                                sTemp = link.TargetPath;
                            }
                        }

                        lFileSize = objFileInfo.Length;
                        dtCreateDate = objFileInfo.CreationTime; //GetCreationTime(stringFileName);
                        dtModifyDate = objFileInfo.LastWriteTime; //GetLastWriteTime(stringFileName);

                        //create fileView data
                        lvData[0] = GetPathName(sTemp);
                        lvData[1] = formatSize(lFileSize);

                        //check if file is in local current day light saving time
                        if (TimeZone.CurrentTimeZone.IsDaylightSavingTime(dtCreateDate) == false)
                            //not in day light saving time adjust time
                            lvData[2] = formatDate(dtCreateDate.AddHours(1));
                        else
                            //is in day light saving time adjust time
                            lvData[2] = formatDate(dtCreateDate);

                        //check if file is in local current day light saving time
                        if (TimeZone.CurrentTimeZone.IsDaylightSavingTime(dtModifyDate) == false)
                            //not in day light saving time adjust time
                            lvData[3] = formatDate(dtModifyDate.AddHours(1));
                        else
                            //not in day light saving time adjust time
                            lvData[3] = formatDate(dtModifyDate);

                        lvData[4] = objFileInfo.FullName; // Filepath

                        //Create actual list item
                        ListViewItem lvItem = new ListViewItem(lvData, 0);

                        // Add our custom TreeTagInfo Object
                        tti.Name = objFileInfo.FullName; // Save path in our TagInfo
                        tti.TagType = TreeTagInfo.TagTypes.t_file;
                        tti.FileCount = -1;
                        lvItem.Tag = tti;
                        fileView.Items.Add(lvItem);
                    }
                }
                catch (IOException e) { MessageBox.Show("Error: Drive not ready or directory does not exist: " + e); }
                catch (UnauthorizedAccessException e) { MessageBox.Show("Error: Drive or directory access denied: " + e); }
                catch (Exception e) { MessageBox.Show("Error: " + e); }
            }
            return scPaths.ToArray();
        }
        //---------------------------------------------------------------------------
        protected string formatDate(DateTime dtDate)
        {
            //Get date and time in short format
            string stringDate = "";

            stringDate = dtDate.ToShortDateString().ToString() + " " + dtDate.ToShortTimeString().ToString();

            return stringDate;
        }
        //---------------------------------------------------------------------------
        protected string formatSize(Int64 lSize)
        {
            //Format number to KB
            string stringSize = "";
            NumberFormatInfo myNfi = new NumberFormatInfo();

            Int64 lKBSize = 0;

            if (lSize < 1024)
            {
                if (lSize == 0)
                {
                    //zero byte
                    stringSize = "0";
                }
                else
                {
                    //less than 1K but not zero byte
                    stringSize = "1";
                }
            }
            else
            {
                //convert to KB
                lKBSize = lSize / 1024;
                //format number with default format
                stringSize = lKBSize.ToString("n", myNfi);
                //remove decimal
                stringSize = stringSize.Replace(".00", "");
            }

            return stringSize + " KB";

        }
        //---------------------------------------------------------------------------
        #endregion

        #region Populate Song List

        private int AddSongsFromTreeView(TreeNode tn)
        {
            int resultCode = 0;

            try
            {
                TreeTagInfo tti = (TreeTagInfo)tn.Tag;

                if (tti.FileCount < 0) // -1 = not set
                {
                    // This could take a long time...
                    // (should have a background troller setting node counts...)
                    this.Cursor = Cursors.WaitCursor;
                    string s = this.Text;
                    this.Text = "(Computing file count... please wait...)";
                    globalCount = 0;
                    FileCount(tti.Name); // sets globalCount...
                    tti.FileCount = globalCount; // Set in the node
                    this.Text = s;
                    this.Cursor = Cursors.Default;
                }

                resultCode = AddSongs(tti.FileCount, tti.Name);
            }
            catch
            {
                resultCode = -1;
            }

            return resultCode;
        }
        //---------------------------------------------------------------------------
        private int AddSongs(int fileCount, string filePath)
        {
            int resultCode = 0;

            if (fileCount == 0)
            {
                resultCode = 1;
                return resultCode; // no items
            }

            try
            {
                if (fileCount > 1000)
                {
                    DialogResult result1 =
                            MessageBox.Show("Attempt to add " +
                                       fileCount.ToString() + " songs..." + Environment.NewLine +
                                "This may take a while." + Environment.NewLine +
                                  "Are you sure?", "SwiftMiX...",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.None,
                    MessageBoxDefaultButton.Button2);

                    if (result1 == DialogResult.No)
                    {
                        resultCode = 2; // User chose NO
                        return resultCode;
                    }
                }

                if (fileCount > 500)
                {
                    fs.SetText("Please wait while the selected files " +
                                "are being loaded and processed..." +
                                Environment.NewLine + Environment.NewLine +
                                "PRESS ANY KEY TO QUIT!");

                    fs.ProgressReset();
                    fs.Show();
                }

                g_keyPressed = -1; // Reset

                // Get the filenames and add to the listView
                songView.BeginUpdate(); // Freeze listbox while we add files

                songView.SelectedItems.Clear();

                try
                {
                    List<string> allfiles = new List<string>();
                    allfiles = GetAllFilesRecurse(filePath);

                    bool bSuppressFileInfo = false;

                    if (allfiles.Count > 500) bSuppressFileInfo = true; // Don't look up info if a lot of files...

                    using (var tagReader = new MediaTags.MediaTags())
                    {
                        for (int ii = 0; ii < allfiles.Count; ii++)
                        {
                            if ((ii % 10) == 0)
                            {
                                fs.ProgressValue = (100 * (ii + 1)) / allfiles.Count;

                                if (this.KeyPressed >= 0 || fs.KeyPressed >= 0)
                                {
                                    resultCode = 3; // User Cancel
                                    break;
                                }

                                Application.DoEvents();
                            }

                            resultCode = AddFileToSongView(tagReader, allfiles[ii], bSuppressFileInfo);

                            if (resultCode != 0)
                            {
                                resultCode = -1; // Error
                                break;
                            }
                        }
                    }

                    fs.Hide();
                    g_keyPressed = -1; // Reset
                    Focus();
                }
                catch
                {
                    resultCode = -2; // Error
                }
                finally
                {
                    songView.EndUpdate();

                    if (songView.Items.Count != 0)
                    {
                        buttonAddA.Enabled = true;
                        buttonAddB.Enabled = true;
                        buttonClear.Enabled = true;
                    }
                }
            }
            catch
            {
                resultCode = -3; // Error
            }

            if (resultCode > 0)
                MessageBox.Show("Add files to list cancelled!");
            else if (resultCode < 0)
                MessageBox.Show("Error occured: " + resultCode.ToString());

            return resultCode;
        }
        //---------------------------------------------------------------------------
        private void RecurseAddNodeCounts(TreeNode treeNode)
        {
            // Print the node.
            TreeTagInfo tti = (TreeTagInfo)treeNode.Tag;
            globalCount += tti.FileCount;

            // Print each node recursively.
            foreach (TreeNode tn in treeNode.Nodes)
            {
                RecurseAddNodeCounts(tn);
            }
        }
        //---------------------------------------------------------------------------
        // Call the procedure using the TreeView.
        private void AddNodeCounts(TreeNode treeNode)
        {
            // Add each node's file-count recursively.
            globalCount = 0;
            TreeNodeCollection nodes = treeNode.Nodes;
            foreach (TreeNode n in nodes)
                RecurseAddNodeCounts(n);
        }
        //---------------------------------------------------------------------------
        private void FileCount(string startLocation)
        {
            FileCountRecurse(startLocation);

            foreach (var file in Directory.GetFiles(startLocation))
            {
                string ext = Path.GetExtension(file).ToLower();

                if (ext == ".lnk")
                {
                    try
                    {
                        // Handle shortcut paths
                        WshShell shell = new WshShell();
                        WshShortcut link = (WshShortcut)shell.CreateShortcut(file);

                        if (Directory.Exists(link.TargetPath))
                            FileCount(link.TargetPath);
                        else
                        {
                            // Check the file's extension against the file-filter list
                            ext = Path.GetExtension(link.TargetPath);

                            int index = 0;

                            if (FileFilterList.Count > 0)
                                index = FileFilterList.FindIndex(item => item.ToLower() == ext);

                            if (index >= 0)
                                globalCount++;
                        }
                    }
                    catch
                    {
                        return;
                    }
                }
                else
                {
                    int index = 0;

                    if (FileFilterList.Count > 0)
                        index = FileFilterList.FindIndex(item => item.ToLower() == ext);

                    if (index >= 0)
                        globalCount++;
                }
            }
        }

        private void FileCountRecurse(string startLocation)
        {
            foreach (var directory in Directory.GetDirectories(startLocation))
            {
                FileCountRecurse(directory);

                try
                {
                    foreach (var file in Directory.GetFiles(directory))
                    {
                        string ext = Path.GetExtension(file).ToLower();

                        if (ext == ".lnk")
                        {
                            try
                            {
                                // Handle shortcut paths
                                WshShell shell = new WshShell();
                                WshShortcut link = (WshShortcut)shell.CreateShortcut(file);

                                if (Directory.Exists(link.TargetPath))
                                    FileCount(link.TargetPath);
                                else
                                {
                                    // Check the file's extension against the file-filter list
                                    ext = Path.GetExtension(link.TargetPath);

                                    int index = 0;

                                    if (FileFilterList.Count > 0)
                                        index = FileFilterList.FindIndex(item => item.ToLower() == ext);

                                    if (index >= 0)
                                        globalCount++;
                                }
                            }
                            catch
                            {
                                return;
                            }
                        }
                        else
                        {
                            int index = 0;

                            if (FileFilterList.Count > 0)
                                index = FileFilterList.FindIndex(item => item.ToLower() == ext);

                            if (index >= 0)
                                globalCount++;
                        }
                    }
                }
                catch
                {
                    return;
                }
            }
        }
        //---------------------------------------------------------------------------
        // Overloaded

        private int AddSongsFromFileView(MouseEventArgs e)
        {
            int resultCode = 0;

            // any selected files, add them to songView
            int cnt = fileView.SelectedItems.Count;

            if (cnt > 0)
                resultCode = AddSongsFromFileView(true); // Add selected items
            else
            {
                songView.SelectedItems.Clear();

                ListViewItem lvi = fileView.GetItemAt(e.X, e.Y);
                using (var tagReader = new MediaTags.MediaTags())
                    resultCode = AddFileToSongView(tagReader, (TreeTagInfo)lvi.Tag);

                if (songView.Items.Count != 0)
                {
                    buttonAddA.Enabled = true;
                    buttonAddB.Enabled = true;
                    buttonClear.Enabled = true;

                    // Make the new items visible...
                    songView.EnsureVisible(songView.Items.Count - 1);
                    songView.Focus();
                }
            }

            return resultCode;
        }

        private int AddSongsFromFileView(bool bSelectedOnly)
        {
            int resultCode = 0;

            songView.SelectedItems.Clear();

            // any selected items, add them to songView
            int cnt;
            if (bSelectedOnly)
                cnt = fileView.SelectedItems.Count;
            else
                cnt = fileView.Items.Count;

            if (cnt == 0)
            {
                resultCode = 1;
                return resultCode; // no items
            }

            if (cnt > 500)
            {
                fs.SetText("Please wait while the selected files " +
                            "are being loaded and processed..." +
                            Environment.NewLine + Environment.NewLine +
                            "PRESS ANY KEY TO QUIT!");

                g_keyPressed = -1;
                fs.ProgressReset();
                fs.Show();

                using (var tagReader = new MediaTags.MediaTags())
                {
                    for (int ii = 0; ii < cnt; ii++)
                    {
                        if ((ii % 10) == 0)
                        {
                            fs.ProgressValue = (100 * (ii + 1)) / cnt;

                            if (this.KeyPressed >= 0 || fs.KeyPressed >= 0)
                            {
                                resultCode = 2; // User Cancel
                                break;
                            }

                            Application.DoEvents();
                        }

                        ListViewItem lvi;

                        if (bSelectedOnly) lvi = fileView.SelectedItems[ii];
                        else lvi = fileView.Items[ii];

                        // Suppress track-info lookup
                        resultCode = AddFileToSongView(tagReader, (TreeTagInfo)lvi.Tag, true);
                    }
                }

                fs.Hide();
            }
            else // Not so many songs, look up track-info
            {
                if (bSelectedOnly)
                {
                    using (var tagReader = new MediaTags.MediaTags())
                    {
                        foreach (ListViewItem lvi in fileView.SelectedItems)
                        {
                            resultCode = AddFileToSongView(tagReader, (TreeTagInfo)lvi.Tag, false);

                            if (resultCode != 0) break;
                        }
                    }
                }
                else
                {
                    using (var tagReader = new MediaTags.MediaTags())
                    {
                        foreach (ListViewItem lvi in fileView.Items)
                        {
                            resultCode = AddFileToSongView(tagReader, (TreeTagInfo)lvi.Tag, false);

                            if (resultCode != 0) break;
                        }
                    }
                }
            }

            if (songView.Items.Count != 0)
            {
                buttonAddA.Enabled = true;
                buttonAddB.Enabled = true;
                buttonClear.Enabled = true;

                // Make the new items visible...
                songView.EnsureVisible(songView.Items.Count - 1);
                songView.Focus();
            }

            return resultCode;
        }
        //---------------------------------------------------------------------------
        // Overloaded...

        private int AddFileToSongView(MediaTags.MediaTags tagReader, TreeTagInfo tti, bool bSuppressTrackInfo = false)
        {
            return AddFileToSongView(tagReader, tti.Name, bSuppressTrackInfo);
        }

        private int AddFileToSongView(MediaTags.MediaTags tagReader, string rPath, bool bSuppressTrackInfo = false)
        {
            if (tagReader == null || String.IsNullOrEmpty(rPath)) return -4;

            try
            {
                // Check the file's extension against the file-filter list
                if (g_fileFilterList.Count > 0)
                {
                    string ext = Path.GetExtension(rPath).ToLower();

                    foreach (string filter in g_fileFilterList)
                        if (ext == filter.ToLower()) return AddSongViewData(tagReader, rPath, bSuppressTrackInfo);
                }
                else
                    return AddSongViewData(tagReader, rPath, bSuppressTrackInfo);
            }
            catch { return -5; }

            return 0;
        }
        //---------------------------------------------------------------------------
        private int AddSongViewData(MediaTags.MediaTags tagReader, string rPath, bool bSuppressTrackInfo = false)
        // If we pass idx >= 0 we replace an existing item in songView...
        {
            if (tagReader == null || !System.IO.File.Exists(rPath)) return -1;

            SongInfo si = tagReader.Read(rPath);
            if (si == null) return -2;

            if (bSuppressTrackInfo)
            {
                SongInfo2 si2 = tagReader.ParsePath(rPath);
                si = new SongInfo();
                if (si == null) return -2;
                si.Title = si2.Title;
                si.Artist = si2.Artist;
                si.Album = si2.Album;
            }
            else
            {
            }

            // Fill in info from the file-path if it's missing
            g_pathTitle = false;
            g_pathArtist = false;
            g_pathAlbum = false;

            try
            {
                if (!si.bTitleTag || (!si.bArtistTag && !si.bPerformerTag) || !si.bAlbumTag)
                {
                    SongInfo2 pathsio = tagReader.ParsePath(rPath);

                    if (!si.bTitleTag)
                    {
                        si.Title = pathsio.Title;
                        if (!string.IsNullOrEmpty(si.Title)) g_pathTitle = true;
                    }

                    if (!si.bArtistTag && !si.bPerformerTag)
                    {
                        si.Artist = pathsio.Artist;
                        if (!string.IsNullOrEmpty(si.Artist)) g_pathArtist = true;
                    }

                    if (!si.bAlbumTag)
                    {
                        si.Album = pathsio.Album;
                        if (!string.IsNullOrEmpty(si.Album)) g_pathAlbum = true;
                    }
                }
            }
            catch { si.bException = true; } // Threw an exception

            string[] lvData = new string[LV_FIELD_COUNT];

            try
            {
                // LV_FIELD_TITLE = 0;
                // LV_FIELD_ARTIST = 1;
                // LV_FIELD_ALBUM = 2;
                // LV_FIELD_PERFORMER = 3;
                // LV_FIELD_COMPOSER = 4;
                // LV_FIELD_GENRE = 5;
                // LV_FIELD_PUBLISHER = 6;
                // LV_FIELD_CONDUCTOR = 7;
                // LV_FIELD_YEAR = 8;
                // LV_FIELD_TRACK = 9;
                // LV_FIELD_DURATION = 10;
                // LV_FIELD_COMMENTS = 11;
                // LV_FIELD_LYRICS = 12;
                // LV_FIELD_PATH = 13;

                //create listview data
                lvData[LV_FIELD_TITLE] = si.Title;
                lvData[LV_FIELD_ARTIST] = si.Artist;
                lvData[LV_FIELD_ALBUM] = si.Album;
                lvData[LV_FIELD_PERFORMER] = si.Performer;
                lvData[LV_FIELD_COMPOSER] = si.Composer;
                lvData[LV_FIELD_GENRE] = si.Genre;
                lvData[LV_FIELD_PUBLISHER] = si.Publisher;
                lvData[LV_FIELD_CONDUCTOR] = si.Conductor;
                lvData[LV_FIELD_YEAR] = si.Year;
                lvData[LV_FIELD_TRACK] = si.Track;

                // this field is read-only! (remove leading 00: and trailing fraction)
                if (si.Duration.TotalSeconds == 0)
                    lvData[LV_FIELD_DURATION] = "*";
                else
                {
                    string s = si.Duration.ToString("g"); // short-format, culture-sensitive
                    while (s.Length > 1 && (s.StartsWith("0") || s.StartsWith(":")))
                        s = s.Remove(0, 1);
                    int pos = s.LastIndexOf(".");
                    if (pos >= 0)
                        s = s.Substring(0, pos);
                    lvData[LV_FIELD_DURATION] = s;
                }

                lvData[LV_FIELD_COMMENTS] = si.Comments;
                lvData[LV_FIELD_LYRICS] = si.Lyrics;
                lvData[LV_FIELD_PATH] = rPath;

                ListViewItem lvi = new ListViewItem(lvData, 0);

                // Set the Tag property for each field to 1 if the actual tag exists in the song-file we read...
                // Also preserves the file-type and "exception thrown" condition in songView Tag properties...
                // (available for later use by the tag-editor or other algorithms). Also sets color black if
                // the actual tag for the item was read or gray if not.
                SetSubitemTags(lvi, si);

                songView.Items.Add(lvi); // Add new item

                return 0;
            }
            catch
            {
                return -3;
            }
        }
        //---------------------------------------------------------------------------
        // Set the Tag property for each field to 1 if the actual tag exists in the song-file we read...
        private bool SetSubitemTags(ListViewItem lvi, SongInfo si)
        {
            try
            {
                lvi.SubItems[LV_FIELD_TITLE].Tag = si.bTitleTag;
                lvi.SubItems[LV_FIELD_ARTIST].Tag = si.bArtistTag;
                lvi.SubItems[LV_FIELD_ALBUM].Tag = si.bAlbumTag;
                lvi.SubItems[LV_FIELD_PERFORMER].Tag = si.bPerformerTag;
                lvi.SubItems[LV_FIELD_COMPOSER].Tag = si.bComposerTag;
                lvi.SubItems[LV_FIELD_GENRE].Tag = si.bGenreTag;
                lvi.SubItems[LV_FIELD_PUBLISHER].Tag = si.bPublisherTag;
                lvi.SubItems[LV_FIELD_CONDUCTOR].Tag = si.bConductorTag;
                lvi.SubItems[LV_FIELD_YEAR].Tag = si.bYearTag;
                lvi.SubItems[LV_FIELD_TRACK].Tag = si.bTrackTag;
                lvi.SubItems[LV_FIELD_DURATION].Tag = si.bDurationTag;
                lvi.SubItems[LV_FIELD_COMMENTS].Tag = si.bCommentsTag;
                lvi.SubItems[LV_FIELD_LYRICS].Tag = si.bLyricsTag;

                // Put the file-type in the Path tag...
                lvi.SubItems[LV_FIELD_PATH].Tag = si.FileType;

                // use main item's Tag to save the duration TimeSpan
                lvi.Tag = si.Duration;

                // Set color red if exception during read or else black or gray
                if (si.bException)
                    lvi.ForeColor = Color.Red;
                else
                {
                    lvi.UseItemStyleForSubItems = false; // Allow subitem colors
                    if (g_pathTitle) lvi.SubItems[LV_FIELD_TITLE].ForeColor = Color.Blue;
                    else lvi.SubItems[LV_FIELD_TITLE].ForeColor = (si.bTitleTag ? Color.Black : Color.Gray);
                    if (g_pathArtist) lvi.SubItems[LV_FIELD_ARTIST].ForeColor = Color.Blue;
                    else lvi.SubItems[LV_FIELD_ARTIST].ForeColor = (si.bArtistTag ? Color.Black : Color.Gray);
                    if (g_pathAlbum) lvi.SubItems[LV_FIELD_ALBUM].ForeColor = Color.Blue;
                    else lvi.SubItems[LV_FIELD_ALBUM].ForeColor = (si.bAlbumTag ? Color.Black : Color.Gray);
                    lvi.SubItems[LV_FIELD_PERFORMER].ForeColor = (si.bPerformerTag ? Color.Black : Color.Gray);
                    lvi.SubItems[LV_FIELD_COMPOSER].ForeColor = (si.bComposerTag ? Color.Black : Color.Gray);
                    lvi.SubItems[LV_FIELD_GENRE].ForeColor = (si.bGenreTag ? Color.Black : Color.Gray);
                    lvi.SubItems[LV_FIELD_PUBLISHER].ForeColor = (si.bPublisherTag ? Color.Black : Color.Gray);
                    lvi.SubItems[LV_FIELD_CONDUCTOR].ForeColor = (si.bConductorTag ? Color.Black : Color.Gray);
                    lvi.SubItems[LV_FIELD_YEAR].ForeColor = (si.bYearTag ? Color.Black : Color.Gray);
                    lvi.SubItems[LV_FIELD_TRACK].ForeColor = (si.bTrackTag ? Color.Black : Color.Gray);
                    lvi.SubItems[LV_FIELD_DURATION].ForeColor = (si.bDurationTag ? Color.Black : Color.Gray);
                    lvi.SubItems[LV_FIELD_COMMENTS].ForeColor = (si.bCommentsTag ? Color.Black : Color.Gray);
                    lvi.SubItems[LV_FIELD_LYRICS].ForeColor = (si.bLyricsTag ? Color.Black : Color.Gray);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        //---------------------------------------------------------------------------
        // Set the Tag property for each field to 1 if the actual tag exists in the song-file we read...
        private bool SetSubitemTags(ListViewItem lvi, SongInfo2 si)
        {
            try
            {
                lvi.SubItems[LV_FIELD_TITLE].Tag = si.bTitleTag;
                lvi.SubItems[LV_FIELD_ARTIST].Tag = si.bArtistTag;
                lvi.SubItems[LV_FIELD_ALBUM].Tag = si.bAlbumTag;
                lvi.SubItems[LV_FIELD_PERFORMER].Tag = si.bPerformerTag;

                // Put the file-type in the Path tag...
                lvi.SubItems[LV_FIELD_PATH].Tag = si.FileType;

                // use main item's Tag to save the duration TimeSpan
                lvi.Tag = si.Duration;

                // Set color red if exception during read or else black or gray
                if (si.bException)
                    lvi.ForeColor = Color.Red;
                else
                {
                    lvi.UseItemStyleForSubItems = false; // Allow subitem colors

                    if (g_pathTitle) lvi.SubItems[LV_FIELD_TITLE].ForeColor = Color.Blue;
                    else lvi.SubItems[LV_FIELD_TITLE].ForeColor = (si.bTitleTag ? Color.Black : Color.Gray);

                    if (g_pathArtist) lvi.SubItems[LV_FIELD_ARTIST].ForeColor = Color.Blue;
                    else lvi.SubItems[LV_FIELD_ARTIST].ForeColor = (si.bArtistTag ? Color.Black : Color.Gray);

                    if (g_pathAlbum) lvi.SubItems[LV_FIELD_ALBUM].ForeColor = Color.Blue;
                    else lvi.SubItems[LV_FIELD_ALBUM].ForeColor = (si.bAlbumTag ? Color.Black : Color.Gray);

                    lvi.SubItems[LV_FIELD_PERFORMER].ForeColor = (si.bPerformerTag ? Color.Black : Color.Gray);
                }
                return true;
            }
            catch
            {
                return false;
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
        #endregion

        #region Misc Routines

        //---------------------------------------------------------------------------
        protected void SongViewRefresh()
        {
            // Check the uint sFieldVisible to see which fields to show Title Artist and Album are always visible
            // the integer assignments below also represent the actual flag-bit in g_fieldVisible
            // LV_FIELD_TITLE = 0;
            // LV_FIELD_ARTIST = 1;
            // LV_FIELD_ALBUM = 2;
            // LV_FIELD_PERFORMER = 3;
            // LV_FIELD_COMPOSER = 4;
            // LV_FIELD_GENRE = 5;
            // LV_FIELD_PUBLISHER = 6;
            // LV_FIELD_CONDUCTOR = 7;
            // LV_FIELD_YEAR = 8;
            // LV_FIELD_TRACK = 9;
            // LV_FIELD_DURATION = 10;
            // LV_FIELD_COMMENTS = 11;
            // LV_FIELD_LYRICS = 12;
            // LV_FIELD_PATH = 13;
            try
            {
                if (songView.Columns.Count != LV_FIELD_COUNT) return;

                //refresh songView control
                songView.Columns[LV_FIELD_TITLE].Width = 140;
                songView.Columns[LV_FIELD_ARTIST].Width = 110;
                songView.Columns[LV_FIELD_ALBUM].Width = 140;

                int iWidth = (g_fieldVisible & (1 << LV_FIELD_PERFORMER)) != 0 ? 110 : 0;
                songView.Columns[LV_FIELD_PERFORMER].Width = iWidth;
                iWidth = (g_fieldVisible & (1 << LV_FIELD_COMPOSER)) != 0 ? 110 : 0;
                songView.Columns[LV_FIELD_COMPOSER].Width = iWidth;
                iWidth = (g_fieldVisible & (1 << LV_FIELD_GENRE)) != 0 ? 110 : 0;
                songView.Columns[LV_FIELD_GENRE].Width = iWidth;
                iWidth = (g_fieldVisible & (1 << LV_FIELD_PUBLISHER)) != 0 ? 110 : 0;
                songView.Columns[LV_FIELD_PUBLISHER].Width = iWidth;
                iWidth = (g_fieldVisible & (1 << LV_FIELD_CONDUCTOR)) != 0 ? 110 : 0;
                songView.Columns[LV_FIELD_CONDUCTOR].Width = iWidth;
                iWidth = (g_fieldVisible & (1 << LV_FIELD_YEAR)) != 0 ? 50 : 0;
                songView.Columns[LV_FIELD_YEAR].Width = iWidth;
                iWidth = (g_fieldVisible & (1 << LV_FIELD_TRACK)) != 0 ? 50 : 0;
                songView.Columns[LV_FIELD_TRACK].Width = iWidth;
                iWidth = (g_fieldVisible & (1 << LV_FIELD_DURATION)) != 0 ? 50 : 0;
                songView.Columns[LV_FIELD_DURATION].Width = iWidth;
                iWidth = (g_fieldVisible & (1 << LV_FIELD_COMMENTS)) != 0 ? 140 : 0;
                songView.Columns[LV_FIELD_COMMENTS].Width = iWidth;
                iWidth = (g_fieldVisible & (1 << LV_FIELD_LYRICS)) != 0 ? 140 : 0;
                songView.Columns[LV_FIELD_LYRICS].Width = iWidth;
                iWidth = (g_fieldVisible & (1 << LV_FIELD_PATH)) != 0 ? 3000 : 0;
                songView.Columns[LV_FIELD_PATH].Width = iWidth;
                songView.Update();
            }
            catch
            {
                MessageBox.Show("Error in SongViewRefresh()", SWIFTMIX);
            }
        }
        //---------------------------------------------------------------------------
        private void SetFieldVisibleFromMenuChecks()
        {
            // Set fieldVisible based on menu checks
            g_fieldVisible = 0;
            g_fieldVisible |= (1 << LV_FIELD_TITLE);
            g_fieldVisible |= (1 << LV_FIELD_ARTIST);
            g_fieldVisible |= (1 << LV_FIELD_ALBUM);
            if (viewPerformerMenuItem.Checked) g_fieldVisible |= (1 << LV_FIELD_PERFORMER);
            if (viewComposerMenuItem.Checked) g_fieldVisible |= (1 << LV_FIELD_COMPOSER);
            if (viewGenreMenuItem.Checked) g_fieldVisible |= (1 << LV_FIELD_GENRE);
            if (viewPublisherMenuItem.Checked) g_fieldVisible |= (1 << LV_FIELD_PUBLISHER);
            if (viewConductorMenuItem.Checked) g_fieldVisible |= (1 << LV_FIELD_CONDUCTOR);
            if (viewYearMenuItem.Checked) g_fieldVisible |= (1 << LV_FIELD_YEAR);
            if (viewTrackMenuItem.Checked) g_fieldVisible |= (1 << LV_FIELD_TRACK);
            if (viewDurationMenuItem.Checked) g_fieldVisible |= (1 << LV_FIELD_DURATION);
            if (viewCommentsMenuItem.Checked) g_fieldVisible |= (1 << LV_FIELD_COMMENTS);
            if (viewLyricsMenuItem.Checked) g_fieldVisible |= (1 << LV_FIELD_LYRICS);
            if (viewPathMenuItem.Checked) g_fieldVisible |= (1 << LV_FIELD_PATH);
            //MessageBox.Show(g_fieldVisible.ToString()); // use this to set Settings initial value
        }
        //---------------------------------------------------------------------------
        private void SetMenuChecksFromFieldVisible()
        {
            // Init the View menu's checkmarks based on flags in fieldVisible
            //MessageBox.Show(g_fieldVisible.ToString()); // use this to set Settings initial value
            viewPerformerMenuItem.Checked = (g_fieldVisible & (1 << LV_FIELD_PERFORMER)) != 0 ? true : false;
            viewComposerMenuItem.Checked = (g_fieldVisible & (1 << LV_FIELD_COMPOSER)) != 0 ? true : false;
            viewGenreMenuItem.Checked = (g_fieldVisible & (1 << LV_FIELD_GENRE)) != 0 ? true : false;
            viewPublisherMenuItem.Checked = (g_fieldVisible & (1 << LV_FIELD_PUBLISHER)) != 0 ? true : false;
            viewConductorMenuItem.Checked = (g_fieldVisible & (1 << LV_FIELD_CONDUCTOR)) != 0 ? true : false;
            viewYearMenuItem.Checked = (g_fieldVisible & (1 << LV_FIELD_YEAR)) != 0 ? true : false;
            viewTrackMenuItem.Checked = (g_fieldVisible & (1 << LV_FIELD_TRACK)) != 0 ? true : false;
            viewDurationMenuItem.Checked = (g_fieldVisible & (1 << LV_FIELD_DURATION)) != 0 ? true : false;
            viewCommentsMenuItem.Checked = (g_fieldVisible & (1 << LV_FIELD_COMMENTS)) != 0 ? true : false;
            viewLyricsMenuItem.Checked = (g_fieldVisible & (1 << LV_FIELD_LYRICS)) != 0 ? true : false;
            viewPathMenuItem.Checked = (g_fieldVisible & (1 << LV_FIELD_PATH)) != 0 ? true : false;
        }
        //---------------------------------------------------------------------------
        private void SetTreeViewToSelectedComboBoxItem()
        {
            FileViewInit(); //clear file's list control

            TreeNode nodeNew =
              new TreeNode(comboBox1.SelectedItem.ToString(), IMAGE_FILE_OPEN, IMAGE_FILE_OPEN);
            nodeNew.Tag = comboBox1.Items[comboBox1.SelectedIndex].Tag;
            nodeNew.ToolTipText = "Click-Left sets a new " + g_rootTitle + " folder." + Environment.NewLine +
                                  "Click-Right adds " + g_rootTitle + " files to song-list.";

            if (comboBox1.SelectedIndex != 0)
            {
                treeView.Nodes.Clear();
                nodePrevious = null;
                treeView.Nodes.Add(nodeNew);
                PopulateDirectory(nodeNew, nodeNew.Nodes);

                nodeNew.Expand(); // Expand it
            }
            else // Pop down comboBox1
                comboBox1.DroppedDown = true;
        }
        //---------------------------------------------------------------------------
        private List<string> GetItemsFromSongViewControl()
        {
            List<string> lviItemsArrayList = new List<string>();
            foreach (ListViewItem lvi in this.songView.Items)
                lviItemsArrayList.Add(lvi.SubItems[LV_FIELD_PATH].Text);
            return lviItemsArrayList;
        }
        //---------------------------------------------------------------------------
        private List<string> GetSelectedItemsFromSongViewControl()
        {
            List<string> lviItemsArrayList = new List<string>();
            foreach (ListViewItem lvi in this.songView.SelectedItems)
                lviItemsArrayList.Add(lvi.SubItems[LV_FIELD_PATH].Text);
            return lviItemsArrayList;
        }
        //---------------------------------------------------------------------------
        private void DeleteSongViewSelItems()
        // Delete values from list-box
        {
            while (songView.SelectedItems.Count > 0)
                songView.Items.Remove(songView.SelectedItems[0]);

            if (songView.Items.Count == 0)
            {
                buttonAddA.Enabled = false;
                buttonAddB.Enabled = false;
                buttonClear.Enabled = false;
            }
        }
        //---------------------------------------------------------------------------
        //private List<string> getCheckedItems()
        //{
        //  List<string> sl = new List<string>(); 

        //  foreach (TreeNode t in treeView.Nodes)
        //  {
        //    if (t.Checked == true) sl.Add(t.Text + Environment.NewLine);

        //    if (t.Nodes.Count > 0)
        //    {
        //      foreach (TreeNode tt in t.Nodes)
        //      {
        //        if (tt.Checked == true)
        //          sl.Add(tt.Text + Environment.NewLine);
        //      }
        //    }
        //  }
        //  return sl;
        //}
        //---------------------------------------------------------------------------
        #endregion

        #region Event Handlers

        private void songView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (songView.Focused)
            {
                if (songView.Items.Count > 0)
                {
                    buttonAddA.Enabled = true;
                    buttonAddB.Enabled = true;
                    buttonClear.Enabled = true;
                }
                else
                {
                    buttonAddA.Enabled = false;
                    buttonAddB.Enabled = false;
                    buttonClear.Enabled = false;
                }
            }
        }
        //---------------------------------------------------------------------------
        private void fileView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fileView.Focused)
            {
                if (fileView.SelectedItems.Count > 0)
                {
                    buttonAddRight.Enabled = true;
                    buttonClear.Enabled = true;
                }
                else
                {
                    buttonAddRight.Enabled = false;
                    buttonClear.Enabled = false;
                }
            }
        }
        //---------------------------------------------------------------------------
        private void songView_KeyDown(object sender, KeyEventArgs e)
        {
            // e.KeyCode is the Keys enumeration value for the key that is down
            // e.KeyData is the same as KeyCode, but combined with any SHIFT/CTRL/ALT keys
            // e.KeyValue is simply an integer representation of KeyCode
            //
            // e.Modifiers

            if (e.KeyCode == Keys.Delete)
                DeleteSongViewSelItems();
        }
        //---------------------------------------------------------------------------
        private void FormDirectories_KeyDown(object sender, KeyEventArgs e)
        {
            g_keyPressed = (int)e.KeyCode;  // Tell background worker to Cancel...
        }
        //---------------------------------------------------------------------------
        private void buttonAddA_Click(object sender, EventArgs e)
        {
            int save = playListFormA.clbCount;

            if (songView.SelectedItems.Count > 0)
                playListFormA.clbAddRange(GetSelectedItemsFromSongViewControl());
            else
                playListFormA.clbAddRange(GetItemsFromSongViewControl());

            if (!playListFormA.IsPlayOrPause()) playListFormA.Queue(save);

            parentForm.SetVolumes();

            // Show the player's listbox
            parentForm.SetPlaylistColor(playListFormA);
            parentForm.ShowPlaylist(playListFormA);

            //SongViewInit(); // Clear local list...
        }
        //---------------------------------------------------------------------------
        private void buttonAddB_Click(object sender, EventArgs e)
        {
            int save = playListFormB.clbCount;

            if (songView.SelectedItems.Count > 0)
                playListFormB.clbAddRange(GetSelectedItemsFromSongViewControl());
            else
                playListFormB.clbAddRange(GetItemsFromSongViewControl());

            if (!playListFormB.IsPlayOrPause()) playListFormB.Queue(save);

            parentForm.SetVolumes();

            // Show the player's listbox
            parentForm.SetPlaylistColor(playListFormB);
            parentForm.ShowPlaylist(playListFormB);

            //SongViewInit(); // Clear local list...
        }
        //---------------------------------------------------------------------------
        private void buttonAddRight_Click(object sender, EventArgs e)
        {
            // any selected items, add them to songView
            if (focusedControl == fileView && fileView.SelectedItems.Count > 0)
            {
                AddSongsFromFileView(null);
                buttonAddRight.Enabled = false;
            }
            else if (focusedControl == treeView && treeView.SelectedNode != null)
            {
                AddSongsFromTreeView(treeView.SelectedNode);
                buttonAddRight.Enabled = false;
            }
        }
        //---------------------------------------------------------------------------
        private void labelSetRootFolder_Click(object sender, EventArgs e)
        {
            // Set new Music folder

            // Initiate a directories dialog
            try
            {
                FolderBrowserDialog browserDialog = new FolderBrowserDialog();

                browserDialog.Description = "Select a " + g_rootTitle + " folder. This will be the" +
                " default folder. Hint: if you have files on different drives," +
                " create shortcuts to them and place them in one folder.";
                browserDialog.ShowNewFolderButton = false;
                browserDialog.SelectedPath = g_rootFolder;

                if (browserDialog.ShowDialog() == DialogResult.OK)
                {
                    g_rootFolder = browserDialog.SelectedPath;
                    g_bRootFolderHasBeenSet = true;
                    labelSetRootFolder.BackColor = Color.GreenYellow;
                    labelSetRootFolder.Text = g_rootTitle;

                    PopulateDriveList();
                    SetTreeViewToSelectedComboBoxItem();
                }
            }
            catch
            {
            }
        }
        //---------------------------------------------------------------------------
        private void treeView_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Don't fire select event while we are in it...
            treeView.AfterSelect -= this.treeView_SelectedIndexChanged;
            this.Cursor = Cursors.WaitCursor;
            treeView.BeginUpdate();

            try
            {
                //Populate folders and files when a folder is selected
                TreeNode nodeCurrent = treeView.SelectedNode;

                // Collapse old
                if (nodeCurrent.Level == 0)
                    nodePrevious = null;

                if (nodePrevious != null && nodeCurrent.Level <= nodePrevious.Level)
                {
                    nodePrevious.Collapse();
                    while (nodePrevious.Parent.Level >= nodeCurrent.Level)
                    {
                        nodePrevious.Parent.Collapse();
                        nodePrevious = nodePrevious.Parent;
                    }
                }

                nodePrevious = nodeCurrent;
                nodeCurrent = RefreshNode(nodeCurrent); // Get first subnode with any files...
                nodePrevious.Expand(); // Expand it
            }
            catch
            {
                MessageBox.Show("Error: treeView_MouseClick()");
            }

            // Enable/disable buttons
            if (treeView.Focused)
            {
                if (treeView.SelectedNode != null)
                    buttonAddRight.Enabled = true;
                else
                    buttonAddRight.Enabled = false;
            }

            treeView.EndUpdate();
            this.Cursor = Cursors.Default;
            treeView.AfterSelect += this.treeView_SelectedIndexChanged;
        }

        //---------------------------------------------------------------------------
        private TreeNode RefreshNode(TreeNode nodeCurrent)
        {
            //clear all sub-nodes
            nodeCurrent.Nodes.Clear();
            PopulateDirectory(nodeCurrent, nodeCurrent.Nodes);

            TreeNode tempNode = nodeCurrent;

            // No files, loop through folders...
            if (fileView.Items.Count == 0 && nodeCurrent.Nodes.Count > 0)
            {
                foreach (TreeNode tn in nodeCurrent.Nodes)
                {
                    tempNode = RefreshNode(tn);

                    //          tempNode.Expand(); // Expand it

                    if (fileView.Items.Count != 0)
                    {
                        tempNode = tn;
                        break;
                    }
                }
            }

            return tempNode;
        }
        //---------------------------------------------------------------------------
        private void fileView_AddAllMenuClick(Object sender, EventArgs e)
        {
            AddSongsFromFileView(false);
        }
        //---------------------------------------------------------------------------
        private void fileView_AddSelectedMenuClick(Object sender, EventArgs e)
        {
            AddSongsFromFileView(true);
        }
        //---------------------------------------------------------------------------
        private void songView_PasteFromClipboardMenuClick(object sender, EventArgs e)
        {
            try
            {
                if (!Clipboard.ContainsData(DataFormats.FileDrop) &&
                                             !Clipboard.ContainsData(DataFormats.Text))
                    return;

                if (Clipboard.ContainsData(DataFormats.FileDrop))
                {
                    string[] filePaths = Clipboard.GetData(DataFormats.FileDrop) as string[];

                    if (filePaths == null || filePaths.Length == 0)
                        return;

                    songView.SelectedItems.Clear();

                    using (var tagReader = new MediaTags.MediaTags())
                    {
                        foreach (string fileLoc in filePaths)
                        {
                            // Code to read the contents of the text file
                            if (Directory.Exists(fileLoc))
                            {
                                // Get files and subdir files - even shortcuts are expanded...
                                List<string> allfiles = new List<string>();
                                allfiles = GetAllFilesRecurse(fileLoc);

                                //Add files from folder
                                foreach (string s in allfiles)
                                {
                                    string ext = Path.GetExtension(s).ToLower();

                                    int index = 0;

                                    if (g_fileFilterList.Count > 0)
                                        index = this.FileFilterList.FindIndex(item => item.ToLower() == ext);

                                    if (index >= 0) AddFileToSongView(tagReader, s);
                                }
                            }
                            else AddFileToSongView(tagReader, fileLoc);
                        }
                    }
                }
                else // We must have Text...
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

                    using (var tagReader = new MediaTags.MediaTags())
                        foreach (string s in list) if (AddFileToSongView(tagReader, s) != 0) break;
                }

                if (songView.Items.Count != 0)
                {
                    buttonAddA.Enabled = true;
                    buttonAddB.Enabled = true;
                    buttonClear.Enabled = true;

                    // Make the new items visible...
                    songView.EnsureVisible(songView.Items.Count - 1);
                    songView.Focus();
                }
            }
            catch
            {
                return;
            }
        }
        //---------------------------------------------------------------------------
        private void fileView_CopyToClipboardMenuClick(object sender, EventArgs e)
        {
            if (this.fileView.Items.Count == 0)
                return;

            try
            {
                Clipboard.Clear();
                StringBuilder buffer = new StringBuilder();

                // Setup the columns
                for (int i = 0; i < this.fileView.Columns.Count; i++)
                {
                    buffer.Append(this.fileView.Columns[i].Text);
                    buffer.Append("\t");
                }
                buffer.Append("\n");

                // Build the data row by row
                for (int i = 0; i < this.fileView.Items.Count; i++)
                {
                    for (int j = 0; j < this.fileView.Columns.Count; j++)
                    {
                        buffer.Append(this.fileView.Items[i].SubItems[j].Text);
                        buffer.Append("\t");
                    }
                    buffer.Append("\n");
                }

                Clipboard.SetText(buffer.ToString());

                MessageBox.Show("Files copied for paste into Excel...");
            }
            catch
            {
            }
        }
        //---------------------------------------------------------------------------
        private void songView_CopyToClipboardMenuClick(object sender, EventArgs e)
        {
            if (this.songView.Items.Count == 0)
                return;

            try
            {
                Clipboard.Clear();
                StringBuilder buffer = new StringBuilder();

                // Setup the columns
                for (int i = 0; i < this.songView.Columns.Count; i++)
                {
                    buffer.Append(this.songView.Columns[i].Text);
                    buffer.Append("\t");
                }
                buffer.Append("\n");

                // Build the data row by row
                for (int i = 0; i < this.songView.Items.Count; i++)
                {
                    for (int j = 0; j < this.songView.Columns.Count; j++)
                    {
                        buffer.Append(this.songView.Items[i].SubItems[j].Text);
                        buffer.Append("\t");
                    }
                    buffer.Append("\n");
                }

                Clipboard.SetText(buffer.ToString());

                MessageBox.Show("Songs copied for paste into Excel...");
            }
            catch
            {
            }
        }
        //---------------------------------------------------------------------------
        private void treeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            //TreeNode tn = e.Node;
            treeView.TopNode.EnsureVisible();
        }
        //---------------------------------------------------------------------------
        private void buttonAddUrl_Click(object sender, EventArgs e)
        {
            string myUri = textBoxUrl.Text;

            if (myUri.ToLower().StartsWith("www."))
                myUri = myUri.Insert(0, "http://");

            if (String.IsNullOrEmpty(myUri)) return;
            // parse off the filename
            //            int len = myUri.Length;
            //            int ii;
            //            for (ii = len - 1; ii >= 0; ii--)
            //                if (myUri[ii] == '\\' || myUri[ii] == '/')
            //                    break;
            //            string fileName = myUri.Substring(ii + 1, len - ii - 1); ;
            //            if (fileName.Length == 0) return;

            // get filename without extension
            string fileName = Path.GetFileNameWithoutExtension(myUri);

            string[] lvData = new string[LV_FIELD_COUNT];
            for (int ii = 0; ii < lvData.Length; ii++)
                lvData[ii] = String.Empty;
            lvData[LV_FIELD_TITLE] = fileName;
            lvData[LV_FIELD_PATH] = myUri;
            ListViewItem lvi = new ListViewItem(lvData, 0);

            songView.Items.Add(lvi); // Add new item (path only)

            //            songView.SelectedItems.Clear();
            songView.Items[songView.Items.Count - 1].Selected = true;

            if (songView.Items.Count != 0)
            {
                buttonAddA.Enabled = true;
                buttonAddB.Enabled = true;
                buttonClear.Enabled = true;

                // Make the new items visible...
                songView.EnsureVisible(songView.Items.Count - 1);
                songView.Focus();
            }
        }
        //---------------------------------------------------------------------------
        private void textBoxUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                buttonAddUrl_Click(null, null);
        }
        //---------------------------------------------------------------------------
        private void DirUpButton_Click(object sender, EventArgs e)
        {
            //Scroll up the combo box list...
            if (comboBox1.Items.Count > 1)
            {
                if (comboBox1.SelectedIndex <= 0)
                    comboBox1.SelectedIndex = 1;
                else
                {
                    int idx = comboBox1.SelectedIndex - 1;
                    if (idx < 1)
                        comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
                    else
                        comboBox1.SelectedIndex = idx;
                }
            }
        }
        //---------------------------------------------------------------------------
        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            SetTreeViewToSelectedComboBoxItem();
        }
        //---------------------------------------------------------------------------
        private void buttonClear_Click(object sender, EventArgs e)
        {
            if (focusedControl == fileView)
            {
                // Deselect fileView items
                if (fileView.SelectedItems.Count > 0)
                {
                    fileView.SelectedItems.Clear();

                    // Don't turn off Clear if any items in songView...
                    if (buttonClear.Enabled && songView.Items.Count == 0)
                        buttonClear.Enabled = false;

                    buttonAddRight.Enabled = false;
                }
            }
            else if (songView.SelectedItems.Count > 0)  // songView
                DeleteSongViewSelItems();
            else
                SongViewInit();
        }
        //---------------------------------------------------------------------------
        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        //---------------------------------------------------------------------------
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
        //---------------------------------------------------------------------------
        private void changeRootMusicFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            labelSetRootFolder_Click(null, null);
        }
        //---------------------------------------------------------------------------
        private void songView_ClearSelectedMenuClick(object sender, EventArgs e)
        {
            try
            {
                for (int ii = 0; ii < songView.Items.Count; ii++)
                    songView.Items[ii].Selected = false;
            }
            catch
            {
            }
        }
        //---------------------------------------------------------------------------
        private void fileView_ClearSelectedMenuClick(object sender, EventArgs e)
        {
            try
            {
                for (int ii = 0; ii < fileView.Items.Count; ii++)
                    fileView.Items[ii].Selected = false;
            }
            catch
            {
            }
        }
        //---------------------------------------------------------------------------
        private void songView_RemoveAllMenuClick(object sender, EventArgs e)
        {
            SongViewInit();
        }
        //---------------------------------------------------------------------------
        private void songView_RemoveSelectedMenuClick(object sender, EventArgs e)
        {
            DeleteSongViewSelItems();
        }
        //---------------------------------------------------------------------------
        // Need to know who was focused when we click the "Add Files" button...
        private void fileView_GotFocus(object sender, EventArgs e)
        {
            focusedControl = (Control)sender;
            buttonAddRight.Enabled = true;
        }
        //---------------------------------------------------------------------------
        private void songView_GotFocus(object sender, EventArgs e)
        {
            focusedControl = (Control)sender;
            buttonAddRight.Enabled = false;
        }
        //---------------------------------------------------------------------------
        private void treeView_GotFocus(object sender, EventArgs e)
        {
            focusedControl = (Control)sender;
            buttonAddRight.Enabled = true;
        }
        //---------------------------------------------------------------------------
        private void FormDirectories_FormClosing(object sender, FormClosingEventArgs e)
        {
            parentForm.RootFolder = g_rootFolder;
            parentForm.RootFolderHasBeenSet = g_bRootFolderHasBeenSet;

            parentForm.FieldVisible = g_fieldVisible;
            parentForm.FormDirectoriesSize = new Size(Width, Height);

            parentForm.BrowserDialog = null; // Null the reference to ourself
        }
        //---------------------------------------------------------------------------
        private TreeNode GetNodeFromPath(TreeNode node, string path)
        {
            TreeNode foundNode = null;
            foreach (TreeNode tn in node.Nodes)
            {
                if (tn.FullPath == path)
                {
                    return tn;
                }
                else if (tn.Nodes.Count > 0)
                {
                    foundNode = GetNodeFromPath(tn, path);
                }
                if (foundNode != null)
                    return foundNode;
            }
            return null;
        }
        //---------------------------------------------------------------------------
        // To Change Sort:
        // ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text,
        //               listviewY.SubItems[ColumnToSort].Text);

        private void songView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.songView.Sort();
        }
        //---------------------------------------------------------------------------
        private void fileView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending) lvwColumnSorter.Order = SortOrder.Descending;
                else lvwColumnSorter.Order = SortOrder.Ascending;
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.fileView.Sort();
        }
        //---------------------------------------------------------------------------
        private void menuViewItem_Click(object sender, EventArgs e)
        {
            // User clicked a view-menu item...
            SetFieldVisibleFromMenuChecks();
            SongViewRefresh();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sHelp = "You can:\r\n" +
                "1) Right-click a music folder in the left box to add its songs to the right box.\n\n" +
                "2) Left-click a music folder in the left box to display its songs in the middle box.\n\n" +
                "3) Click the >> button to add songs in the middle box to the right box.\n\n" +
                "4) Add a range of songs in the right box to a player-list by:\n" +
                "   a. Click the first song in the range, press and hold the Shift key, then click the last song in the range.\n" +
                "      (or press and hold the Ctrl key and click the songs you want).\n" +
                "   b. Click either the Add A or Add B button to add the selected songs to a player's song-list.\n\n" +
                "5) Drag-drop selected music-folders or songs from the left box to the right box.\n\n" +
                "6) Drag-drop a range of selected folders or songs songs onto a player or its song-list.\n\n" +
                "7) Drag-drop songs from Windows Explorer into the right box or onto a player or its song-list.\n\n" +
                "8) Drag-drop songs from one player's song-list to the other player or drag a song to a new position in a song-list.";
            MessageBox.Show(sHelp, "SwiftMiX");
        }
        //---------------------------------------------------------------------------
        #endregion

        #region Mouse Events

        private void fileView_MouseDoubleClick(Object sender, MouseEventArgs e)
        {
            ListViewItem lvi = fileView.GetItemAt(e.X, e.Y);

            using (var tagReader = new MediaTags.MediaTags())
                AddFileToSongView(tagReader, (TreeTagInfo)lvi.Tag);

            if (songView.Items.Count != 0)
            {
                buttonAddA.Enabled = true;
                buttonAddB.Enabled = true;

                if (songView.Focused)
                    buttonClear.Enabled = true;

                // Make the new items visible...
                songView.EnsureVisible(songView.Items.Count - 1);
                songView.Focus();
            }
        }
        //---------------------------------------------------------------------------
        // Used for both fileView and songView
        private void listView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
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
                catch // Click over empty area
                {
                    dragBox = Rectangle.Empty;
                }
            }
            else
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                dragBox = Rectangle.Empty;
        }
        //---------------------------------------------------------------------------
        // Used for both fileView and songView
        private void listView_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                ListView lv = sender as ListView;

                // If the mouse moves outside the rectangle, start the drag.
                if (e.Button == MouseButtons.Left && dragBox != Rectangle.Empty &&
                                                             !dragBox.Contains(e.X, e.Y))
                {
                    dragSourcePaths.Clear();

                    if (lv.SelectedItems.Count != 0)
                    {
                        // "path is always the last column for both songView and fileView
                        if (lv == songView)
                        {
                            // songView "path" is in column 3
                            for (int ii = 0; ii < lv.SelectedItems.Count; ii++)
                                dragSourcePaths.Add(lv.SelectedItems[ii].SubItems[lv.Columns.Count - 1].Text);
                        }
                        else
                        {
                            // fileView "path" is in column 4
                            for (int ii = 0; ii < lv.SelectedItems.Count; ii++)
                                dragSourcePaths.Add(lv.SelectedItems[ii].SubItems[lv.Columns.Count - 1].Text);
                        }
                    }

                    var data = new DataObject(DataFormats.FileDrop, dragSourcePaths.ToArray());
                    lv.DoDragDrop(data, DragDropEffects.Copy);
                }
                else
                    // Reset the rectangle if the mouse is not over an item in the ListBox.
                    dragBox = Rectangle.Empty;
            }
            catch
            {
                dragBox = Rectangle.Empty;
            }
        }
        //---------------------------------------------------------------------------
        // Used for both fileView and songView
        private void listView_MouseUp(object sender, MouseEventArgs e)
        {
            // NOTE: this event hook won't fire for normal drag/drop... event
            // is purged i suppose...

            // Reset the drag rectangle when the mouse button is raised.
            dragBox = Rectangle.Empty;
        }
        //---------------------------------------------------------------------------
        private void treeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                try
                {
                    TreeView tv = sender as TreeView;

                    TreeNode dragSourceNode = tv.GetNodeAt(e.X, e.Y);

                    if (dragSourceNode == null)
                    {
                        dragBox = Rectangle.Empty;
                        return;
                    }

                    TreeTagInfo tti = new TreeTagInfo();
                    tti = (TreeTagInfo)dragSourceNode.Tag;

                    dragSourcePaths.Clear();
                    dragSourcePaths.Add(tti.Name); // Directory file path the node represents

                    // Remember the point where the mouse down occurred. The DragSize indicates
                    // the size that the mouse can move before a drag event should be started.                
                    Size dragSize = SystemInformation.DragSize;

                    // Create a rectangle using the DragSize, with the mouse position being
                    // at the center of the rectangle.
                    dragBox = new Rectangle(new Point(e.X - (dragSize.Width / 2),
                                              e.Y - (dragSize.Height / 2)), dragSize);
                }
                catch // Click over empty area
                {
                    dragBox = Rectangle.Empty;
                }
            }
            else
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                dragBox = Rectangle.Empty;
        }
        //---------------------------------------------------------------------------
        private void treeView_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                TreeView tv = sender as TreeView;

                // If the mouse moves outside the rectangle, start the drag.
                if (e.Button == MouseButtons.Left && dragBox != Rectangle.Empty &&
                                                                 !dragBox.Contains(e.X, e.Y))
                    tv.DoDragDrop(new DataObject(DataFormats.FileDrop, dragSourcePaths.ToArray()),
                              DragDropEffects.Copy);
                else
                    // Reset the rectangle if the mouse is not over an item in the ListBox.
                    dragBox = Rectangle.Empty;
            }
            catch
            {
                dragBox = Rectangle.Empty;
            }
        }
        //---------------------------------------------------------------------------
        //private void treeView_CopyToClipboardMenuClick(object sender, EventArgs e)
        //{
        //  if (this.treeView.Nodes.Count == 0)
        //    return;

        //  try
        //  {
        //    TreeView tv = sender as TreeView;

        //    Clipboard.Clear();

        //    // Copy node's file path to the clipboard as a FileDrop
        //    TreeTagInfo tti = new TreeTagInfo();
        //    TreeNode tn = tv.SelectedNode;
        //    tti = (TreeTagInfo)tn.Tag;
        //    string directory = tti.Name;
        //    string[] paths = { directory };
        //    tv.DoDragDrop(new DataObject(DataFormats.FileDrop, paths),
        //              DragDropEffects.Copy);
        //  }
        //  catch
        //  {
        //  }
        //}
        //---------------------------------------------------------------------------
        private void treeView_MouseUp(object sender, MouseEventArgs e)
        {
            // NOTE: this event hook won't fire for normal drag/drop... event
            // is purged i suppose...

            // Reset the drag rectangle when the mouse button is raised.
            dragBox = Rectangle.Empty;
        }
        //---------------------------------------------------------------------------
        private void treeView_MouseClick(Object sender, MouseEventArgs e)
        {
            //get current selected drive or folder
            TreeNode nodeCurrent = treeView.GetNodeAt(e.X, e.Y);

            if (e.Button == MouseButtons.Right)
            {
                if (treeView.SelectedNode != nodeCurrent)
                    treeView.SelectedNode = nodeCurrent;

                AddSongsFromTreeView(nodeCurrent);
            }
            else if (e.Button == MouseButtons.Left)
            {
                if (nodeCurrent.Level == 0)
                {
                    // Initiate a directories dialog
                    try
                    {
                        FolderBrowserDialog browserDialog = new FolderBrowserDialog();

                        browserDialog.Description = "Select a new " + g_rootTitle + " directory.";
                        browserDialog.ShowNewFolderButton = false;
                        browserDialog.SelectedPath = g_rootFolder;

                        if (browserDialog.ShowDialog() == DialogResult.OK)
                        {
                            g_rootFolder = browserDialog.SelectedPath;
                            PopulateDriveList();
                            SetTreeViewToSelectedComboBoxItem();
                        }
                    }
                    catch
                    {
                    }

                    // nodePrevious = null;
                    return;
                }
            }
        }
        //---------------------------------------------------------------------------
        #endregion

        #region Drag Drop

        private void songView_DragOver(object sender, DragEventArgs e)
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
        private void songView_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] filePaths = null;

                    if (e.Data.GetDataPresent(DataFormats.FileDrop))
                        filePaths = e.Data.GetData(DataFormats.FileDrop) as string[];

                    if (filePaths == null || filePaths.Length == 0)
                        return;

                    songView.SelectedItems.Clear();

                    int resultCode = 0;

                    using (var tagReader = new MediaTags.MediaTags())
                    {
                        foreach (string fileLoc in filePaths)
                        {
                            // Code to read the contents of the text file
                            if (Directory.Exists(fileLoc))
                            {
                                // Get files and subdir files - even shortcuts are expanded...
                                List<string> allfiles = new List<string>();
                                allfiles = GetAllFilesRecurse(fileLoc);

                                //Add files from folder
                                foreach (string s in allfiles)
                                {
                                    string ext = Path.GetExtension(s).ToLower();

                                    int index = 0;

                                    if (g_fileFilterList.Count > 0)
                                        index = this.FileFilterList.FindIndex(item => item.ToLower() == ext);

                                    if (index >= 0)
                                        if ((resultCode = AddFileToSongView(tagReader, s)) != 0)
                                            break;
                                }
                            }
                            else
                                resultCode = AddFileToSongView(tagReader, fileLoc);

                            if (resultCode != 0)
                                break;
                        }
                    }

                    if (songView.Items.Count != 0)
                    {
                        buttonAddA.Enabled = true;
                        buttonAddB.Enabled = true;
                        buttonClear.Enabled = true;

                        // Make the new items visible...
                        songView.EnsureVisible(songView.Items.Count - 1);
                        songView.Focus();
                    }
                }
            }
            catch
            {
            }
        }
        //---------------------------------------------------------------------------
        #endregion
    }
}
