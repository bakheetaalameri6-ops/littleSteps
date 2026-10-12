using LittleSteps.Api.Data;
using LittleSteps.Api.Domain;
using LittleSteps.Api.ViewModels;
using Microsoft.EntityFrameworkCore;
using E=LittleSteps.Api.Data.Entities;
namespace LittleSteps.Api.Services;
public interface IGuardianService {Task<object> Search(string? emiratesId);Task<object> Detail(int id);Task<object> Save(int? id,GuardianRequest request,E.Staff actor);Task<byte[]?> Photo(int id);Task SetPhoto(int id,byte[]? photo,E.Staff actor);}
public sealed class GuardianService(AppDbContext db,IAuditService audit):IGuardianService
{
 public async Task<object> Search(string? emiratesId){var q=db.Guardians.AsNoTracking().AsQueryable();if(!string.IsNullOrWhiteSpace(emiratesId))q=q.Where(g=>g.EmiratesId==new EmiratesId(emiratesId).Value);return await q.OrderBy(g=>g.FullName).Select(g=>new{id=g.GuardianId,g.FullName,g.EmiratesId,g.Phone,photoUrl=g.Photo==null?null:"/guardians/"+g.GuardianId+"/photo"}).ToListAsync();}
 private async Task<E.Guardian> Find(int id)=>await db.Guardians.SingleOrDefaultAsync(g=>g.GuardianId==id)??throw new DomainException("Guardian not found.",404);
 public async Task<object> Detail(int id){var g=await Find(id);var links=await db.StudentGuardians.Include(l=>l.Student).ThenInclude(s=>s.Class).Where(l=>l.GuardianId==id).ToListAsync();return new{id=g.GuardianId,g.FullName,g.EmiratesId,g.Phone,photoUrl=g.Photo is null?null:$"/guardians/{id}/photo",children=links.Select(l=>new{student=StudentService.Summary(l.Student),l.Relationship})};}
 public async Task<object> Save(int? id,GuardianRequest r,E.Staff actor){var d=new Guardian(id??0,r.FullName,Rules.Text(r.Phone,"Phone",20),null,r.EmiratesId);var g=id.HasValue?await Find(id.Value):new E.Guardian{CreatedAt=DateTime.UtcNow};g.FullName=d.FullName;g.Phone=d.Phone!;g.EmiratesId=d.EmiratesId.Value;if(!id.HasValue)db.Guardians.Add(g);audit.Record("Guardians",id.HasValue?"Guardian edited":"Guardian created",g.FullName,staff:actor);await db.SaveChangesAsync();return new{id=g.GuardianId};}
 public async Task<byte[]?> Photo(int id)=>(await Find(id)).Photo;
 public async Task SetPhoto(int id,byte[]? photo,E.Staff actor){Rules.Photo(photo);var g=await Find(id);g.Photo=photo;audit.Record("Guardians",photo is null?"Photo removed":"Photo added",g.FullName,staff:actor);await db.SaveChangesAsync();}
}
public interface IDashboardService {Task<object> Summary();}
public sealed class DashboardService(AppDbContext db,SchoolClock clock):IDashboardService
{
 public async Task<object> Summary(){var today=clock.Today;var(start,end)=clock.Day(today);var enrolled=await db.Students.CountAsync(s=>s.IsActive);var present=await db.AttendanceRecords.CountAsync(a=>a.AttendanceDate==today&&a.Status=='P'&&a.Student.IsActive);var absent=await db.AttendanceRecords.CountAsync(a=>a.AttendanceDate==today&&a.Status=='A'&&a.Student.IsActive);var pending=await db.Students.Include(s=>s.Class).Where(s=>s.IsActive&&s.Photo==null).ToListAsync();var activity=await db.AuditEntries.OrderByDescending(a=>a.OccurredAt).Take(5).Select(a=>new{a.OccurredAt,a.Username,a.Action,a.Result}).ToListAsync();return new{enrolled,present,absent,notMarked=enrolled-present-absent,photosMissing=pending.Count,classes=await db.Classes.CountAsync(),rooms=await db.Rooms.CountAsync(),visitorsOnSite=await db.Visits.CountAsync(v=>v.TimeOut==null),earlyExits=await db.EarlyExits.CountAsync(e=>e.Authorized&&e.OccurredAt>=start&&e.OccurredAt<end),refused=await db.EarlyExits.CountAsync(e=>!e.Authorized&&e.OccurredAt>=start&&e.OccurredAt<end),pending=pending.Select(StudentService.Summary),activity};}
}
