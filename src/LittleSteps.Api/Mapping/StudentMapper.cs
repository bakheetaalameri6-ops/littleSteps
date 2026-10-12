using LittleSteps.Api.Domain;
namespace LittleSteps.Api.Mapping;
public static class StudentMapper
{
 public static Student FromEntity(Data.Entities.Student e)=>new(e.StudentId,e.StudentCode??"",e.FirstName,e.LastName,e.DateOfBirth,e.Gender,e.ClassId,e.Photo,e.StudentGuardians.Select(g=>new StudentGuardian(g.GuardianId,g.Guardian.EmiratesId,g.Relationship)).ToList());
 public static void Apply(Student d,Data.Entities.Student e){e.FirstName=d.FirstName;e.LastName=d.LastName;e.DateOfBirth=d.DateOfBirth;e.Gender=d.Gender;e.ClassId=d.ClassId;e.Photo=d.Photo;}
 public static Guardian Guardian(Data.Entities.Guardian e)=>new(e.GuardianId,e.FullName,e.Phone,e.Photo,e.EmiratesId);
}
