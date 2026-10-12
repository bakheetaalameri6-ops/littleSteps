using LittleSteps.Api.Data;
using LittleSteps.Api.Domain;
using LittleSteps.Api.Mapping;
using LittleSteps.Api.ViewModels;
using Microsoft.EntityFrameworkCore;
using E=LittleSteps.Api.Data.Entities;
namespace LittleSteps.Api.Services;
public interface IClassService {Task<object> Rooms(bool free);Task<object> CreateRoom(RoomRequest request,E.Staff actor);Task<object> List();Task<object> Save(int? id,ClassRequest request,E.Staff actor);Task Assign(int id,int? teacherId,E.Staff actor);}
public sealed class ClassService(AppDbContext db,IAuditService audit):IClassService
{
    public async Task<object> Rooms(bool free)=>await db.Rooms.Where(r=>!free||r.Class==null).OrderBy(r=>r.Name).Select(r=>new{id=r.RoomId,r.Name}).ToListAsync();
    public async Task<object> CreateRoom(RoomRequest r,E.Staff actor){var room=new E.Room{Name=Rules.Text(r.Name,"Room name",40)};db.Rooms.Add(room);audit.Record("Classes","Room created",room.Name,staff:actor);await db.SaveChangesAsync();return new{id=room.RoomId,room.Name};}
    public async Task<object> List()=>await db.Classes.OrderBy(c=>c.Name).Select(c=>new{id=c.ClassId,c.Name,c.RoomId,roomName=c.Room.Name,c.TeacherId,teacherName=c.Teacher!=null?c.Teacher.FullName:null,c.Capacity,studentCount=c.Students.Count(s=>s.IsActive)}).ToListAsync();
    private async Task<SchoolClass> Domain(E.Class c)=>new(c.ClassId,c.Name,c.Capacity,await db.Students.CountAsync(s=>s.ClassId==c.ClassId&&s.IsActive),c.TeacherId);
    private async Task SetTeacher(E.Class c,SchoolClass domain,int? id){if(id is null){c.TeacherId=null;return;}var t=await db.Staff.Include(s=>s.Class).SingleOrDefaultAsync(s=>s.StaffId==id)??throw new DomainException("Teacher not found.",404);if(StaffFactory.FromEntity(t) is not Teacher teacher)throw new DomainException("Choose a teacher.");domain.AssignTeacher(teacher);c.TeacherId=domain.TeacherId;}
    public async Task<object> Save(int? id,ClassRequest r,E.Staff actor){var c=id.HasValue?await db.Classes.SingleOrDefaultAsync(c=>c.ClassId==id)??throw new DomainException("Class not found.",404):new E.Class();c.Name=Rules.Text(r.Name,"Class name",20);c.RoomId=r.RoomId;var domain=new SchoolClass(c.ClassId,c.Name,r.Capacity,id.HasValue?await db.Students.CountAsync(s=>s.ClassId==id&&s.IsActive):0);c.Capacity=domain.Capacity;await SetTeacher(c,domain,r.TeacherId);if(!id.HasValue)db.Classes.Add(c);audit.Record("Classes",id.HasValue?"Class edited":"Class created",c.Name,staff:actor);await db.SaveChangesAsync();return new{id=c.ClassId};}
    public async Task Assign(int id,int? teacherId,E.Staff actor){var c=await db.Classes.SingleOrDefaultAsync(c=>c.ClassId==id)??throw new DomainException("Class not found.",404);await SetTeacher(c,await Domain(c),teacherId);audit.Record("Classes","Teacher assigned",c.Name,staff:actor);await db.SaveChangesAsync();}
}
