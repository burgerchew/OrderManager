using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagerEF.Data
{
    public class OMDbContextFactory : IDesignTimeDbContextFactory<OMDbContext>
    {
        public OMDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OMDbContext>();
            optionsBuilder.UseSqlServer("Data Source=src0997b355b\\ABM;Initial Catalog=Rubies-live;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;");

            return new OMDbContext(optionsBuilder.Options);
        }
    }
}
