namespace SwiftMiX
{
  partial class FormInfo
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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInfo));
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.buttonOk = new System.Windows.Forms.Button();
      this.richInfo = new System.Windows.Forms.RichTextBox();
      this.menuPopup = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.tableLayoutPanel1.SuspendLayout();
      this.menuPopup.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.buttonOk, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.richInfo, 0, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(292, 266);
      this.tableLayoutPanel1.TabIndex = 1;
      // 
      // buttonOk
      // 
      this.buttonOk.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.buttonOk.Location = new System.Drawing.Point(112, 232);
      this.buttonOk.Name = "buttonOk";
      this.buttonOk.Size = new System.Drawing.Size(68, 27);
      this.buttonOk.TabIndex = 2;
      this.buttonOk.Text = "OK";
      this.buttonOk.UseVisualStyleBackColor = true;
      this.buttonOk.Click += new System.EventHandler(this.buttonOkClick);
      // 
      // richInfo
      // 
      this.richInfo.ContextMenuStrip = this.menuPopup;
      this.richInfo.Dock = System.Windows.Forms.DockStyle.Fill;
      this.richInfo.Location = new System.Drawing.Point(3, 3);
      this.richInfo.Name = "richInfo";
      this.richInfo.ReadOnly = true;
      this.richInfo.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
      this.richInfo.Size = new System.Drawing.Size(286, 220);
      this.richInfo.TabIndex = 3;
      this.richInfo.Text = "";
      // 
      // menuPopup
      // 
      this.menuPopup.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem});
      this.menuPopup.Name = "menuPopup";
      this.menuPopup.Size = new System.Drawing.Size(103, 26);
      // 
      // copyToolStripMenuItem
      // 
      this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
      this.copyToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
      this.copyToolStripMenuItem.Text = "Copy";
      this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
      // 
      // FormInfo
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(292, 266);
      this.Controls.Add(this.tableLayoutPanel1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "FormInfo";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Song Information";
      this.Shown += new System.EventHandler(this.FormInfo_Shown);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.menuPopup.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Button buttonOk;
    private System.Windows.Forms.RichTextBox richInfo;
    private System.Windows.Forms.ContextMenuStrip menuPopup;
    private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;

  }
}