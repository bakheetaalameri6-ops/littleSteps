using LittleSteps.Api.Domain;
namespace LittleSteps.Api.Mapping;
public static class StaffFactory
{
    // This is the only place that interprets the stored discriminator; callers use polymorphism.
    public static Staff FromEntity(Data.Entities.Staff e)=>e.Role switch {
        "Admin"=>new Admin(e.StaffId,e.FullName,e.Username,e.Phone,e.Photo,e.IsActive,e.MustChangePassword),
        "Teacher"=>new Teacher(e.StaffId,e.FullName,e.Username,e.Phone,e.Photo,e.IsActive,e.MustChangePassword,e.Class?.ClassId),
        "SecurityGuard"=>new SecurityGuard(e.StaffId,e.FullName,e.Username,e.Phone,e.Photo,e.IsActive,e.MustChangePassword),
        "Manager"=>new Manager(e.StaffId,e.FullName,e.Username,e.Phone,e.Photo,e.IsActive,e.MustChangePassword),
        _=>throw new DomainException("Unknown staff role.")};
}
