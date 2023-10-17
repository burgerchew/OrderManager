using DevExpress.LookAndFeel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using OrderManagerEF.Classes;
using OrderManagerEF.DTOs;
using OrderManagerEF.Entities;
using Label = OrderManagerEF.Entities.Label;
using PendingBatch = OrderManagerEF.DTOs.PendingBatch;

namespace OrderManagerEF.Data
{

    public class OMDbContext : DbContext
    {
        public OMDbContext(DbContextOptions<OMDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<UserActivity> UserActivities { get; set; }

        public DbSet<LabelsArchive> LabelArchives { get; set; }

        public DbSet<Label> Labels { get; set; }

        public DbSet<StarShipITOrder> StarShipITOrders { get; set; }
        public DbSet<StarShipITOrderDetail> StarShipITOrderDetails { get; set; }

        public DbSet<ReplenHeader> ReplenHeaders { get; set; }
        public DbSet<ReplenDetail> ReplenDetails { get; set; }

        public DbSet<AddressPart> AddressParts { get; set; }

        public DbSet<StarShipITAPIKeyManager> StarShipITAPIKeyManager { get; set; }

        public virtual DbSet<ScanPackReportLookup> ScanPackReportLookups { get; set; }

        public virtual DbSet<ReplenishmentResult> ReplenishmentResults { get; set; }

        public List<ScanPackReportLookup> ExecuteScanPackReportLookup(string searchTerm)
        {
            var searchTermParam = new SqlParameter("@SearchTerm", searchTerm);
            var result = ScanPackReportLookups.FromSqlRaw("EXEC sp_ABM_ScanPackReportLookup @SearchTerm", searchTermParam)
                .ToList();

            if (result == null || !result.Any()) // Check if the result is null or empty
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("No data found for the given search term.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return new List<ScanPackReportLookup>(); // Return an empty list instead of null to prevent potential NullReferenceExceptions elsewhere
            }

            return result;
        }



        public DbSet<ScanPackReportOverview> vScanPackReportOverviews { get; set; }

        public async Task<List<ReplenishmentResult>> GetReplenishmentDataAsync(int sourceLocationNo, string orderType, int dateRange, int retailBinThreshold)
        {
            var sourceLocationNoParam = new SqlParameter("@SourceLocationNo", sourceLocationNo);
            var orderTypeParam = new SqlParameter("@OrderType", orderType);
            var dateRangeParam = new SqlParameter("@DateRange", dateRange);
            var retailBinThresholdParam = new SqlParameter("@RetailBinThreshold", retailBinThreshold);

            var result = await ReplenishmentResults.FromSqlRaw("EXEC sp_ASP_MasterReplenCTE @SourceLocationNo, @OrderType, @DateRange, @RetailBinThreshold",
                    sourceLocationNoParam, orderTypeParam, dateRangeParam, retailBinThresholdParam)
                .ToListAsync();

            if (result == null || !result.Any()) // Check if the result is null or empty
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("No data found for the given parameters.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<ReplenishmentResult>(); // Return an empty list to prevent potential issues elsewhere
            }

            return result;
        }



        public virtual DbSet<OrderLookupResult> OrderLookupResults { get; set; }

        public List<OrderLookupResult> ExecuteOrderLookupResult(string searchTerm)
        {
            var searchTermParam = new SqlParameter("@SearchTerm", searchTerm);
            var result = OrderLookupResults.FromSqlRaw("EXEC ASP_OrderCheckBinParam @SearchTerm", searchTermParam)
                .ToList();

            if (result == null || !result.Any()) // Check if the result is null or empty
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("No data found for the given search term.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<OrderLookupResult>(); // Return an empty list to prevent potential issues elsewhere
            }

            return result;
        }


        public DbSet<PendingBatch> PendingBatches { get; set; }

        public DbSet<BINContentsLocn1> BINContentsLocn1s { get; set; }
        public DbSet<BINContentsLocn11> BINContentsLocn11s { get; set; }

        public DbSet<LabelPrintQueue> LabelPrintQueues { get; set; }

        public DbSet<CSCOrderData> CscOrderDatas { get; set; }

        public DbSet<DSOrderData> DSOrderDatas { get; set; }

        public DbSet<PreOrderData> PreOrderDatas { get; set; }

        public DbSet<SampleOrderData> SampleOrderDatas { get; set; }

        public DbSet<RubiesOrderData> RubiesOrderDatas { get; set; }

        public DbSet<RubiesOver5OrderData> RubiesOver5OrderDatas { get; set; }

        public DbSet<NZOrderData> NzOrderDatas { get; set; }

        public DbSet<PrintedOrderData> PrintedOrderDatas { get; set; }

        public DbSet<HoldOrderData> HoldOrderDatas { get; set; }

        public DbSet<CustomerResult> CustomerResults { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // For Views (Keyless Entities)
            modelBuilder.Entity<PendingBatch>().ToView("vPendingBatches").HasNoKey();
            modelBuilder.Entity<LabelPrintQueue>().ToView("vLabelPrintQueue").HasNoKey();
            modelBuilder.Entity<ScanPackReportOverview>().ToView("vScanPackReportOverview").HasNoKey();
            modelBuilder.Entity<ScanPackReportLookup>().HasNoKey();
            modelBuilder.Entity<ReplenishmentResult>().HasNoKey();
            modelBuilder.Entity<OrderLookupResult>().HasNoKey();
            modelBuilder.Entity<CustomerResult>().ToView("ASP_vGetCustomerList").HasNoKey();
            modelBuilder.Entity<BINContentsLocn1>().ToView("vBINContents_Locn1").HasNoKey();
            modelBuilder.Entity<BINContentsLocn11>().ToView("vBINContents_Locn11").HasNoKey();
            modelBuilder.Entity<AddressPart>().ToView("vAddressParts").HasNoKey();
            modelBuilder.Entity<CSCOrderData>().ToView("vASP_CSC_SSI").HasNoKey();
            modelBuilder.Entity<DSOrderData>().ToView("vASP_DS_SSI").HasNoKey();
            modelBuilder.Entity<PreOrderData>().ToView("vASP_PREORDERS_SSI").HasNoKey();
            modelBuilder.Entity<SampleOrderData>().ToView("vASP_SAMPLES_SSI").HasNoKey();
            modelBuilder.Entity<RubiesOrderData>().ToView("vASP_RUB_SSI").HasNoKey();
            modelBuilder.Entity<RubiesOver5OrderData>().ToView("vASP_RUB_SSI_OVER5").HasNoKey();
            modelBuilder.Entity<NZOrderData>().ToView("vASP_NZ_SSI").HasNoKey();
            modelBuilder.Entity<HoldOrderData>().ToView("vASP_HOLD_SSI").HasNoKey();
            modelBuilder.Entity<PrintedOrderData>().ToView("vASP_COMPLETE_SSI").HasNoKey();

            modelBuilder.Entity<StarShipITOrder>()
                .HasMany(s => s.StarShipITOrderDetails)
                .WithOne(sd => sd.StarShipITOrder)
                .HasForeignKey(sd => sd.OrderId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<ReplenHeader>()
                .HasMany(r => r.ReplenDetails)
                .WithOne(rd => rd.ReplenHeader)
                .HasForeignKey(rd => rd.ReplenId)
                .OnDelete(DeleteBehavior.Cascade);
        }



    }
}
