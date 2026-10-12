namespace LittleSteps.Api.Domain;
public sealed class Guardian(int id,string name,string phone,byte[]? photo,string emiratesId):Person(name,phone,photo) {public int Id{get;}=id;public EmiratesId EmiratesId{get;}=new(emiratesId);}
public sealed record StudentGuardian(int GuardianId,string EmiratesId,string Relationship);
public sealed record ClassTransfer(int StudentId,int? FromClassId,int ToClassId,DateOnly EffectiveDate,string Reason);
public sealed class Student
{
 public int Id{get;} public string StudentCode{get;} public string FirstName{get;private set;} public string LastName{get;private set;} public DateOnly DateOfBirth{get;private set;} public char Gender{get;private set;} public int ClassId{get;private set;} public byte[]? Photo{get;private set;} public string FullName=>FirstName+" "+LastName;public bool PhotoMissing=>Photo is null;
 private readonly IReadOnlyList<StudentGuardian> guardians;
 public Student(int id,string code,string first,string last,DateOnly birth,char gender,int classId,byte[]? photo,IReadOnlyList<StudentGuardian>? guardians=null) {Id=id;StudentCode=code;FirstName=first;LastName=last;DateOfBirth=birth;Gender=gender;ClassId=classId;Photo=photo;this.guardians=guardians??[];}
 public void Edit(string first,string last,DateOnly birth,char gender,DateOnly today){FirstName=Rules.Text(first,"First name",60);LastName=Rules.Text(last,"Last name",60);if(birth>=today || birth<today.AddYears(-18))throw new DomainException("Date of birth must be in the past and within 18 years.");if(gender is not ('M' or 'F'))throw new DomainException("Choose Male or Female.");DateOfBirth=birth;Gender=gender;}
 public void SetPhoto(byte[]? photo){Rules.Photo(photo);Photo=photo;}
 public bool IsLinkedTo(EmiratesId id)=>guardians.Any(g=>g.EmiratesId==id.Value);
 // The service commits the returned history and the changed class in one transaction.
 public ClassTransfer MoveToClass(SchoolClass target,DateOnly date,string reason){if(target.Id==ClassId)throw new DomainException("The student is already in this class.",409);if(!target.HasSpace())throw new DomainException("The target class is full.",409);reason=Rules.Text(reason,"Reason",200);var transfer=new ClassTransfer(Id,ClassId,target.Id,date,reason);ClassId=target.Id;return transfer;}
}
