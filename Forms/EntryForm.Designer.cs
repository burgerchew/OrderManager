namespace OrderManagerEF.Forms
{
    partial class EntryForm
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
            xtraTabbedMdiManager1 = new DevExpress.XtraTabbedMdi.XtraTabbedMdiManager(components);
            navBarControl1 = new DevExpress.XtraNavBar.NavBarControl();
            navBarGroup1 = new DevExpress.XtraNavBar.NavBarGroup();
            navBarGroup2 = new DevExpress.XtraNavBar.NavBarGroup();
            navBarItem1 = new DevExpress.XtraNavBar.NavBarItem();
            navBarItem2 = new DevExpress.XtraNavBar.NavBarItem();
            navBarItem3 = new DevExpress.XtraNavBar.NavBarItem();
            navBarItem4 = new DevExpress.XtraNavBar.NavBarItem();
            navBarItem5 = new DevExpress.XtraNavBar.NavBarItem();
            navBarItem6 = new DevExpress.XtraNavBar.NavBarItem();
            navBarItem7 = new DevExpress.XtraNavBar.NavBarItem();
            navBarItem8 = new DevExpress.XtraNavBar.NavBarItem();
            ((System.ComponentModel.ISupportInitialize)xtraTabbedMdiManager1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)navBarControl1).BeginInit();
            SuspendLayout();
            // 
            // xtraTabbedMdiManager1
            // 
            xtraTabbedMdiManager1.MdiParent = this;
            // 
            // navBarControl1
            // 
            navBarControl1.ActiveGroup = navBarGroup1;
            navBarControl1.Dock = System.Windows.Forms.DockStyle.Left;
            navBarControl1.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] { navBarGroup1, navBarGroup2 });
            navBarControl1.Items.AddRange(new DevExpress.XtraNavBar.NavBarItem[] { navBarItem1, navBarItem2, navBarItem3, navBarItem4, navBarItem5, navBarItem6, navBarItem7, navBarItem8 });
            navBarControl1.Location = new System.Drawing.Point(0, 0);
            navBarControl1.Name = "navBarControl1";
            navBarControl1.Size = new System.Drawing.Size(212, 655);
            navBarControl1.TabIndex = 1;
            navBarControl1.Text = "navBarControl1";
            // 
            // navBarGroup1
            // 
            navBarGroup1.Caption = "New Orders";
            navBarGroup1.Expanded = true;
            navBarGroup1.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[] { new DevExpress.XtraNavBar.NavBarItemLink(navBarItem1), new DevExpress.XtraNavBar.NavBarItemLink(navBarItem2), new DevExpress.XtraNavBar.NavBarItemLink(navBarItem3), new DevExpress.XtraNavBar.NavBarItemLink(navBarItem4), new DevExpress.XtraNavBar.NavBarItemLink(navBarItem5), new DevExpress.XtraNavBar.NavBarItemLink(navBarItem6), new DevExpress.XtraNavBar.NavBarItemLink(navBarItem8) });
            navBarGroup1.Name = "navBarGroup1";
            // 
            // navBarGroup2
            // 
            navBarGroup2.Caption = "StarShipIT";
            navBarGroup2.Expanded = true;
            navBarGroup2.ItemLinks.AddRange(new DevExpress.XtraNavBar.NavBarItemLink[] { new DevExpress.XtraNavBar.NavBarItemLink(navBarItem7) });
            navBarGroup2.Name = "navBarGroup2";
            // 
            // navBarItem1
            // 
            navBarItem1.Caption = "CSC";
            navBarItem1.Name = "navBarItem1";
            // 
            // navBarItem2
            // 
            navBarItem2.Caption = "DS";
            navBarItem2.Name = "navBarItem2";
            // 
            // navBarItem3
            // 
            navBarItem3.Caption = "NZ";
            navBarItem3.Name = "navBarItem3";
            // 
            // navBarItem4
            // 
            navBarItem4.Caption = "Samples";
            navBarItem4.Name = "navBarItem4";
            // 
            // navBarItem5
            // 
            navBarItem5.Caption = "PreOrders";
            navBarItem5.Name = "navBarItem5";
            // 
            // navBarItem6
            // 
            navBarItem6.Caption = "Rubies Dropship";
            navBarItem6.Name = "navBarItem6";
            // 
            // navBarItem7
            // 
            navBarItem7.Caption = "navBarItem7";
            navBarItem7.Name = "navBarItem7";
            // 
            // navBarItem8
            // 
            navBarItem8.Caption = "Rubies Over 5KG";
            navBarItem8.Name = "navBarItem8";
            // 
            // EntryForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1127, 655);
            Controls.Add(navBarControl1);
            IsMdiContainer = true;
            Name = "EntryForm";
            Text = "EntryForm";
            ((System.ComponentModel.ISupportInitialize)xtraTabbedMdiManager1).EndInit();
            ((System.ComponentModel.ISupportInitialize)navBarControl1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraTabbedMdi.XtraTabbedMdiManager xtraTabbedMdiManager1;
        private DevExpress.XtraNavBar.NavBarControl navBarControl1;
        private DevExpress.XtraNavBar.NavBarGroup navBarGroup1;
        private DevExpress.XtraNavBar.NavBarItem navBarItem1;
        private DevExpress.XtraNavBar.NavBarItem navBarItem2;
        private DevExpress.XtraNavBar.NavBarItem navBarItem3;
        private DevExpress.XtraNavBar.NavBarItem navBarItem4;
        private DevExpress.XtraNavBar.NavBarItem navBarItem5;
        private DevExpress.XtraNavBar.NavBarItem navBarItem6;
        private DevExpress.XtraNavBar.NavBarGroup navBarGroup2;
        private DevExpress.XtraNavBar.NavBarItem navBarItem7;
        private DevExpress.XtraNavBar.NavBarItem navBarItem8;
    }
}