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
  public partial class FormFileTypes : Form
  {
    private FormMain form = null;

    public FormFileTypes(FormMain form)
    {
      InitializeComponent();
      this.form = form;
      textBox.Text = DecodeFilters(form.FileFilterList);
    }

    private void buttonRestore_Click(object sender, EventArgs e)
    {
      textBox.Text = ".mp3 .wma .wav .aif .aiff";
    }

    private void buttonOk_Click(object sender, EventArgs e)
    {
      form.FileFilterList = EncodeFilters(textBox.Text);
      Close();
    }

    private void buttonCancel_Click(object sender, EventArgs e)
    {
      Close();
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
  }
}
