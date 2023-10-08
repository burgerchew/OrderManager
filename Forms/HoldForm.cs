using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Microsoft.Extensions.Configuration;
using OrderManagerEF.Classes;
using OrderManagerEF.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrderManagerEF
{
    public partial class HoldForm : DevExpress.XtraEditors.XtraForm
    {


        private GridControl gridControl1; // Replace with your actual grid control name
        private GridView gridView1; // Replace with your actual grid view name
        private IConfiguration _configuration;
        private bool _dataLoaded = false;
        private OMDbContext _context;
        private StoredProcedureService _storedProcedureService;


        public HoldForm(IConfiguration configuration, OMDbContext context)
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;

            this.VisibleChanged += new EventHandler(this.Hold_VisibleChanged);
            Load += Hold_Load;
            barButtonItem1.ItemClick += barButtonItem1_ItemClick;

        }


        private void LoadData()
        {
            var data = _context.HoldOrderDatas.ToList();
            // Populate the grid control with the fetched data
            gridView1.GridControl.DataSource = data;


        }



        private void Hold_Load(object sender, EventArgs e)
        {
            LoadData();

        }

        private void Hold_VisibleChanged(Object sender, EventArgs e)
        {
            if (this.Visible && !_dataLoaded)
            {
                LoadData();
                _dataLoaded = true;
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            {


                if (gridView1.SelectedRowsCount == 0)
                {
                    XtraMessageBox.Show("Please select one or more rows");
                    return;
                }


                var selectedRowHandles = gridView1.GetSelectedRows();
                var salesOrderReferences = new List<string>();

                foreach (var rowHandle in selectedRowHandles)
                {
                    var salesOrderReference = gridView1.GetRowCellValue(rowHandle, "AccountingRef").ToString();
                    salesOrderReferences.Add(salesOrderReference);
                }

                ActivateOrder(salesOrderReferences);
                // Refresh the GridView
                XtraMessageBox.Show(
                    "These orders has been moved to the relevant order import screen.");

                // Refresh the GridView
                var data = _context.HoldOrderDatas.ToList();
                // Populate the grid control with the fetched data
                gridView1.GridControl.DataSource = data;
                gridView1.RefreshData();
            }
        }

        private void ActivateOrder(List<string> salesOrderReferences)
        {
            var connectionString = _configuration.GetConnectionString("RubiesConnectionString");

            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var salesOrderReference in salesOrderReferences)
                    using (var command = new System.Data.SqlClient.SqlCommand("dbo.ASP_ACTIVATE", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Assuming the parameter name is @SalesOrderReference in your stored procedure
                        command.Parameters.AddWithValue("@SalesOrderReference", salesOrderReference);

                        command.ExecuteNonQuery();
                    }
            }
        }

    }
}