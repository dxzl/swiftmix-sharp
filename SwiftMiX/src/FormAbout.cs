using System;
using System.Windows.Forms;
using System.Diagnostics; // "Process" needs this...

namespace SwiftMiX
{
    public partial class FormAbout : Form
    {
        // Days over which we display "unlimited" in About dialog
        // Dec 31, 2099 - Jan 1, 1980 (around 120 years)
        public const int LK_DISPLAY_UNLIMITTED_DAYS = (2488082 - 2444588);

        private FormMain parentForm = null;

        public FormAbout(FormMain ParentForm)
        {
            InitializeComponent();

            parentForm = ParentForm;
            this.Owner = parentForm;

            this.PrintDaysRemaining();

            this.label1.Text = FormMain.REVISION.ToString() + "." + FormMain.DEF_PRODUCT_ID.ToString() +
                                "." + FormMain.DEF_SUPER_REV.ToString();
            this.label2.Text = "SwiftMiX by\r\nMister Swift\r\n(Discrete-Time Systems), 2014.";
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            parentForm.DoHelp();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            parentForm.DoHelp();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            //if (!FormMain.FREEWARE)
            //{
            //  try
            //  {
            //    LicenseKey lk = new LicenseKey(parentForm);

            //    if (!lk.DoKey(false))
            //      lk.ValidateLicenseKey(false); // Revalidate old existing key if new key was bad.

            //    this.PrintDaysRemaining();
            //  }
            //  catch
            //  {
            //  }
            //}
        }

        private void PrintDaysRemaining()
        {
            string S;

            //if (FormMain.FREEWARE)
            //{
            S = "V" + parentForm.Version + " - License Days: (No Expiration)";
            //}
            //else
            //{
            //  int DaysRem = parentForm.pk.ComputeDaysRemaining();

            //  // -100 if failure
            //  if (DaysRem >= 0 || DaysRem > LK_DISPLAY_UNLIMITTED_DAYS)
            //  {
            //    S = "V" + parentForm.Version + " - License Days: ";

            //    if (DaysRem >= LK_DISPLAY_UNLIMITTED_DAYS)
            //      S += "(No Expiration)";
            //    else if (DaysRem == 0)
            //      S += "(License Expired)";
            //    else
            //      S += DaysRem.ToString();
            //  }
            //  else
            //    S = "V" + parentForm.Version + " - (Invalid License)";
            //}

            this.Text = S;
        }
    }
}