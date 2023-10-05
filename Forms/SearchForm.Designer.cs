namespace OrderManagerEF
{
    partial class SearchForm
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
            simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)textEdit1.Properties).BeginInit();
            SuspendLayout();
            // 
            // textEdit1
            // 
            textEdit1.Location = new System.Drawing.Point(76, 33);
            textEdit1.Name = "textEdit1";
            textEdit1.Size = new System.Drawing.Size(100, 20);
            textEdit1.TabIndex = 0;
            // 
            // simpleButton1
            // 
            simpleButton1.Location = new System.Drawing.Point(76, 70);
            simpleButton1.Name = "simpleButton1";
            simpleButton1.Size = new System.Drawing.Size(75, 23);
            simpleButton1.TabIndex = 1;
            simpleButton1.Text = "Search";
            // 
            // SearchForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(270, 178);
            Controls.Add(simpleButton1);
            Controls.Add(textEdit1);
            Name = "SearchForm";
            Text = "Search";
            ((System.ComponentModel.ISupportInitialize)textEdit1.Properties).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.TextEdit textEdit1;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
    }
}