using System.ComponentModel.DataAnnotations;
namespace LittleSteps.Api.ViewModels;
public sealed record StaffRequest([Required,StringLength(120)]string FullName,[Required]string Role,[Required,StringLength(50)]string Username,[StringLength(20)]string? Phone,string? TemporaryPassword);
public sealed record RoomRequest([Required,StringLength(40)]string Name);
public sealed record ClassRequest([Required,StringLength(20)]string Name,int RoomId,[Range(1,40)]int Capacity,int? TeacherId);
public sealed record TeacherRequest(int? TeacherId);
