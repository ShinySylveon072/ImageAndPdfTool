namespace ImageAndPdfTool
{
    partial class A4ImagePdfToolForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnSelectFolder = new Button();
            txtFolder = new TextBox();
            btnProcess = new Button();
            lblStatus = new Label();
            progressBar = new ProgressBar();
            SuspendLayout();
            // 
            // btnSelectFolder
            // 
            btnSelectFolder.Location = new Point(12, 14);
            btnSelectFolder.Name = "btnSelectFolder";
            btnSelectFolder.Size = new Size(120, 34);
            btnSelectFolder.TabIndex = 0;
            btnSelectFolder.Text = "Select Folder";
            btnSelectFolder.UseVisualStyleBackColor = true;
            btnSelectFolder.Click += btnSelectFolder_Click;
            // 
            // txtFolder
            // 
            txtFolder.Location = new Point(138, 18);
            txtFolder.Name = "txtFolder";
            txtFolder.ReadOnly = true;
            txtFolder.Size = new Size(350, 23);
            txtFolder.TabIndex = 1;
            // 
            // btnProcess
            // 
            btnProcess.Location = new Point(12, 57);
            btnProcess.Name = "btnProcess";
            btnProcess.Size = new Size(120, 34);
            btnProcess.TabIndex = 2;
            btnProcess.Text = "Process Images";
            btnProcess.UseVisualStyleBackColor = true;
            btnProcess.Click += btnProcess_Click;
            // 
            // lblStatus
            // 
            lblStatus.Location = new Point(12, 102);
            lblStatus.MaximumSize = new Size(476, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(476, 0);
            lblStatus.TabIndex = 5;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(138, 57);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(350, 34);
            progressBar.TabIndex = 4;
            // 
            // A4ImagePdfToolForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(500, 108);
            Controls.Add(progressBar);
            Controls.Add(lblStatus);
            Controls.Add(btnProcess);
            Controls.Add(txtFolder);
            Controls.Add(btnSelectFolder);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "A4ImagePdfToolForm";
            Text = "A3 to A4 Image Splitter and PDF Merger";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Button btnProcess;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}
