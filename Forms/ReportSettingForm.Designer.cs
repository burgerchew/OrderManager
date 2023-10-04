namespace OrderManagerEF
{
    partial class ReportSettingForm
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
            textEdit1 = new DevExpress.XtraEditors.TextEdit();
            textEdit2 = new DevExpress.XtraEditors.TextEdit();
            textEdit3 = new DevExpress.XtraEditors.TextEdit();
            simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            labelControl2 = new DevExpress.XtraEditors.LabelControl();
            labelControl3 = new DevExpress.XtraEditors.LabelControl();
            labelControl4 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)textEdit1.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)textEdit2.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)textEdit3.Properties).BeginInit();
            SuspendLayout();
            // 
            // textEdit1
            // 
            textEdit1.Location = new System.Drawing.Point(134, 83);
            textEdit1.Name = "textEdit1";
            textEdit1.Size = new System.Drawing.Size(171, 20);
            textEdit1.TabIndex = 0;
            // 
            // textEdit2
            // 
            textEdit2.Location = new System.Drawing.Point(134, 130);
            textEdit2.Name = "textEdit2";
            textEdit2.Size = new System.Drawing.Size(171, 20);
            textEdit2.TabIndex = 1;
            // 
            // textEdit3
            // 
            textEdit3.Location = new System.Drawing.Point(135, 174);
            textEdit3.Name = "textEdit3";
            textEdit3.Size = new System.Drawing.Size(170, 20);
            textEdit3.TabIndex = 2;
            // 
            // simpleButton1
            // 
            simpleButton1.Location = new System.Drawing.Point(135, 218);
            simpleButton1.Name = "simpleButton1";
            simpleButton1.Size = new System.Drawing.Size(75, 23);
            simpleButton1.TabIndex = 3;
            simpleButton1.Text = "Save";
            // 
            // labelControl1
            // 
            labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            labelControl1.Appearance.Options.UseFont = true;
            labelControl1.Location = new System.Drawing.Point(27, 27);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new System.Drawing.Size(129, 23);
            labelControl1.TabIndex = 4;
            labelControl1.Text = "Report Settings";
            // 
            // labelControl2
            // 
            labelControl2.Location = new System.Drawing.Point(34, 90);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new System.Drawing.Size(69, 13);
            labelControl2.TabIndex = 5;
            labelControl2.Text = "Label File Path";
            // 
            // labelControl3
            // 
            labelControl3.Location = new System.Drawing.Point(34, 137);
            labelControl3.Name = "labelControl3";
            labelControl3.Size = new System.Drawing.Size(78, 13);
            labelControl3.TabIndex = 6;
            labelControl3.Text = "PickSlip File Path";
            // 
            // labelControl4
            // 
            labelControl4.Location = new System.Drawing.Point(34, 181);
            labelControl4.Name = "labelControl4";
            labelControl4.Size = new System.Drawing.Size(68, 13);
            labelControl4.TabIndex = 7;
            labelControl4.Text = "Error File Path";
            // 
            // ReportSettingForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(329, 301);
            Controls.Add(labelControl4);
            Controls.Add(labelControl3);
            Controls.Add(labelControl2);
            Controls.Add(labelControl1);
            Controls.Add(simpleButton1);
            Controls.Add(textEdit3);
            Controls.Add(textEdit2);
            Controls.Add(textEdit1);
            Name = "ReportSettingForm";
            Text = "ReportSettingForm";
            ((System.ComponentModel.ISupportInitialize)textEdit1.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)textEdit2.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)textEdit3.Properties).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraEditors.TextEdit textEdit1;
        private DevExpress.XtraEditors.TextEdit textEdit2;
        private DevExpress.XtraEditors.TextEdit textEdit3;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl4;
    }
}