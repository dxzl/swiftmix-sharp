using System;
using System.Windows.Forms;

namespace SwiftMiX
{
    public partial class FormImport : Form
    {
        public FormImport()
        {
            InitializeComponent();
        }

        private int encodingIndex = -1;
        public int EncodingIndex
        {
            get { return encodingIndex; }
            set { encodingIndex = value; }
        }

        private void FormImport_Load(object sender, EventArgs e)
        {
            // For every encoding, get the property values.
            comboBoxEncoding.Items.Add("UTF8");
            comboBoxEncoding.Items.Add("ANSI (ASCII)");
            comboBoxEncoding.Items.Add("Unicode (UTF16LE)");
            comboBoxEncoding.Items.Add("Unicode (Big Endian)");
            comboBoxEncoding.Items.Add("UTF32");
            comboBoxEncoding.Items.Add("UTF7");
            comboBoxEncoding.Items.Add("Windows Default");

            encodingIndex = -1; // Auto
        }

        private void radioButtonManual_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButtonManual.Checked)
                comboBoxEncoding.Enabled = false;
            else
                comboBoxEncoding.Enabled = true;
        }

        private void FormImport_Shown(object sender, EventArgs e)
        {
            if (comboBoxEncoding.Items.Count <= 0) return;

            // user can change the EncodingIndex property before ShowDialog is called...
            if (encodingIndex == -1)
                comboBoxEncoding.SelectedIndex = 0; // UTF-8
            else
                comboBoxEncoding.SelectedIndex = encodingIndex;
        }

        private void FormImport_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (radioButtonManual.Checked)
                encodingIndex = comboBoxEncoding.SelectedIndex;
            else
                encodingIndex = -1; // Auto
        }
    }
}
