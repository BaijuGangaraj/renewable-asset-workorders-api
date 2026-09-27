namespace RenewableAsset.WorkOrders.Api.Models;

public sealed class WorkOrderInput
{
    public string WorkOrderNumber { get; set; } = string.Empty;
    public string AssetName { get; set; } = string.Empty;
    public string AssetType { get; set; } = string.Empty;
    public string Site { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ReportedDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal EstimatedCost { get; set; }
}
