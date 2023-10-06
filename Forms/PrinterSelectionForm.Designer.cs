namespace OrderManagerEF.Forms
{
    partial class PrinterSelectionForm
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
            layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            comboBoxEdit1 = new DevExpress.XtraEditors.ComboBoxEdit();
            checkedComboBoxEdit1 = new DevExpress.XtraEditors.CheckedComboBoxEdit();
            layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            Root = new DevExpress.XtraLayout.LayoutControlGroup();
            emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)comboBoxEdit1.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)checkedComboBoxEdit1.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem2).BeginInit();
            SuspendLayout();
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(comboBoxEdit1);
            layoutControl1.Controls.Add(checkedComboBoxEdit1);
            layoutControl1.HiddenItems.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem1 });
            layoutControl1.Location = new System.Drawing.Point(23, 50);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(680, 0, 650, 400);
            layoutControl1.Root = Root;
            layoutControl1.Size = new System.Drawing.Size(355, 140);
            layoutControl1.TabIndex = 0;
            layoutControl1.Text = "layoutControl1";
            // 
            // comboBoxEdit1
            // 
            comboBoxEdit1.Location = new System.Drawing.Point(68, 45);
            comboBoxEdit1.Name = "comboBoxEdit1";
            comboBoxEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            comboBoxEdit1.Size = new System.Drawing.Size(263, 20);
            comboBoxEdit1.StyleController = layoutControl1;
            comboBoxEdit1.TabIndex = 5;
            // 
            // checkedComboBoxEdit1
            // 
            checkedComboBoxEdit1.Location = new System.Drawing.Point(129, 45);
            checkedComboBoxEdit1.Name = "checkedComboBoxEdit1";
            checkedComboBoxEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            checkedComboBoxEdit1.Size = new System.Drawing.Size(50, 20);
            checkedComboBoxEdit1.StyleController = layoutControl1;
            checkedComboBoxEdit1.TabIndex = 4;
            // 
            // layoutControlItem1
            // 
            layoutControlItem1.Control = checkedComboBoxEdit1;
            layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            layoutControlItem1.Name = "layoutControlItem1";
            layoutControlItem1.Size = new System.Drawing.Size(159, 24);
            layoutControlItem1.TextSize = new System.Drawing.Size(93, 13);
            // 
            // Root
            // 
            Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            Root.GroupBordersVisible = false;
            Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { emptySpaceItem1, layoutControlGroup1 });
            Root.Name = "Root";
            Root.Size = new System.Drawing.Size(355, 140);
            Root.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            emptySpaceItem1.AllowHotTrack = false;
            emptySpaceItem1.Location = new System.Drawing.Point(0, 100);
            emptySpaceItem1.Name = "emptySpaceItem1";
            emptySpaceItem1.Size = new System.Drawing.Size(335, 20);
            emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlGroup1
            // 
            layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItem2, emptySpaceItem2 });
            layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            layoutControlGroup1.Name = "layoutControlGroup1";
            layoutControlGroup1.Size = new System.Drawing.Size(335, 100);
            layoutControlGroup1.Text = "Choose Printer";
            // 
            // layoutControlItem2
            // 
            layoutControlItem2.Control = comboBoxEdit1;
            layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            layoutControlItem2.Name = "layoutControlItem2";
            layoutControlItem2.Size = new System.Drawing.Size(311, 24);
            layoutControlItem2.Text = "Printer";
            layoutControlItem2.TextSize = new System.Drawing.Size(32, 13);
            // 
            // emptySpaceItem2
            // 
            emptySpaceItem2.AllowHotTrack = false;
            emptySpaceItem2.Location = new System.Drawing.Point(0, 24);
            emptySpaceItem2.Name = "emptySpaceItem2";
            emptySpaceItem2.Size = new System.Drawing.Size(311, 31);
            emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // simpleButton1
            // 
            simpleButton1.Location = new System.Drawing.Point(38, 196);
            simpleButton1.Name = "simpleButton1";
            simpleButton1.Size = new System.Drawing.Size(332, 23);
            simpleButton1.TabIndex = 1;
            simpleButton1.Text = "Save Changes";
            simpleButton1.Click += simpleButton1_Click_1;
            // 
            // labelControl1
            // 
            labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            labelControl1.Appearance.Options.UseFont = true;
            labelControl1.Location = new System.Drawing.Point(38, 12);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new System.Drawing.Size(110, 23);
            labelControl1.TabIndex = 2;
            labelControl1.Text = "Printer Setup";
            // 
            // PrinterSelectionForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(402, 251);
            Controls.Add(labelControl1);
            Controls.Add(simpleButton1);
            Controls.Add(layoutControl1);
            Name = "PrinterSelectionForm";
            Text = "PrinterSelectionForm";
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)comboBoxEdit1.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)checkedComboBoxEdit1.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)Root).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.CheckedComboBoxEdit checkedComboBoxEdit1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.ComboBoxEdit comboBoxEdit1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
    }
}