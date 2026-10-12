using System.Text.RegularExpressions;
namespace LittleSteps.Api.Domain;
public enum Module { StaffAndClasses, StudentsAndGuardians, Attendance, GateControl, Audit }
public sealed class DomainException(string message, int status = 400) : Exception(message) { public int Status { get; } = status; }
public static class Rules
{
    public static string Text(string? value, string field, int max=120) {
        if (string.IsNullOrWhiteSpace(value) || value.Trim().Length>max) throw new DomainException($"{field} is required and must be at most {max} characters.");
        return value.Trim();
    }
    public static void Password(string value) { if(value.Length<8 || value.Length>128) throw new DomainException("Password must have 8–128 characters."); }
    public static void Relationship(string value) { if(!new[]{"Father","Mother","Other"}.Contains(value)) throw new DomainException("Choose Father, Mother or Other."); }
    public static void Photo(byte[]? photo) {
        if(photo is null) return;
        // Inspect bytes as well as the file extension; the browser's MIME type is untrusted.
        bool png=photo.Length>=24 && photo.AsSpan(0,8).SequenceEqual(new byte[]{137,80,78,71,13,10,26,10});
        bool jpeg=photo.Length>=4 && photo[0]==255 && photo[1]==216 && photo[2]==255 && photo[^2]==255 && photo[^1]==217;
        if(photo.Length>2*1024*1024 || !(png||jpeg)) throw new DomainException("Photo must be a JPEG or PNG up to 2 MB.");
    }
    public static string PhotoType(byte[] bytes)=>bytes[0]==137?"image/png":"image/jpeg";
}
public sealed record EmiratesId
{
    public string Value { get; }
    public EmiratesId(string value) { if(!Regex.IsMatch(value??"",@"\A784-[0-9]{4}-[0-9]{7}-[0-9]\z"))throw new DomainException("Emirates ID must match 784-YYYY-NNNNNNN-N."); Value=value!; }
    public override string ToString()=>Value;
}
public abstract class Person
{
    public string FullName { get; private set; }
    public string? Phone { get; private set; }
    public byte[]? Photo { get; private set; }
    public bool HasPhoto=>Photo is not null;
    protected Person(string name,string? phone,byte[]? photo) { FullName=Rules.Text(name,"Full name"); Phone=phone; Photo=photo; }
}
public sealed class SchoolClock(IConfiguration config)
{
    public DateTime Now=>DateTime.UtcNow;
    public DateOnly Today=>DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(Now,TimeZoneInfo.FindSystemTimeZoneById(config["School:TimeZone"]??"Asia/Dubai")));
    public (DateTime Start,DateTime End) Day(DateOnly date) { var zone=TimeZoneInfo.FindSystemTimeZoneById(config["School:TimeZone"]??"Asia/Dubai");return(TimeZoneInfo.ConvertTimeToUtc(date.ToDateTime(TimeOnly.MinValue),zone),TimeZoneInfo.ConvertTimeToUtc(date.AddDays(1).ToDateTime(TimeOnly.MinValue),zone)); }
}
