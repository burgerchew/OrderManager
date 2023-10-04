using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraGrid.Views.Grid;

namespace OrderManagerEF.Classes
{


    public class GridHelper
    {
        public static void ExpandAllGroups(GridView view)
        {
            view.BeginUpdate();
            try
            {
                view.ExpandAllGroups();
            }
            finally
            {
                view.EndUpdate();
            }
        }
    }

}
