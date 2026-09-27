using Microsoft.EntityFrameworkCore;
using RenewableAsset.WorkOrders.Api.Data;
using RenewableAsset.WorkOrders.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WorkOrdersDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("WorkOrders")
        ?? "Data Source=workorders.db"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WorkOrdersDbContext>();
    db.Database.EnsureCreated();
    DemoData.Seed(db);
}

app.UseDefaultFiles();
app.UseStaticFiles();

var workOrders = app.MapGroup("/api/workorders");

workOrders.MapGet("", async (string? status, string? site, WorkOrdersDbContext db) =>
{
    var query = db.WorkOrders.AsNoTracking().AsQueryable();

    if (!string.IsNullOrWhiteSpace(status))
        query = query.Where(order => order.Status == status);

    if (!string.IsNullOrWhiteSpace(site))
        query = query.Where(order => order.Site.Contains(site));

    var results = await query
        .OrderByDescending(order => order.UpdatedUtc)
        .ToListAsync();

    return Results.Ok(results);
});

workOrders.MapGet("/{id:int}", async (int id, WorkOrdersDbContext db) =>
{
    var order = await db.WorkOrders.AsNoTracking().FirstOrDefaultAsync(order => order.Id == id);
    return order is null ? Results.NotFound() : Results.Ok(order);
});

workOrders.MapPost("", async (WorkOrderInput input, WorkOrdersDbContext db) =>
{
    var validation = Validate(input);
    if (validation is not null) return validation;

    var order = new WorkOrder();
    ApplyInput(order, input);
    order.CreatedUtc = DateTime.UtcNow;
    order.UpdatedUtc = DateTime.UtcNow;

    db.WorkOrders.Add(order);
    await db.SaveChangesAsync();

    return Results.Created($"/api/workorders/{order.Id}", order);
});

workOrders.MapPut("/{id:int}", async (int id, WorkOrderInput input, WorkOrdersDbContext db) =>
{
    var validation = Validate(input);
    if (validation is not null) return validation;

    var order = await db.WorkOrders.FindAsync(id);
    if (order is null) return Results.NotFound();

    ApplyInput(order, input);
    order.UpdatedUtc = DateTime.UtcNow;
    await db.SaveChangesAsync();

    return Results.Ok(order);
});

workOrders.MapDelete("/{id:int}", async (int id, WorkOrdersDbContext db) =>
{
    var order = await db.WorkOrders.FindAsync(id);
    if (order is null) return Results.NotFound();

    db.WorkOrders.Remove(order);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapFallbackToFile("index.html");
app.Run();

static IResult? Validate(WorkOrderInput input)
{
    var errors = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(input.WorkOrderNumber)) errors["workOrderNumber"] = ["Work order number is required."];
    if (string.IsNullOrWhiteSpace(input.AssetName)) errors["assetName"] = ["Asset name is required."];
    if (string.IsNullOrWhiteSpace(input.AssetType)) errors["assetType"] = ["Asset type is required."];
    if (string.IsNullOrWhiteSpace(input.Site)) errors["site"] = ["Site is required."];
    if (string.IsNullOrWhiteSpace(input.Priority)) errors["priority"] = ["Priority is required."];
    if (string.IsNullOrWhiteSpace(input.Status)) errors["status"] = ["Status is required."];
    if (input.DueDate.Date < input.ReportedDate.Date) errors["dueDate"] = ["Due date cannot be before the reported date."];
    if (input.EstimatedCost < 0) errors["estimatedCost"] = ["Estimated cost cannot be negative."];

    return errors.Count == 0 ? null : Results.ValidationProblem(errors);
}

static void ApplyInput(WorkOrder order, WorkOrderInput input)
{
    order.WorkOrderNumber = input.WorkOrderNumber.Trim();
    order.AssetName = input.AssetName.Trim();
    order.AssetType = input.AssetType.Trim();
    order.Site = input.Site.Trim();
    order.Priority = input.Priority.Trim();
    order.Status = input.Status.Trim();
    order.Description = input.Description?.Trim() ?? string.Empty;
    order.ReportedDate = input.ReportedDate.Date;
    order.DueDate = input.DueDate.Date;
    order.EstimatedCost = input.EstimatedCost;
}
