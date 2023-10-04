using DevExpress.XtraGrid.Views.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManager
{
    public static class GridCellFormatter
    {
        public static void FormatFilePathColumn(RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName == "FilePath")
            {
                string filePath = e.CellValue.ToString();
                if (File.Exists(filePath))
                {
                    e.Appearance.BackColor = Color.Green;
                }
                else
                {
                    e.Appearance.BackColor = Color.Red;
                }
            }
        }

        public static void FormatStatusColumn(RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName == "Status")
            {
                if (e.CellValue.ToString() == "OK")
                {
                    e.Appearance.ForeColor = Color.Green;
                }
                else
                {
                    e.Appearance.ForeColor = Color.Red;
                }
            }
        }
    }
}