using DevExpress.LookAndFeel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagerEF.DTOs;
using OrderManagerEF.DTOs.OrderManager.Classes;
using OrderManagerEF.Entities;

namespace OrderManagerEF.Data
{

    public class OMDbContext : DbContext
    {
        public OMDbContext(DbContextOptions<OMDbContext> options)
            : base(options)
        {
        }

        //public DbSet<LabelsArchive> LabelArchives { get; set; }

        public DbSet<Label> Labels { get; set; }

        public DbSet<StarShipITOrder> StarShipITOrders { get; set; }
        public DbSet<StarShipITOrderDetail> StarShipITOrderDetails { get; set; }

        //public DbSet<AddressPart> AddressParts { get; set; }


        //public virtual DbSet<ScanPackReportLookup> ScanPackReportLookups { get; set; }

        //public List<ScanPackReportLookup> ExecuteScanPackReportLookup(string searchTerm)
        //{
        //    var searchTermParam = new SqlParameter("@SearchTerm", searchTerm);
        //    return ScanPackReportLookups.FromSqlRaw("EXEC sp_ABM_ScanPackReportLookup @SearchTerm", searchTermParam)
        //        .ToList();
        //}

        //public DbSet<PendingBatch> PendingBatches { get; set; }

        //public DbSet<BINContentsLocn1> BINContentsLocn1s { get; set; }
        //public DbSet<BINContentsLocn11> BINContentsLocn11s { get; set; }

        //public DbSet<LabelPrintQueue> LabelPrintQueues { get; set; }

        public DbSet<CSCOrderData> CscOrderDatas { get; set; }

        public DbSet<DSOrderData> DSOrderDatas { get; set; }

        public DbSet<PreOrderData> PreOrderDatas { get; set; }

        public DbSet<SampleOrderData> SampleOrderDatas { get; set; }

        public DbSet<RubiesOrderData> RubiesOrderDatas { get; set; }

        public DbSet<RubiesOver5OrderData> RubiesOver5OrderDatas { get; set; }

        public DbSet<NZOrderData> NzOrderDatas { get; set; }

        public DbSet<PrintedOrderData> PrintedOrderDatas { get; set; }

        public DbSet<HoldOrderData> HoldOrderDatas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // For Views (Keyless Entities)
            modelBuilder.Entity<PendingBatch>().ToView("vPendingBatches").HasNoKey();
            //modelBuilder.Entity<LabelPrintQueue>().ToView("vLabelPrintQueue").HasNoKey();
            //modelBuilder.Entity<BINContentsLocn1>().ToView("vBINContents_Locn1").HasNoKey();
            //modelBuilder.Entity<BINContentsLocn11>().ToView("vBINContents_Locn11").HasNoKey();

            modelBuilder.Entity<CSCOrderData>().ToView("vASP_CSC_SI").HasNoKey();
            modelBuilder.Entity<DSOrderData>().ToView("vASP_DS_SSI").HasNoKey();
            modelBuilder.Entity<PreOrderData>().ToView("vASP_PREORDER_SSI").HasNoKey();
            modelBuilder.Entity<SampleOrderData>().ToView("vASP_SSI_SAMPLES").HasNoKey();
            modelBuilder.Entity<RubiesOrderData>().ToView("vASP_SSI_RUB").HasNoKey();
            modelBuilder.Entity<RubiesOver5OrderData>().ToView("vASP_SSI_RUBOVER5").HasNoKey();
            modelBuilder.Entity<NZOrderData>().ToView("vASP_SSI_NZ").HasNoKey();
            modelBuilder.Entity<HoldOrderData>().ToView("vASP_SSI_HOLD").HasNoKey();
            modelBuilder.Entity<PrintedOrderData>().ToView("vASP_SSI_COMPLETE").HasNoKey();

            modelBuilder.Entity<StarShipITOrder>()
                .HasMany(s => s.StarShipITOrderDetails)
                .WithOne(sd => sd.StarShipITOrder)
                .HasForeignKey(sd => sd.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
