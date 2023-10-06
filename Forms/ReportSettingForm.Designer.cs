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
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            textEdit2 = new DevExpress.XtraEditors.TextEdit();
            textEdit3 = new DevExpress.XtraEditors.TextEdit();
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)textEdit1.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)textEdit2.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)textEdit3.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).BeginInit();
            SuspendLayout();
            // 
            // textEdit1
            // 
            textEdit1.Location = new System.Drawing.Point(114, 45);
            textEdit1.Name = "textEdit1";
            textEdit1.Size = new System.Drawing.Size(260, 20);
            textEdit1.StyleController = layoutControl1;
            textEdit1.TabIndex = 0;
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(textEdit1);
            layoutControl1.Controls.Add(textEdit2);
            layoutControl1.Controls.Add(textEdit3);
            layoutControl1.Location = new System.Drawing.Point(27, 56);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(736, 170, 650, 400);
            layoutControl1.Root = Root;
            layoutControl1.Size = new System.Drawing.Size(398, 143);
            layoutControl1.TabIndex = 8;
            layoutControl1.Text = "layoutControl1";
            // 
            // textEdit2
            // 
            textEdit2.Location = new System.Drawing.Point(114, 69);
            textEdit2.Name = "textEdit2";
            textEdit2.Size = new System.Drawing.Size(260, 20);
            textEdit2.StyleController = layoutControl1;
            textEdit2.TabIndex = 1;
            // 
            // textEdit3
            // 
            textEdit3.Location = new System.Drawing.Point(114, 93);
            textEdit3.Name = "textEdit3";
            textEdit3.Size = new System.Drawing.Size(260, 20);
            textEdit3.StyleController = layoutControl1;
            textEdit3.TabIndex = 2;
            // 
            // Root
            // 
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlGroup1 });
            Root.Name = "Root";
            Root.Size = new System.Drawing.Size(398, 143);
            Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.Control = textEdit1;
            layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Size = new System.Drawing.Size(354, 24);
            layoutControlItem1.Text = "Label File Path";
            layoutControlItem1.TextSize = new System.Drawing.Size(78, 13);
            // 
            // layoutControlItem2
            // 
            layoutControlItem2.Control = textEdit2;
            layoutControlItem2.Location = new System.Drawing.Point(0, 24);
            layoutControlItem2.Name = "layoutControlItem2";
            layoutControlItem2.Size = new System.Drawing.Size(354, 24);
            layoutControlItem2.Text = "PickSlip File Path";
            layoutControlItem2.TextSize = new System.Drawing.Size(78, 13);
            // 
            // layoutControlItem3
            // 
            layoutControlItem3.Control = textEdit3;
            layoutControlItem3.Location = new System.Drawing.Point(0, 48);
            layoutControlItem3.Name = "layoutControlItem3";
            layoutControlItem3.Size = new System.Drawing.Size(354, 30);
            layoutControlItem3.Text = "Error File Path";
            layoutControlItem3.TextSize = new System.Drawing.Size(78, 13);
            // 
            // layoutControlGroup1
            // 
            layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem1, layoutControlItem3, layoutControlItem2 });
            layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            layoutControlGroup1.Name = "layoutControlGroup1";
            layoutControlGroup1.Size = new System.Drawing.Size(378, 123);
            layoutControlGroup1.Text = "Configure Label and Pickslip Paths";
            // 
            // simpleButton1
            // 
            simpleButton1.Location = new System.Drawing.Point(36, 205);
            simpleButton1.Name = "simpleButton1";
            simpleButton1.Size = new System.Drawing.Size(377, 23);
            simpleButton1.TabIndex = 3;
            simpleButton1.Text = "Save";
            simpleButton1.Click += simpleButton1_Click;
            // 
            // labelControl1
            // 
            labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            labelControl1.Appearance.Options.UseFont = true;
            labelControl1.Location = new System.Drawing.Point(36, 12);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new System.Drawing.Size(129, 23);
            labelControl1.TabIndex = 4;
            labelControl1.Text = "Report Settings";
            // 
            // ReportSettingForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(441, 285);
            Controls.Add(layoutControl1);
            Controls.Add(labelControl1);
            Controls.Add(simpleButton1);
            Name = "ReportSettingForm";
            Text = "ReportSettingForm";
            ((System.ComponentModel.ISupportInitialize)textEdit1.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)textEdit2.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)textEdit3.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem3).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraEditors.TextEdit textEdit1;
        private DevExpress.XtraEditors.TextEdit textEdit2;
        private DevExpress.XtraEditors.TextEdit textEdit3;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
    }
}