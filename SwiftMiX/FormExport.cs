using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace SwiftMiX
{
    public partial class FormExport : Form
    {
        String Ext;

        private int FMode = -1;
        public int Mode
        {
            get { return FMode; }
            set { FMode = value; }
        }

        private bool FSaveAsUtf8 = false;
        public bool SaveAsUtf8
        {
            get { return FSaveAsUtf8; }
            set { FSaveAsUtf8 = value; }
        }

        private bool FUncPathFormat = false;
        public bool UncPathFormat
        {
            get { return FUncPathFormat; }
            set { FUncPathFormat = value; }
        }

        private String FFile = "";
        public String FileName
        {
            get { return FFile; }
            set { FFile = value; }
        }

        private String FTitle = "";
        public String Title
        {
            get { return FTitle; }
            set { FTitle = value; }
        }

        public FormExport()
        {
            InitializeComponent();
        }

        private void FormExport_Load(object sender, EventArgs e)
        {
            // set the caption and tag of each mode-radio-button (tag is used for the selected index)
            radioButtonRelative.Text = "This list will be in my Music's root folder (relative path)";
            radioButtonRelative.Tag = 0;
            radioButtonRootedRelative.Text = "This list will be on the same drive as my music (rooted relative path)";
            radioButtonRootedRelative.Tag = 1;
            radioButtonAbsolute.Text = "This list will be on a different drive from my music (absolute or \"full\" path)";
            radioButtonAbsolute.Tag = 2;
            radioButtonFileNameOnly.Text = "Just save the song file-names and I'll edit the playlist if I need to (no path)";
            radioButtonFileNameOnly.Tag = 3;
        }

        private void FormExport_Shown(object sender, EventArgs e)
        {
            if (FMode == -1)
                FMode = ExportClass.EXPORT_PATH_ABSOLUTE;

            // set the user's default radio-button "checked" and clear the rest...
            foreach (var control in groupBoxMode.Controls)
                if (control is RadioButton)
                {
                    int index = (int)((RadioButton)control).Tag;
                    if (index == FMode)
                        ((RadioButton)control).Checked = true;
                    else
                        ((RadioButton)control).Checked = false;
                }

            this.Text = FFile; // this will handle UTF-8 (with special handler WMSetText())
            labelExportList.Text = FTitle; // "Export Player X Songlist"

            Ext = Path.GetExtension(FFile).ToLower();
            //  "All Files (*.*)|*.*|" + // 1
            //  "Windows Media (wpl)|*.wpl|" +
            //  "MPEG UTF-8 (m3u8)|*.m3u8|" +
            //  "MPEG ANSI (m3u)|*.m3u|" +
            //  "Adv Stream XML (asx)|*.asx|" +
            //  "XML Shareable (xspf)|*.xspf|" +
            //  "Win Audio XML (wax)|*.wax|" +
            //  "Windows XML (wmx)|*.wmx|" +
            //  "Winamp (pls)|*.pls|" +
            //  "Text (txt)|*.txt"; // 11
            checkBoxSaveAsUtf8.Enabled = true;
            checkBoxSaveAsUtf8.Checked = (Ext == ".m3u" || Ext == ".pls" || Ext == ".txt") ? false : true;

            checkBoxStoreUncFilePaths.Enabled = (Ext == ".xspf") ? false : true; // xspf no choice allowed
            checkBoxStoreUncFilePaths.Checked = (Ext == ".xspf") ? true : false;
        }

        private void FormExport_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var control in groupBoxMode.Controls)
                if (control is RadioButton && ((RadioButton)control).Checked)
                    FMode = (int)((RadioButton)control).Tag;

            FSaveAsUtf8 = checkBoxSaveAsUtf8.Checked;
            FUncPathFormat = checkBoxStoreUncFilePaths.Checked;
        }
    }
}
