
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderManagerEF.Data;

namespace OrderManagerEF.Classes
{
    public class StoredProcedureService
    {
        private readonly OMDbContext _context;

        public StoredProcedureService(OMDbContext context)
        {
            _context = context;
        }

        public List<ASP_SSI_Result> ExecuteStoredProcedure(string storedProcedureName)
        {
            if (string.IsNullOrEmpty(storedProcedureName))
                throw new ArgumentNullException(nameof(storedProcedureName));

            return _context.Set<ASP_SSI_Result>().FromSqlRaw($"EXEC [dbo].[{storedProcedureName}]").ToList();
        }
    }

}
