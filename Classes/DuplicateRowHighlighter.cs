using System.Collections.Generic;
using System.Drawing;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base;
using System.Text;

namespace OrderManager.Classes
{
    public class DuplicateRowHighlighter
    {
        public void HighlightDuplicates(GridView gridView)
        {
            gridView.CustomDrawCell += GridView_CustomDrawCell;
        }

        private void GridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName == "TradingRef")
            {
                GridView gridView = sender as GridView;
                string tradingRef = gridView.GetRowCellValue(e.RowHandle, "TradingRef").ToString();

                if (IsDuplicate(tradingRef, gridView, e.RowHandle))
                {
                    e.Appearance.BackColor = Color.Red;
                    e.Appearance.ForeColor = Color.White;
                }
                else
                {

                    e.Appearance.ForeColor = Color.Black;
                }
            }
        }

        private bool IsDuplicate(string tradingRef, GridView gridView, int currentRowHandle)
        {
            int count = 0;

            for (int i = 0; i < gridView.RowCount; i++)
            {
                object valueObj = gridView.GetRowCellValue(i, "TradingRef");
                if (valueObj == null) continue; // Skip if the value is null

                string value = valueObj.ToString();

                if (value == tradingRef)
                {
                    count++;
                }

                if (count > 1)
                {
                    return true;
                }
            }

            return false;
        }


        public void FilterDuplicates(GridView gridView)
        {
            gridView.ActiveFilter.Clear();
            gridView.ActiveFilterString = $"[TradingRef] In ({GetDuplicateTradingRefs(gridView)})";
        }

        private string GetDuplicateTradingRefs(GridView gridView)
        {
            Dictionary<string, int> tradingRefCount = new Dictionary<string, int>();

            for (int i = 0; i < gridView.RowCount; i++)
            {
                string value = gridView.GetRowCellValue(i, "TradingRef").ToString();

                if (tradingRefCount.ContainsKey(value))
                {
                    tradingRefCount[value]++;
                }
                else
                {
                    tradingRefCount[value] = 1;
                }
            }

            StringBuilder duplicates = new StringBuilder();

            foreach (var kvp in tradingRefCount)
            {
                if (kvp.Value > 1)
                {
                    if (duplicates.Length > 0)
                    {
                        duplicates.Append(", ");
                    }
                    duplicates.Append($"'{kvp.Key}'");
                }
            }

            return duplicates.ToString();
        }

    }
}