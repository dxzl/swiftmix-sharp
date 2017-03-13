using System;
using System.Windows.Forms;

namespace SwiftMiX
{
  public partial class FormLicense : Form
  {
    private FormMain f1 = null;

    public string LicenseText
    {
      get { return this.licenseText.Text; }
    }

    public string EmailText
    {
      get { return this.emailText.Text; }
    }

    public FormLicense(FormMain f)
    {
      InitializeComponent();

      f1 = f;
      this.Owner = f1;
    }

    private void buttonOK_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void buttonCancel_Click(object sender, EventArgs e)
    {
      Close();
    }
  }
}