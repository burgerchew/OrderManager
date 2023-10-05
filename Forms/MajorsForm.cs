using DevExpress.XtraEditors;
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
    public partial class MajorsForm : XtraForm
    {
        // Set up a flag to keep track of whether data has been loaded yet.
        private bool _dataLoaded = false;
        private readonly IConfiguration _configuration;
        private readonly OMDbContext _context;

        public MajorsForm(IConfiguration configuration, OMDbContext context)
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;
            this.VisibleChanged += Majors_VisibleChanged;
        }

        private void Majors_VisibleChanged(object sender, EventArgs e)
        {
            // Load data when the form is visible, but only if it hasn't been loaded already.
            if (this.Visible && !_dataLoaded)
            {
                LoadData();
                _dataLoaded = true;
            }
        }

        private void LoadData()
        {
            var data = _context.BINContentsLocn11s.ToList();

            // Assuming gridControl or some other control is being populated
            gridControl1.DataSource = data;

            AddGroupSum();
        }

        private void AddGroupSum()
        {

            GridView gridView = (GridView)gridControl1.MainView;
            GridViewSumHelper gridViewSumHelper = new GridViewSumHelper(gridView);

            // Group by 'ProductCode' and sum 'ActualQuantity'
            gridViewSumHelper.AddSumToGroupedColumn("ActualQuantity", "ProductCode");
        }

        private void gridView_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;

            if (e.RowHandle >= 0)
            {
                int actualQuantity = Convert.ToInt32(view.GetRowCellValue(e.RowHandle, "ActualQuantity"));

                if (actualQuantity == 0)
                {
                    e.Appearance.BackColor = Color.LightPink;
                    e.Appearance.ForeColor = Color.Black;
                }
            }
        }
    }
}