namespace LittleSteps.Api.Domain;
public enum AttendanceStatus {Present,Absent}
public sealed record AttendanceMark(int StudentId,AttendanceStatus Status);
public sealed class AttendanceSheet
{
 public int ClassId{get;}public DateOnly Date{get;}public bool IsLocked{get;private set;}public string? SavedBy{get;private set;}public DateTime? SavedAt{get;private set;}
 private readonly HashSet<int> studentIds;private readonly Dictionary<int,AttendanceStatus> marks=[];
 public IReadOnlyList<AttendanceMark> Marks=>marks.Select(m=>new AttendanceMark(m.Key,m.Value)).ToList().AsReadOnly();
 public AttendanceSheet(int classId,DateOnly date,DateOnly today,IEnumerable<int> ids,bool saved){if(date>today)throw new DomainException("Future attendance dates are not allowed.");if(date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)throw new DomainException("No school on this day.");ClassId=classId;Date=date;studentIds=ids.ToHashSet();IsLocked=saved;}
 public void StartCorrection(){if(!IsLocked)throw new DomainException("This day has not been saved.",409);IsLocked=false;}
 public void Mark(int studentId,AttendanceStatus status){if(IsLocked)throw new DomainException("Start a correction before editing this day.",409);if(!studentIds.Contains(studentId))throw new DomainException("The student is not in your class for this day.",403);if(!Enum.IsDefined(status))throw new DomainException("Choose Present or Absent.");marks[studentId]=status;}
 public void MarkAllPresent(){foreach(var id in studentIds.Where(id=>!marks.ContainsKey(id)))Mark(id,AttendanceStatus.Present);}
 public void Save(Teacher by,DateTime now){if(!by.CanMarkAttendanceFor(ClassId))throw new DomainException("You may mark only your own class.",403);if(marks.Count!=studentIds.Count)throw new DomainException("Mark every student before saving.");if(studentIds.Count==0)throw new DomainException("There are no students to mark.");SavedBy=by.FullName;SavedAt=now;IsLocked=true;}
}
