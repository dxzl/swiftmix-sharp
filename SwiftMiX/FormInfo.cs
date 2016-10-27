//---------------------------------------------------------------------------
// SwiftMiX - Automatically fade songs between two playlists using
// dual Windows Media Player Active-X controls.
//
// Author: Scott Swift
//
// Released to GitHub under GPL v3 October, 2016
//
//---------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SwiftMiX
{
  public partial class FormInfo : Form
  {
    #region Properties

    // Properties
    private List<string> infoList;
    public List<string> InfoList
    {
      set { this.infoList = value; }
    }

    private string titleStr;
    public string TitleStr
    {
      set { this.titleStr = value; this.Text = value; }
    }
    #endregion

    public FormInfo()
    {
      this.InitializeComponent();
    }

    private void buttonOkClick(object sender, EventArgs e)
    {
      this.Close();
    }

    private void FormInfo_Shown(object sender, EventArgs e)
    {
      if ( this.infoList.Count > 0 )
        this.richInfo.Lines = infoList.ToArray();
    }

    private void copyToolStripMenuItem_Click(object sender, EventArgs e)
    {
      //Clipboard.Clear();

      //DataObject data = new DataObject();
      //data.SetData(DataFormats.Text, this.richInfo.Text);
      //data.SetData(DataFormats.Rtf, this.richInfo.Rtf);

      //Clipboard.SetDataObject(data);

      // Fix the CR/LF order in any string...
      //string s = this.richInfo.Text;
      //s.Replace(Environment.NewLine.ToString(), ControlChars.Cr.ToString()).
      //  Replace(ControlChars.Lf.ToString(), ControlChars.Cr.ToString()).
      //  Replace(ControlChars.Cr.ToString(), Environment.NewLine.ToString());

      StringBuilder sb = new StringBuilder();

      foreach (string line in this.richInfo.Lines)
        sb.AppendLine(line);

      Clipboard.SetText(sb.ToString());
    }
  }
}
