namespace PackageScanner
{
    partial class Scan
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Scan));
            txtLog = new TextBox();
            btnLoadFile = new Button();
            btnScanFile = new Button();
            btnExtractFile = new Button();
            toolTip1 = new ToolTip(components);
            chkDeleteWebhook = new CheckBox();
            chkDeleteDll = new CheckBox();
            chkDeleteCs = new CheckBox();
            SuspendLayout();
            // 
            // txtLog
            // 
            txtLog.Location = new Point(12, 12);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.Size = new Size(552, 335);
            txtLog.TabIndex = 0;
            // 
            // btnLoadFile
            // 
            btnLoadFile.Location = new Point(570, 12);
            btnLoadFile.Name = "btnLoadFile";
            btnLoadFile.Size = new Size(164, 23);
            btnLoadFile.TabIndex = 1;
            btnLoadFile.Text = "Select File";
            btnLoadFile.UseVisualStyleBackColor = true;
            btnLoadFile.Click += btnLoadFile_Click;
            // 
            // btnScanFile
            // 
            btnScanFile.Location = new Point(570, 69);
            btnScanFile.Name = "btnScanFile";
            btnScanFile.Size = new Size(164, 23);
            btnScanFile.TabIndex = 2;
            btnScanFile.Text = "Scan Selected File";
            toolTip1.SetToolTip(btnScanFile, "Scans file (you don't need to extract before hand as this process will do that)");
            btnScanFile.UseVisualStyleBackColor = true;
            btnScanFile.Click += btnScanFile_Click;
            // 
            // btnExtractFile
            // 
            btnExtractFile.Location = new Point(570, 41);
            btnExtractFile.Name = "btnExtractFile";
            btnExtractFile.Size = new Size(164, 23);
            btnExtractFile.TabIndex = 4;
            btnExtractFile.Text = "Extract Selected File";
            toolTip1.SetToolTip(btnExtractFile, "Only extracts the file so you can see contents without being in unity");
            btnExtractFile.UseVisualStyleBackColor = true;
            btnExtractFile.Click += btnExtractFile_Click;
            // 
            // chkDeleteWebhook
            // 
            chkDeleteWebhook.AutoSize = true;
            chkDeleteWebhook.Checked = true;
            chkDeleteWebhook.CheckState = CheckState.Checked;
            chkDeleteWebhook.Location = new Point(570, 98);
            chkDeleteWebhook.Name = "chkDeleteWebhook";
            chkDeleteWebhook.Size = new Size(164, 19);
            chkDeleteWebhook.TabIndex = 5;
            chkDeleteWebhook.Text = "Delete Possible Webhooks";
            chkDeleteWebhook.UseVisualStyleBackColor = true;
            // 
            // chkDeleteDll
            // 
            chkDeleteDll.AutoSize = true;
            chkDeleteDll.Location = new Point(570, 123);
            chkDeleteDll.Name = "chkDeleteDll";
            chkDeleteDll.Size = new Size(125, 19);
            chkDeleteDll.TabIndex = 6;
            chkDeleteDll.Text = "Delete All DLL Files";
            toolTip1.SetToolTip(chkDeleteDll, "For the paranoid");
            chkDeleteDll.UseVisualStyleBackColor = true;
            // 
            // chkDeleteCs
            // 
            chkDeleteCs.AutoSize = true;
            chkDeleteCs.Location = new Point(570, 148);
            chkDeleteCs.Name = "chkDeleteCs";
            chkDeleteCs.Size = new Size(119, 19);
            chkDeleteCs.TabIndex = 7;
            chkDeleteCs.Text = "Delete All CS Files";
            toolTip1.SetToolTip(chkDeleteCs, "For the paranoid");
            chkDeleteCs.UseVisualStyleBackColor = true;
            // 
            // Scan
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(740, 358);
            Controls.Add(chkDeleteCs);
            Controls.Add(chkDeleteDll);
            Controls.Add(chkDeleteWebhook);
            Controls.Add(btnExtractFile);
            Controls.Add(btnScanFile);
            Controls.Add(btnLoadFile);
            Controls.Add(txtLog);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Scan";
            Text = "Scan";
            Load += Scan_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtLog;
        private Button btnLoadFile;
        private Button btnScanFile;
        private Button btnExtractFile;
        private ToolTip toolTip1;
        private CheckBox chkDeleteWebhook;
        private CheckBox chkDeleteDll;
        private CheckBox chkDeleteCs;
    }
}