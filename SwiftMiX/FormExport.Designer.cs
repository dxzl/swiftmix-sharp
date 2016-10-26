namespace SwiftMiX
{
    partial class FormExport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormExport));
            this.labelExportList = new System.Windows.Forms.Label();
            this.checkBoxSaveAsUtf8 = new System.Windows.Forms.CheckBox();
            this.checkBoxStoreUncFilePaths = new System.Windows.Forms.CheckBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxMode = new System.Windows.Forms.GroupBox();
            this.radioButtonFileNameOnly = new System.Windows.Forms.RadioButton();
            this.radioButtonAbsolute = new System.Windows.Forms.RadioButton();
            this.radioButtonRootedRelative = new System.Windows.Forms.RadioButton();
            this.radioButtonRelative = new System.Windows.Forms.RadioButton();
            this.groupBoxMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelExportList
            // 
            this.labelExportList.AutoSize = true;
            this.labelExportList.Location = new System.Drawing.Point(12, 9);
            this.labelExportList.Name = "labelExportList";
            this.labelExportList.Size = new System.Drawing.Size(51, 13);
            this.labelExportList.TabIndex = 0;
            this.labelExportList.Text = "(add text)";
            // 
            // checkBoxSaveAsUtf8
            // 
            this.checkBoxSaveAsUtf8.AutoSize = true;
            this.checkBoxSaveAsUtf8.Location = new System.Drawing.Point(63, 181);
            this.checkBoxSaveAsUtf8.Name = "checkBoxSaveAsUtf8";
            this.checkBoxSaveAsUtf8.Size = new System.Drawing.Size(99, 17);
            this.checkBoxSaveAsUtf8.TabIndex = 1;
            this.checkBoxSaveAsUtf8.Text = "Save As UTF-8";
            this.checkBoxSaveAsUtf8.UseVisualStyleBackColor = true;
            // 
            // checkBoxStoreUncFilePaths
            // 
            this.checkBoxStoreUncFilePaths.AutoSize = true;
            this.checkBoxStoreUncFilePaths.Location = new System.Drawing.Point(256, 181);
            this.checkBoxStoreUncFilePaths.Name = "checkBoxStoreUncFilePaths";
            this.checkBoxStoreUncFilePaths.Size = new System.Drawing.Size(126, 17);
            this.checkBoxStoreUncFilePaths.TabIndex = 2;
            this.checkBoxStoreUncFilePaths.Text = "Store UNC File Paths";
            this.checkBoxStoreUncFilePaths.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(31, 213);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(168, 23);
            this.buttonOk.TabIndex = 3;
            this.buttonOk.Text = "&Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(238, 213);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(168, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBoxMode
            // 
            this.groupBoxMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxMode.Controls.Add(this.radioButtonFileNameOnly);
            this.groupBoxMode.Controls.Add(this.radioButtonAbsolute);
            this.groupBoxMode.Controls.Add(this.radioButtonRootedRelative);
            this.groupBoxMode.Controls.Add(this.radioButtonRelative);
            this.groupBoxMode.Location = new System.Drawing.Point(12, 42);
            this.groupBoxMode.Name = "groupBoxMode";
            this.groupBoxMode.Size = new System.Drawing.Size(414, 123);
            this.groupBoxMode.TabIndex = 5;
            this.groupBoxMode.TabStop = false;
            this.groupBoxMode.Text = "Mode";
            // 
            // radioButtonFileNameOnly
            // 
            this.radioButtonFileNameOnly.AutoSize = true;
            this.radioButtonFileNameOnly.Location = new System.Drawing.Point(19, 91);
            this.radioButtonFileNameOnly.Name = "radioButtonFileNameOnly";
            this.radioButtonFileNameOnly.Size = new System.Drawing.Size(85, 17);
            this.radioButtonFileNameOnly.TabIndex = 3;
            this.radioButtonFileNameOnly.TabStop = true;
            this.radioButtonFileNameOnly.Text = "radioButton4";
            this.radioButtonFileNameOnly.UseVisualStyleBackColor = true;
            // 
            // radioButtonAbsolute
            // 
            this.radioButtonAbsolute.AutoSize = true;
            this.radioButtonAbsolute.Location = new System.Drawing.Point(19, 68);
            this.radioButtonAbsolute.Name = "radioButtonAbsolute";
            this.radioButtonAbsolute.Size = new System.Drawing.Size(85, 17);
            this.radioButtonAbsolute.TabIndex = 2;
            this.radioButtonAbsolute.TabStop = true;
            this.radioButtonAbsolute.Text = "radioButton3";
            this.radioButtonAbsolute.UseVisualStyleBackColor = true;
            // 
            // radioButtonRootedRelative
            // 
            this.radioButtonRootedRelative.AutoSize = true;
            this.radioButtonRootedRelative.Location = new System.Drawing.Point(19, 45);
            this.radioButtonRootedRelative.Name = "radioButtonRootedRelative";
            this.radioButtonRootedRelative.Size = new System.Drawing.Size(85, 17);
            this.radioButtonRootedRelative.TabIndex = 1;
            this.radioButtonRootedRelative.TabStop = true;
            this.radioButtonRootedRelative.Text = "radioButton2";
            this.radioButtonRootedRelative.UseVisualStyleBackColor = true;
            // 
            // radioButtonRelative
            // 
            this.radioButtonRelative.AutoSize = true;
            this.radioButtonRelative.Location = new System.Drawing.Point(19, 22);
            this.radioButtonRelative.Name = "radioButtonRelative";
            this.radioButtonRelative.Size = new System.Drawing.Size(85, 17);
            this.radioButtonRelative.TabIndex = 0;
            this.radioButtonRelative.TabStop = true;
            this.radioButtonRelative.Text = "radioButton1";
            this.radioButtonRelative.UseVisualStyleBackColor = true;
            // 
            // FormExport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 248);
            this.Controls.Add(this.groupBoxMode);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.checkBoxStoreUncFilePaths);
            this.Controls.Add(this.checkBoxSaveAsUtf8);
            this.Controls.Add(this.labelExportList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormExport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormExport";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormExport_FormClosing);
            this.Load += new System.EventHandler(this.FormExport_Load);
            this.Shown += new System.EventHandler(this.FormExport_Shown);
            this.groupBoxMode.ResumeLayout(false);
            this.groupBoxMode.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelExportList;
        private System.Windows.Forms.CheckBox checkBoxSaveAsUtf8;
        private System.Windows.Forms.CheckBox checkBoxStoreUncFilePaths;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBoxMode;
        private System.Windows.Forms.RadioButton radioButtonFileNameOnly;
        private System.Windows.Forms.RadioButton radioButtonAbsolute;
        private System.Windows.Forms.RadioButton radioButtonRootedRelative;
        private System.Windows.Forms.RadioButton radioButtonRelative;
    }
}