namespace LittleSteps.Api.ViewModels;
public sealed record MarkRequest(int StudentId,string Status);
public sealed record AttendanceRequest(DateOnly Date,MarkRequest[] Marks,bool Correction,int? ClassId=null);
