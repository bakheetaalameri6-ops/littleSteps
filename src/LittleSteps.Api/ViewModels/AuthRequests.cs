using System.ComponentModel.DataAnnotations;
namespace LittleSteps.Api.ViewModels;
public sealed record LoginRequest([Required,StringLength(50)]string Username,[Required,StringLength(128)]string Password);
public sealed record PasswordRequest([Required]string Current,[Required,StringLength(128,MinimumLength=8)]string New);
