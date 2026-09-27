using RenewableAsset.WorkOrders.Api.Models;

namespace RenewableAsset.WorkOrders.Api.Data;

public static class DemoData
{
    public static void Seed(WorkOrdersDbContext db)
    {
        if (db.WorkOrders.Any()) return;

        var now = DateTime.UtcNow;
        db.WorkOrders.AddRange(
            new WorkOrder
            {
                WorkOrderNumber = "WO-2026-1001",
                AssetName = "Cedar Ridge Turbine 14",
                AssetType = "Wind Turbine",
                Site = "Cedar Ridge Wind Farm",
                Priority = "High",
                Status = "Open",
                Description = "Investigate vibration alert reported by condition monitoring.",
                ReportedDate = DateTime.Today.AddDays(-1),
                DueDate = DateTime.Today.AddDays(2),
                EstimatedCost = 2400m,
                CreatedUtc = now,
                UpdatedUtc = now
            },
            new WorkOrder
            {
                WorkOrderNumber = "WO-2026-1002",
                AssetName = "Sunrise Inverter Block B",
                AssetType = "Solar Inverter",
                Site = "Sunrise Solar Facility",
                Priority = "Medium",
                Status = "In Progress",
                Description = "Replace DC isolator after intermittent fault event.",
                ReportedDate = DateTime.Today.AddDays(-3),
                DueDate = DateTime.Today.AddDays(4),
                EstimatedCost = 850m,
                CreatedUtc = now,
                UpdatedUtc = now
            },
            new WorkOrder
            {
                WorkOrderNumber = "WO-2026-1003",
                AssetName = "Gulf Storage Battery Rack 07",
                AssetType = "Battery Energy Storage",
                Site = "Gulf Storage Hub",
                Priority = "Low",
                Status = "Completed",
                Description = "Quarterly cooling-system inspection completed.",
                ReportedDate = DateTime.Today.AddDays(-14),
                DueDate = DateTime.Today.AddDays(-7),
                EstimatedCost = 300m,
                CreatedUtc = now,
                UpdatedUtc = now
            });

        db.SaveChanges();
    }
}
