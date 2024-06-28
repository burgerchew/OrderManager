using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace OrderManagerEF.Reports
{
    public partial class PreorderPickslipReport : DevExpress.XtraReports.UI.XtraReport
    {
        public PreorderPickslipReport()
        {
            InitializeComponent();
            ApplyConditionalFormatting();
        }

        private void ApplyConditionalFormatting()
        {
            // Create the formatting rule for QtyOnHand == 0 (Red)
            FormattingRule formattingRuleRed = new FormattingRule();
            formattingRuleRed.DataSource = this.DataSource;
            formattingRuleRed.DataMember = this.DataMember;
            formattingRuleRed.Condition = "[QtyOnHand] == 0";
            formattingRuleRed.Formatting.ForeColor = Color.Red;

            // Create the formatting rule for QtyOnHand != 0 (Green)
            FormattingRule formattingRuleGreen = new FormattingRule();
            formattingRuleGreen.DataSource = this.DataSource;
            formattingRuleGreen.DataMember = this.DataMember;
            formattingRuleGreen.Condition = "[QtyOnHand] != 0";
            formattingRuleGreen.Formatting.ForeColor = Color.Green;

            // Find the field in the report and apply the rules
            // Find the xrLabel26 in the report and apply the rules
            XRLabel qtyOnHandLabel = this.FindControl("xrLabel26", true) as XRLabel;
            if (qtyOnHandLabel != null)
            {
                qtyOnHandLabel.FormattingRules.Add(formattingRuleRed);
                qtyOnHandLabel.FormattingRules.Add(formattingRuleGreen);
            }
        }
    }
}
