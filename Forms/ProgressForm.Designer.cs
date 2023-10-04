namespace OrderManagerEF
{
    partial class ProgressForm
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
            progressBarControl1 = new DevExpress.XtraEditors.ProgressBarControl();
            MessageLabel = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)progressBarControl1.Properties).BeginInit();
            SuspendLayout();
            // 
            // progressBarControl1
            // 
            progressBarControl1.Location = new System.Drawing.Point(61, 54);
            progressBarControl1.Name = "progressBarControl1";
            progressBarControl1.Size = new System.Drawing.Size(191, 39);
            progressBarControl1.TabIndex = 0;
            // 
            // MessageLabel
            // 
            MessageLabel.Location = new System.Drawing.Point(61, 122);
            MessageLabel.Name = "MessageLabel";
            MessageLabel.Size = new System.Drawing.Size(63, 13);
            MessageLabel.TabIndex = 1;
            MessageLabel.Text = "labelControl1";
            // 
            // ProgressForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(310, 202);
            Controls.Add(MessageLabel);
            Controls.Add(progressBarControl1);
            Name = "ProgressForm";
            Text = "ProgressForm1";
            ((System.ComponentModel.ISupportInitialize)progressBarControl1.Properties).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraEditors.ProgressBarControl progressBarControl1;
        private DevExpress.XtraEditors.LabelControl MessageLabel;
    }
}