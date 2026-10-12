using System.ComponentModel.DataAnnotations;
namespace LittleSteps.Api.ViewModels;
public sealed record GuardianLink([Required]string EmiratesId,[Required]string Relationship);
public sealed record StudentRequest([Required,StringLength(60)]string FirstName,[Required,StringLength(60)]string LastName,DateOnly DateOfBirth,char Gender,int ClassId,GuardianLink[] Guardians,byte[]? Photo=null);
public sealed record GuardianRequest([Required]string EmiratesId,[Required,StringLength(120)]string FullName,[Required,StringLength(20)]string Phone);
public sealed record TransferRequest(int ToClassId,DateOnly EffectiveDate,[Required,StringLength(200)]string Reason);
