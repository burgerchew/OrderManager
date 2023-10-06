using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using Microsoft.Extensions.Configuration;
using OrderManagerEF.Classes;
using OrderManagerEF.Data;
using OrderManagerEF.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using DevExpress.XtraSplashScreen;

namespace OrderManagerEF
{
    public partial class PackingForm : RibbonForm
    {
        private readonly TabbedViewHelper _tabbedViewHelper;
        private readonly ExcelExporter _excelExporter;
        private readonly IConfiguration _configuration;
        private readonly OMDbContext _context;


        public PackingForm(IConfiguration configuration, OMDbContext context)
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;

            gridView1.CustomDrawCell += gridView1_CustomDrawCell;
            _excelExporter = new ExcelExporter(gridView1);

            // Assign the Form_Load event
            Load += PackingForm_Load;
        }

        private void PackingForm_Load(object sender, EventArgs e)
        {
            LoadScanPackReportOverview();
            InitializeHyperLink();
        }

        private void LoadScanPackReportOverview()
        {
            var result = _context.vScanPackReportOverviews.ToList();
            
            gridControl1.DataSource = result;
        }

        private void InitializeHyperLink()
        {
            var repositoryItemHyperLinkEdit1 = new RepositoryItemHyperLinkEdit();

            repositoryItemHyperLinkEdit1.OpenLink += (sender, e) =>
            {
                var hyperlink = sender as HyperLinkEdit;
                if (hyperlink != null)
                {
                    var OrderRef = hyperlink.EditValue.ToString();
                    var currentRow = gridView1.GetFocusedDataRow();

                    if (currentRow != null &&
                        currentRow["CompleteFlag"].ToString() == "Packed" &&
                        int.Parse(currentRow["ItemCount"].ToString()) > 10)
                    {
                        var result = XtraMessageBox.Show("This query will take some time. Are you sure you want to check?",
                            "Confirmation",
                            MessageBoxButtons.YesNo);
                        if (result == DialogResult.No)
                        {
                            e.Handled = true;
                            return;
                        }

                        // Show the SplashScreen
                        SplashScreenManager.ShowDefaultWaitForm();
                    }

                    // Run your operation
                    var detailForm = new PackingDetailForm(_configuration, _context,OrderRef);
                    detailForm.Show();
                    e.Handled = true;

                    // If SplashScreen was shown, close it
                    if (SplashScreenManager.Default != null) SplashScreenManager.CloseForm(false);
                }
            };


            gridView1.Columns["AccountingRef"].ColumnEdit = repositoryItemHyperLinkEdit1;
        }


        private void gridView1_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName == "CompleteFlag")
            {
                var status = e.CellValue.ToString();
                switch (status)
                {
                    case "Pending":
                        e.Appearance.BackColor = Color.LightPink;
                        e.Appearance.ForeColor = Color.Black;
                        break;
                    case "Allocated":
                        e.Appearance.BackColor = Color.LightSalmon;
                        e.Appearance.ForeColor = Color.Black;
                        break;
                    case "Packed":
                        e.Appearance.BackColor = Color.LightGreen;
                        e.Appearance.ForeColor = Color.Black;
                        break;
                }
            }
        }


        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            _excelExporter.ExportToXls();
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Assuming gridView1 is your GridView
            gridView1.ActiveFilterString = "[CompleteFlag] = 'Allocated'";
        }


        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Assuming gridView1 is your GridView
            gridView1.ActiveFilterString = "[CompleteFlag] = 'Pending'";
        }

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Assuming gridView1 is your GridView
            gridView1.ActiveFilterString = "[CompleteFlag] = 'Packed'";
        }
    }
}