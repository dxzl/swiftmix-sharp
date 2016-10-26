using System;
using System.Windows.Forms;

namespace SwiftMiX
{
    public partial class FormAbout : Form
    {
        // Days over which we display "unlimited" in About dialog
        // Dec 31, 2099 - Jan 1, 1980 (around 120 years)
        public const int LK_DISPLAY_UNLIMITTED_DAYS = (2488082 - 2444588);

        private FormMain f1 = null;

        public FormAbout(FormMain f)
        {
            InitializeComponent();

            f1 = f;
            this.Owner = f1;

            this.PrintDaysRemaining();

            this.label1.Text = FormMain.REVISION.ToString() + "." + FormMain.DEF_PRODUCT_ID.ToString() +
                                "." + FormMain.DEF_SUPER_REV.ToString();
            this.label2.Text = "SwiftMiX by\r\nScott Swift\r\n(GitHub Release), 2016.";
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            f1.RunIexplore(FormMain.HELPSITE);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            f1.RunIexplore(FormMain.HELPSITE);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("WhhhhheeeeeeeHaaaaaaaa!");
        }

        private void PrintDaysRemaining()
        {
            this.Text = "V" + f1.Version + " - License Days: (No Expiration)";
        }
    }
}
