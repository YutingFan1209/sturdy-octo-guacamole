namespace AssetAccessManager.Domain;

public enum AssetStatus { Available, Assigned, Retired }

public sealed class Asset
{
    private Asset() { }
    private Asset(Guid id, string assetTag, string serialNumber, string model)
    { Id = id; AssetTag = Required(assetTag); SerialNumber = Required(serialNumber); Model = Required(model); Status = AssetStatus.Available; }
    public Guid Id { get; private set; }
    public string AssetTag { get; private set; } = string.Empty;
    public string SerialNumber { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public AssetStatus Status { get; private set; }
    public long Version { get; private set; }
    public static Asset Register(string assetTag, string serialNumber, string model) => new(Guid.NewGuid(), assetTag, serialNumber, model);
    public static Asset RegisterSeed(Guid id, string assetTag, string serialNumber, string model) => new(id, assetTag, serialNumber, model);
    public void MarkAssigned() { if (Status != AssetStatus.Available) throw new InvalidOperationException("Only available assets can be assigned."); Status = AssetStatus.Assigned; Version++; }
    public void MarkReturned() { if (Status != AssetStatus.Assigned) throw new InvalidOperationException("Only assigned assets can be returned."); Status = AssetStatus.Available; Version++; }
    private static string Required(string value) => string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("Value is required.") : value.Trim();
}

public sealed class AssetAssignment
{
    private AssetAssignment() { }
    public AssetAssignment(Guid id, Guid assetId, string employeeId, string assignedBy, DateTimeOffset assignedAt)
    { Id = id; AssetId = assetId; EmployeeId = employeeId; AssignedBy = assignedBy; AssignedAt = assignedAt; }
    public Guid Id { get; private set; }
    public Guid AssetId { get; private set; }
    public string EmployeeId { get; private set; } = string.Empty;
    public string AssignedBy { get; private set; } = string.Empty;
    public DateTimeOffset AssignedAt { get; private set; }
    public DateTimeOffset? ReturnedAt { get; private set; }
    public string? ReturnedBy { get; private set; }
    public void Return(string actor, DateTimeOffset now) { if (ReturnedAt is not null) throw new InvalidOperationException("Assignment is already closed."); ReturnedAt = now; ReturnedBy = actor; }
}
