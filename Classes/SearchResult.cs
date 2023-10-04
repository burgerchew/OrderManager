using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrderManager.Classes
{


        public class SearchResult
        {
            public Form Form { get; set; }
            public int RowHandle { get; set; }
            public DevExpress.XtraGrid.Columns.GridColumn Column { get; set; }
        }

    
}
