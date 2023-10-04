using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraCharts;
using System.Data;

namespace OrderManagerEF.Classes
{
    public static class BarChartHelper
    {
        public static void SetupBarChart<T>(ChartControl chart, List<T> data, string argumentDataMember, string valueDataMember, string chartTitle, string appearanceName, bool showLabels, bool showLegend, int count)
        {
            // Bind the List to the Bar Chart control
            chart.DataSource = data;

            // Create a new series for the chart
            Series series = new Series("Number of Orders", ViewType.Bar);

            // Specify the value data member as the count of AccountingRef
            series.ValueDataMembers.AddRange(new string[] { valueDataMember });

            // Specify the argument data member
            series.ArgumentDataMember = argumentDataMember;

            // Add the series to the chart
            chart.Series.Add(series);

            // Set the chart title to include the count
            chart.Titles.Add(new ChartTitle() { Text = $"{chartTitle} ({count})" });

            // Cast the View property to SideBySideBarSeriesView and set the AggregateFunction property
            SideBySideBarSeriesView view = (SideBySideBarSeriesView)series.View;
            view.AggregateFunction = (SeriesAggregateFunction)DevExpress.XtraCharts.AggregateFunction.Count;

            // Customize the appearance of the chart
            chart.AppearanceName = appearanceName;
            chart.Legend.Visibility = showLegend ? DevExpress.Utils.DefaultBoolean.True : DevExpress.Utils.DefaultBoolean.False;

            // Show the total value of each bar by customizing the series label options
            if (showLabels)
            {
                series.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
                series.Label.ResolveOverlappingMode = ResolveOverlappingMode.Default;
                series.Label.TextPattern = "{V}"; // Display count on the bars
            }
        }
    }


}


