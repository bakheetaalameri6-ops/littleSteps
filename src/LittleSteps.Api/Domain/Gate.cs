namespace LittleSteps.Api.Domain;
public sealed class Visit
{
    public string VisitorName { get; }
    public EmiratesId VisitorEmiratesId { get; }
    public string Reason { get; }
    public string PersonVisited { get; }
    public DateTime TimeIn { get; }
    public DateTime? TimeOut { get; private set; }
    public bool IsOnSite => TimeOut is null;
    public Visit(string name, string id, string reason, string person, DateTime timeIn, DateTime? timeOut = null) { VisitorName = Rules.Text(name, "Visitor name"); VisitorEmiratesId = new(id); if (!new[] { "Meeting", "Delivery", "Maintenance", "Parent visit", "Other" }.Contains(reason)) throw new DomainException("Choose a valid visit reason."); Reason = reason; PersonVisited = Rules.Text(person, "Person visited"); TimeIn = timeIn; TimeOut = timeOut; }
    public void CheckOut(DateTime now) { if (!IsOnSite) throw new DomainException("This visitor has already checked out.", 409); if (now < TimeIn) throw new DomainException("Check-out cannot precede arrival."); TimeOut = now; }
}
public sealed class EarlyExitRequest(Student student, EmiratesId presentedId, Guardian? guardian)
{
    public Student Student { get; } = student; public EmiratesId PresentedId { get; } = presentedId; public Guardian? Guardian { get; private set; } = guardian; public bool Authorized { get; private set; }
    public string? Reason { get; private set; }
    public bool Check() => Guardian is not null && Guardian.EmiratesId == PresentedId && Student.IsLinkedTo(PresentedId);
    // Confirmation rechecks the relationship on the server; an earlier browser result grants no rights.
    public void Authorize(string reason) { if (!Check()) throw new DomainException("This adult is not a registered responsible adult.", 403); if (!new[] { "Medical appointment", "Family reason", "Feeling unwell", "Other" }.Contains(reason)) throw new DomainException("Choose an early-exit reason."); Reason = reason; Authorized = true; }
    public void Refuse() { if (Check()) throw new DomainException("A linked guardian cannot be recorded as unlinked."); Guardian = null; Authorized = false; Reason = null; }
}
