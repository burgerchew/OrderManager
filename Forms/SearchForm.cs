using DevExpress.XtraEditors;
using OrderManagerEF.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace OrderManagerEF
{
    public partial class SearchForm : XtraForm
    {
        public SearchForm()
        {
            InitializeComponent();
        }

        private List<SearchResult> PerformSearch(string searchText)
        {
            List<SearchResult> searchResults = new List<SearchResult>();

            // Iterate through all the forms in the application
            foreach (Form form in Application.OpenForms)
            {
                // Check if the form contains a DevExpress GridView
                DevExpress.XtraGrid.GridControl gridControl = form.Controls.OfType<DevExpress.XtraGrid.GridControl>().FirstOrDefault();
                if (gridControl != null)
                {
                    // Get the GridView
                    DevExpress.XtraGrid.Views.Grid.GridView gridView = gridControl.MainView as DevExpress.XtraGrid.Views.Grid.GridView;

                    // Iterate through all rows in the GridView
                    for (int i = 0; i < gridView.RowCount; i++)
                    {
                        // Iterate through all columns in the GridView
                        for (int j = 0; j < gridView.Columns.Count; j++)
                        {
                            // Get the cell value
                            string cellValue = gridView.GetRowCellValue(i, gridView.Columns[j])?.ToString();

                            // Check if the cell value contains the search text
                            if (!string.IsNullOrEmpty(cellValue) && cellValue.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                // Add the search result to the list
                                searchResults.Add(new SearchResult
                                {
                                    Form = form,
                                    RowHandle = i,
                                    Column = gridView.Columns[j]
                                });
                            }
                        }
                    }
                }
            }

            return searchResults;
        }


        private void simpleButton1_Click_1(object sender, EventArgs e)
        {
            string searchText = textEdit1.Text;
            List<SearchResult> searchResults = PerformSearch(searchText);

            if (searchResults.Count > 0)
            {
                // Open the first form with search results and bring it to front
                SearchResult firstResult = searchResults[0];
                firstResult.Form.BringToFront();

                // Get the GridView
                DevExpress.XtraGrid.GridControl gridControl = firstResult.Form.Controls.OfType<DevExpress.XtraGrid.GridControl>().FirstOrDefault();
                DevExpress.XtraGrid.Views.Grid.GridView gridView = gridControl.MainView as DevExpress.XtraGrid.Views.Grid.GridView;

                // Set the focused row and column
                gridView.FocusedRowHandle = firstResult.RowHandle;
                gridView.FocusedColumn = firstResult.Column;

                // Highlight the entire row containing the search result
                gridView.ClearSelection(); // Clear any previous selection
                gridView.SelectRow(firstResult.RowHandle);

                // Subscribe to the CustomDrawCell event to set the background color of the row
                //gridView.CustomDrawCell += gridView_CustomDrawCell;

                // Refresh the GridView to apply the custom drawing
                gridView.RefreshRow(firstResult.RowHandle);
            }
            else
            {
                XtraMessageBox.Show("No results found.", "Search");
            }
        }
    }
}