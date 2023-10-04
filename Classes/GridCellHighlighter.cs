using System.Drawing;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

public static class GridCellHighlighter
{
    public static void HighlightCells(GridControl gridControl, string columnName, string trueValue, Color trueBackColor, Color trueForeColor, string falseValue, Color falseBackColor, Color falseForeColor)
    {
        GridView gridView = gridControl.MainView as GridView;
        gridView.RowCellStyle += (sender, e) =>
        {
            if (e.Column.FieldName == columnName)
            {
                string cellValue = gridView.GetRowCellValue(e.RowHandle, e.Column).ToString();
                string displayText = gridView.GetDisplayTextByColumnValue(e.Column, cellValue);
                if (displayText == trueValue)
                {
                    e.Appearance.BackColor = trueBackColor;
                    e.Appearance.ForeColor = trueForeColor;
                }
                else if (displayText == falseValue)
                {
                    e.Appearance.BackColor = falseBackColor;
                    e.Appearance.ForeColor = falseForeColor;
                }
            }
        };
    }
}