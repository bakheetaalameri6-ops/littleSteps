using System.ComponentModel.DataAnnotations;
namespace LittleSteps.Api.ViewModels;
public sealed record VisitRequest([Required, StringLength(120)] string FullName, [Required] string EmiratesId, [Required] string Reason, [Required, StringLength(120)] string PersonVisited);
public sealed record ExitCheckRequest(int StudentId, [Required] string EmiratesId);
public sealed record ExitRequest(int StudentId, [Required] string EmiratesId, [Required] string Reason);
