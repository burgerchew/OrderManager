using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using OrderManager.Data;

namespace OrderManager.Classes
{
    public class PickSlipGenerator
    {
        private readonly IConfiguration _configuration;
        private readonly OMDbContext _context;

        public PickSlipGenerator(IConfiguration configuration, OMDbContext context)
        {
            _context = context;
            _configuration = configuration;
        }

        public int GeneratePickSlipsForGroups(IEnumerable<string> customerGroups)
        {
            int totalRowsInserted = 0;

            Dictionary<string, string> customerGroupDict = customerGroups.ToDictionary(group => group, group => group);

            MergeTable(customerGroupDict);

            foreach (var group in customerGroups)
            {
                totalRowsInserted += GeneratePickSlipsForGroup(group);
            }
            return totalRowsInserted;
        }

        private void TruncateTable()
        {
            // EF Core way of executing raw SQL commands
            _context.Database.ExecuteSqlRaw("TRUNCATE TABLE ASPPickSlipData");
        }

        public void MergeTable(Dictionary<string, string> customerGroups)
        {
            foreach (var customerGroup in customerGroups)
            {
                _context.Database.ExecuteSqlRaw("[dbo].[ASP_MergeIntoASPPickSlipData] @CustomerGroup",
                    new SqlParameter("@CustomerGroup", customerGroup.Value));
            }
        }

        private int GeneratePickSlipsForGroup(string customerGroup)
        {
            // EF Core way of executing stored procedures and getting result
            var rowsInserted = _context.Database.ExecuteSqlRaw("[dbo].[ASP_PickSlipGenerator] @CustomerGroup",
                new SqlParameter("@CustomerGroup", customerGroup));

            return rowsInserted; // Return the number of rows affected
        }
    }
}

