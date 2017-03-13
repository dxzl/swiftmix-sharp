namespace SwiftMiX
{
  partial class FormFileTypes
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFileTypes));
      this.buttonOk = new System.Windows.Forms.Button();
      this.textBox = new System.Windows.Forms.TextBox();
      this.buttonRestore = new System.Windows.Forms.Button();
      this.buttonCancel = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // buttonOk
      // 
      this.buttonOk.BackColor = System.Drawing.SystemColors.InactiveCaption;
      this.buttonOk.Location = new System.Drawing.Point(12, 47);
      this.buttonOk.Name = "buttonOk";
      this.buttonOk.Size = new System.Drawing.Size(90, 23);
      this.buttonOk.TabIndex = 0;
      this.buttonOk.Text = "&Save";
      this.buttonOk.UseVisualStyleBackColor = false;
      this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
      // 
      // textBox
      // 
      this.textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBox.Location = new System.Drawing.Point(12, 12);
      this.textBox.Name = "textBox";
      this.textBox.Size = new System.Drawing.Size(268, 22);
      this.textBox.TabIndex = 3;
      this.textBox.Text = ".wma .mp3 .wav .aif .aiff";
      // 
      // buttonRestore
      // 
      this.buttonRestore.BackColor = System.Drawing.SystemColors.InactiveCaption;
      this.buttonRestore.Location = new System.Drawing.Point(108, 47);
      this.buttonRestore.Name = "buttonRestore";
      this.buttonRestore.Size = new System.Drawing.Size(172, 23);
      this.buttonRestore.TabIndex = 1;
      this.buttonRestore.Text = "&Restore Default";
      this.buttonRestore.UseVisualStyleBackColor = false;
      this.buttonRestore.Click += new System.EventHandler(this.buttonRestore_Click);
      // 
      // buttonCancel
      // 
      this.buttonCancel.BackColor = System.Drawing.SystemColors.InactiveCaption;
      this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.buttonCancel.Location = new System.Drawing.Point(12, 87);
      this.buttonCancel.Name = "buttonCancel";
      this.buttonCancel.Size = new System.Drawing.Size(268, 23);
      this.buttonCancel.TabIndex = 2;
      this.buttonCancel.Text = "&Cancel";
      this.buttonCancel.UseVisualStyleBackColor = false;
      this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(39, 124);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(206, 13);
      this.label1.TabIndex = 4;
      this.label1.Text = "Use a space between file extension types!";
      // 
      // FormFileTypes
      // 
      this.AcceptButton = this.buttonOk;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
      this.CancelButton = this.buttonCancel;
      this.ClientSize = new System.Drawing.Size(292, 146);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.buttonCancel);
      this.Controls.Add(this.buttonRestore);
      this.Controls.Add(this.textBox);
      this.Controls.Add(this.buttonOk);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FormFileTypes";
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Set Allowed File-Types";
      this.TopMost = true;
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button buttonOk;
    private System.Windows.Forms.TextBox textBox;
    private System.Windows.Forms.Button buttonRestore;
    private System.Windows.Forms.Button buttonCancel;
    private System.Windows.Forms.Label label1;
  }
}