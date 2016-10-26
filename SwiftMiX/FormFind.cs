using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SwiftMiX
{
  public partial class FormFind : Form
  {
    FormPlaylist f2;

    public FormFind(FormPlaylist f)
    {
      InitializeComponent();

      f2 = f;
      this.Owner = f2;

      textBox1.Text = f2.FindText;
      checkBox1.Checked = f2.ExactCase;
    }

    private void button1_Click(object sender, EventArgs e)
    {
      f2.FindText = textBox1.Text;
      f2.ExactCase = checkBox1.Checked;
      this.Close();
    }

    private void button2_Click(object sender, EventArgs e)
    {
      Close();
    }
  }
}