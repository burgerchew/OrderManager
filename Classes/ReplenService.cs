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

        // Method for creating a single transaction from multiple ReplenishmentResults
        public void CreateReplenTransaction(List<ReplenishmentResult> replenishmentResults, int warehouseId)
        {
            if (_context == null || replenishmentResults == null)
            {
                XtraMessageBox.Show("Arguments cannot be null.");
                return;
            }

            try
            {
                // Create a single ReplenHeader for all the ReplenishmentResults
                ReplenHeader replenHeader = new ReplenHeader
                {
                    ReplenDate = DateTime.Now,
                    TradingRef = "Replen-" + DateTime.Now.ToString("yyyyMMdd"),
                    WarehouseId = warehouseId,
                };

                // Create ReplenDetails for each ReplenishmentResult
                foreach (var replenishmentResult in replenishmentResults)
                {
                    // Skip null records
                    if (replenishmentResult == null)
                    {
                        continue;
                    }

                    ReplenDetail replenDetail = new ReplenDetail
                    {
                        ProductCode = replenishmentResult.ProductCode,
                        // Check for null and replace with a default value (0 here)
                        Qty = (int)((replenishmentResult.BulkQty ?? 0) + (replenishmentResult.RetailQty ?? 0)),
                        // Use null-coalescing to default to an empty string if the property is null
                        FromLocation = replenishmentResult.BulkBin ?? string.Empty,
                        ToLocation = replenishmentResult.RetailBin ?? string.Empty
                    };

                    replenHeader.ReplenDetails.Add(replenDetail);
                }


                // Add to DbContext and Save
                _context.ReplenHeaders.Add(replenHeader);
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
