using Microsoft.EntityFrameworkCore;
using RenewableAsset.WorkOrders.Api.Models;

namespace RenewableAsset.WorkOrders.Api.Data;

public sealed class WorkOrdersDbContext(DbContextOptions<WorkOrdersDbContext> options) : DbContext(options)
{
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var workOrder = modelBuilder.Entity<WorkOrder>();
        workOrder.HasKey(order => order.Id);
        workOrder.HasIndex(order => order.WorkOrderNumber).IsUnique();
        workOrder.Property(order => order.WorkOrderNumber).HasMaxLength(30).IsRequired();
        workOrder.Property(order => order.AssetName).HasMaxLength(120).IsRequired();
        workOrder.Property(order => order.AssetType).HasMaxLength(60).IsRequired();
        workOrder.Property(order => order.Site).HasMaxLength(120).IsRequired();
        workOrder.Property(order => order.Priority).HasMaxLength(20).IsRequired();
        workOrder.Property(order => order.Status).HasMaxLength(30).IsRequired();
        workOrder.Property(order => order.Description).HasMaxLength(1000);
        workOrder.Property(order => order.EstimatedCost).HasPrecision(12, 2);
    }
}
