using LittleSteps.Api.Data;
using LittleSteps.Api.Domain;
using LittleSteps.Api.Mapping;
using LittleSteps.Api.Security;
using LittleSteps.Api.ViewModels;
using Microsoft.EntityFrameworkCore;
using E=LittleSteps.Api.Data.Entities;
namespace LittleSteps.Api.Services;
public interface IStaffService {Task<object> List(string? q,string? role);Task<object> Save(int? id,StaffRequest request,E.Staff actor);Task Deactivate(int id,E.Staff actor);}
public sealed class StaffService(AppDbContext db,IAuditService audit):IStaffService
{
    public async Task<object> List(string? q,string? role){var query=db.Staff.AsNoTracking().AsQueryable();if(!string.IsNullOrWhiteSpace(q))query=query.Where(s=>EF.Functions.ILike(s.FullName,"%"+q+"%")||EF.Functions.ILike(s.Username,"%"+q+"%"));if(!string.IsNullOrWhiteSpace(role))query=query.Where(s=>s.Role==role);return await query.OrderBy(s=>s.FullName).Select(s=>new{id=s.StaffId,s.FullName,s.Role,s.Username,s.Phone,s.IsActive,s.MustChangePassword,classId=s.Class!=null?(int?)s.Class.ClassId:null,className=s.Class!=null?s.Class.Name:null}).ToListAsync();}
    public async Task<object> Save(int? id,StaffRequest r,E.Staff actor){
        if(!new[]{"Admin","Teacher","SecurityGuard","Manager"}.Contains(r.Role))throw new DomainException("Choose a valid staff role.");
        var staff=id.HasValue?await db.Staff.Include(s=>s.Class).SingleOrDefaultAsync(s=>s.StaffId==id.Value)??throw new DomainException("Staff member not found.",404):new E.Staff{IsActive=true,CreatedAt=DateTime.UtcNow};
        if(staff.Class is not null && r.Role!="Teacher")throw new DomainException("Unassign this teacher's class before changing their role.",409);
        if(id==actor.StaffId && r.Role!=actor.Role)throw new DomainException("You cannot change your own role.",409);
        if(id.HasValue && staff.Role=="Admin" && r.Role!="Admin" && await db.Staff.CountAsync(s=>s.IsActive&&s.Role=="Admin")<=1)throw new DomainException("Keep at least one active administrator.",409);
        staff.FullName=Rules.Text(r.FullName,"Full name");staff.Role=r.Role;staff.Username=Rules.Text(r.Username,"Username",50);staff.Phone=r.Phone;
        if(!id.HasValue){var(hash,salt)=PasswordHasher.Create(r.TemporaryPassword??"");staff.PasswordHash=hash;staff.PasswordSalt=salt;staff.MustChangePassword=true;db.Staff.Add(staff);}
        audit.Record("Staff",id.HasValue?"Staff edited":"Staff created",staff.FullName+" · "+staff.Role,staff:actor);await db.SaveChangesAsync();return new{id=staff.StaffId};
    }
    public async Task Deactivate(int id,E.Staff actor){if(id==actor.StaffId)throw new DomainException("You cannot deactivate your own account.",409);var staff=await db.Staff.Include(s=>s.Class).SingleOrDefaultAsync(s=>s.StaffId==id)??throw new DomainException("Staff member not found.",404);if(!staff.IsActive)throw new DomainException("Account is already inactive.",409);if(staff.Role=="Admin"&&await db.Staff.CountAsync(s=>s.IsActive&&s.Role=="Admin")<=1)throw new DomainException("Keep at least one active administrator.",409);var domain=StaffFactory.FromEntity(staff);domain.Deactivate();staff.IsActive=domain.IsActive;if(staff.Class is not null)staff.Class.TeacherId=null;var sessions=await db.Sessions.Where(s=>s.StaffId==id&&s.RevokedAt==null).ToListAsync();foreach(var s in sessions)s.RevokedAt=DateTime.UtcNow;audit.Record("Staff","Staff deactivated",staff.FullName,staff:actor);await db.SaveChangesAsync();}
}
