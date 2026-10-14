namespace LittleSteps.Api.Domain;
public sealed class AuditEntry
{
    public DateTime OccurredAt { get; }
    public int? StaffId { get; }
    public string? Username { get; }
    public string Area { get; }
    public string Action { get; }
    public string? Details { get; }
    public string Result { get; }
    public AuditEntry(int? staffId, string? username, string area, string action, string? details, string result) { if (!new[] { "Access", "Staff", "Classes", "Students", "Guardians", "Attendance", "Gate control" }.Contains(area)) throw new DomainException("Invalid audit area."); if (!new[] { "OK", "Refused", "Incomplete" }.Contains(result)) throw new DomainException("Invalid audit result."); OccurredAt = DateTime.UtcNow; StaffId = staffId; Username = username; Area = area; Action = Rules.Text(action, "Action", 40); Details = details; Result = result; }
}
