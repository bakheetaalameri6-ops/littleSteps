using LittleSteps.Api.Data;
using LittleSteps.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
namespace LittleSteps.Api.Services;
public interface IAuditService { Task<object> Users(); void Record(string area, string action, string? details, string result = "OK", Staff? staff = null, string? username = null); Task<object> Search(DateTime? from, DateTime? to, int? staffId, string? area, int page, int pageSize, string? username = null); }
public sealed class AuditService(AppDbContext db) : IAuditService
{
    // Stage the entry in the caller's DbContext so the change and its audit commit together.
    public void Record(string area, string action, string? details, string result = "OK", Staff? staff = null, string? username = null)
    {
        var entry = new Domain.AuditEntry(staff?.StaffId, staff?.Username ?? username, area, action, details, result);
        db.AuditEntries.Add(new AuditEntry { OccurredAt = entry.OccurredAt, StaffId = entry.StaffId, Username = entry.Username, Area = entry.Area, Action = entry.Action, Details = entry.Details, Result = entry.Result });
    }
    public async Task<object> Users() => await db.Staff.AsNoTracking().OrderBy(s => s.FullName).Select(s => new { id = s.StaffId, s.FullName }).ToListAsync();
    public async Task<object> Search(DateTime? from, DateTime? to, int? staffId, string? area, int page, int pageSize, string? username = null)
    {
        var q = db.AuditEntries.AsNoTracking().AsQueryable(); if (from.HasValue) q = q.Where(a => a.OccurredAt >= from.Value); if (to.HasValue) q = q.Where(a => a.OccurredAt < to.Value); if (staffId.HasValue) q = q.Where(a => a.StaffId == staffId); if (!string.IsNullOrWhiteSpace(area)) q = q.Where(a => a.Area == area); if (!string.IsNullOrWhiteSpace(username)) q = q.Where(a => a.Username == username);
        page = Math.Max(1, page); pageSize = Math.Clamp(pageSize, 1, 100); var total = await q.CountAsync(); var items = await q.OrderByDescending(a => a.OccurredAt).ThenByDescending(a => a.AuditId).Skip((page - 1) * pageSize).Take(pageSize).Select(a => new { a.AuditId, a.OccurredAt, a.StaffId, a.Username, FullName = a.Staff != null ? a.Staff.FullName : a.Username, Role = a.Staff != null ? a.Staff.Role : null, a.Area, a.Action, a.Details, a.Result }).ToListAsync(); return new { items, total, page, pageSize };
    }
}
