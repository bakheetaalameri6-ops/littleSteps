namespace LittleSteps.Api.Domain;
public abstract class Staff(int id,string name,string username,string? phone,byte[]? photo,bool active,bool mustChange):Person(name,phone,photo)
{
    public int Id { get; }=id;
    public string Username { get; private set; }=username;
    public bool IsActive { get; private set; }=active;
    public bool MustChangePassword { get; private set; }=mustChange;
    public abstract bool CanAccess(Module module);
    public virtual bool CanMarkAttendanceFor(int classId)=>false;
    public void Deactivate()=>IsActive=false;
}
public sealed class Admin(int id,string name,string username,string? phone,byte[]? photo,bool active,bool mustChange):Staff(id,name,username,phone,photo,active,mustChange) { public override bool CanAccess(Module m)=>m is Module.StaffAndClasses or Module.StudentsAndGuardians; }
public sealed class Teacher(int id,string name,string username,string? phone,byte[]? photo,bool active,bool mustChange,int? classId):Staff(id,name,username,phone,photo,active,mustChange) { public int? ClassId { get; }=classId;public override bool CanAccess(Module m)=>m==Module.Attendance;public override bool CanMarkAttendanceFor(int c)=>ClassId==c; }
public sealed class SecurityGuard(int id,string name,string username,string? phone,byte[]? photo,bool active,bool mustChange):Staff(id,name,username,phone,photo,active,mustChange) { public override bool CanAccess(Module m)=>m==Module.GateControl; }
public sealed class Manager(int id,string name,string username,string? phone,byte[]? photo,bool active,bool mustChange):Staff(id,name,username,phone,photo,active,mustChange) { public override bool CanAccess(Module m)=>m==Module.Audit; }
public sealed class SchoolClass
{
    public int Id {get;} public string Name {get;} public int Capacity {get;private set;} public int StudentCount {get;} public int? TeacherId {get;private set;}
    public SchoolClass(int id,string name,int capacity,int count,int? teacher=null) {Id=id;Name=Rules.Text(name,"Class name",20);StudentCount=count;SetCapacity(capacity);TeacherId=teacher;}
    public bool HasSpace()=>StudentCount<Capacity;
    public void SetCapacity(int capacity) {if(capacity is <1 or >40 || capacity<StudentCount)throw new DomainException("Capacity must be 1–40 and cannot be below current enrolment.");Capacity=capacity;}
    public void AssignTeacher(Teacher teacher){if(!teacher.IsActive)throw new DomainException("Teacher is inactive.");if(teacher.ClassId is not null && teacher.ClassId!=Id)throw new DomainException("Teacher already has a class.",409);TeacherId=teacher.Id;}
}
public sealed class Session(string token,DateTime expiresAt,DateTime? revokedAt) {public string Token{get;}=token;public DateTime ExpiresAt{get;}=expiresAt;public bool IsValid(DateTime now)=>revokedAt is null && now<ExpiresAt;}
