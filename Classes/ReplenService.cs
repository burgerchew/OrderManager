using DevExpress.XtraEditors;
using OrderManagerEF.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OrderManagerEF.Data;
using OrderManagerEF.Entities;

namespace OrderManagerEF.Classes
{
    public class ReplenService
    {
        private readonly IConfiguration _configuration;
        private readonly OMDbContext _context;
        private readonly UserSession _userSession;
       


        // Dependency injection through constructor
        public ReplenService(IConfiguration configuration, OMDbContext context, UserSession _userSession)
        {
            _configuration = configuration;
                _context = context;
                _userSession = _userSession;

        }

        public void CreateReplenTransaction(ReplenishmentResult replenishmentResult, int warehouseId)
        {

            // Check for null arguments
            if (_context == null || replenishmentResult == null)
            {
                XtraMessageBox.Show("Arguments cannot be null.");
                return;
            }

            try
            {
                // Map ReplenishmentResult to ReplenHeader
                ReplenHeader replenHeader = new ReplenHeader
                {
                    ReplenDate = DateTime.Now,
                    TradingRef = replenishmentResult.AccountingRef,
                    WarehouseId = warehouseId,
                };

                // Map ReplenishmentResult to ReplenDetail
                ReplenDetail replenDetail = new ReplenDetail
                {
                    ProductCode = replenishmentResult.ProductCode,
                    Qty = (int)(replenishmentResult.BulkQty + replenishmentResult.RetailQty),
                    FromLocation = replenishmentResult.BulkBin,
                    ToLocation = replenishmentResult.RetailBin
                };

                // Add to the corresponding collections
                replenHeader.ReplenDetails.Add(replenDetail);

                // Add to DbContext
                _context.ReplenHeaders.Add(replenHeader);

                // Save changes
                _context.SaveChanges();

                XtraMessageBox.Show("Data successfully copied.");
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
    }
}
