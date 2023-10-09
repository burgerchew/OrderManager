using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using Microsoft.Extensions.Configuration;
using OrderManagerEF.Classes;
using OrderManagerEF.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrderManagerEF
{
    public partial class PickandPackForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private bool _dataLoaded = false;
        private readonly IConfiguration _configuration;
        private readonly OMDbContext _context;

        public PickandPackForm(IConfiguration configuration, OMDbContext context)
        {
            InitializeComponent();
            _configuration = configuration;
            _context = context;

            this.WindowState = FormWindowState.Maximized;
            this.VisibleChanged += new EventHandler(this.PickandPack_VisibleChanged);


        }

        private void PickandPack_VisibleChanged(Object sender, EventArgs e)
        {
            if (this.Visible && !_dataLoaded)
            {
                LoadData();
                _dataLoaded = true;
            }
        }
        private void LoadData()
        {
            var data = _context.BINContentsLocn1s.ToList();

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

        public void SetSearchTextAndClickButton(string SKU)
        {
            // Assuming your GridControl is named gridControl1
            this.gridControl1.ForceInitialize(); // Ensures that all visual elements of the grid control are created

            // Cast MainView to GridView (which is derived from ColumnView)
            GridView view = this.gridControl1.MainView as GridView;
            if (view != null)
            {
                // Set the search string for the Find Panel
                view.FindFilterText = SKU;
            }
        }



        private void gridView_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
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

        public static Image ResizeImage(Image image, int maxWidth, int maxHeight)
        {
            double ratioX = (double)maxWidth / image.Width;
            double ratioY = (double)maxHeight / image.Height;
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(image.Width * ratio);
            int newHeight = (int)(image.Height * ratio);

            Image resizedImage = new Bitmap(newWidth, newHeight);

            using (Graphics graphics = Graphics.FromImage(resizedImage))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return resizedImage;
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "ImageColumn" && e.IsGetData)
            {
                try
                {
                    string uncPathPrefix = @"C:\test\images\";
                    string productCode = gridView1.GetListSourceRowCellValue(e.ListSourceRowIndex, "ProductCode").ToString();
                    string fileName = productCode + ".jpg"; // Append the ".jpg" extension
                    string uncImagePath = Path.Combine(uncPathPrefix, fileName);

                    if (File.Exists(uncImagePath))
                    {
                        Image image = Image.FromFile(uncImagePath);

                        // Resize the image to fit in the grid column
                        int maxWidth = 300; // Set the maximum width for the image
                        int maxHeight = 300; // Set the maximum height for the image
                        Image resizedImage = ResizeImage(image, maxWidth, maxHeight);

                        e.Value = resizedImage;
                    }
                    else
                    {
                        e.Value = null;
                    }
                }
                catch
                {
                    e.Value = null;
                }
            }
        }

        private void PickandPackForm_Load(object sender, EventArgs e)
        {

        }
    }
}