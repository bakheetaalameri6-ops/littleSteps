using LittleSteps.Api.Domain;
using LittleSteps.Api.Security;
using LittleSteps.Api.Services;
using Microsoft.AspNetCore.Mvc;
namespace LittleSteps.Api.Controllers;
[ApiController, Route("api/audit"), RequireModule(Module.Audit)]
public sealed class AuditController(IAuditService audit, SchoolClock clock) : SchoolController
{
    [HttpGet] public Task<object> Search(DateOnly? from, DateOnly? to, int? staffId, string? area, int page = 1, int pageSize = 20) { if (from.HasValue && to.HasValue && from > to) throw new DomainException("From date must precede To date."); return audit.Search(from.HasValue ? clock.Day(from.Value).Start : null, to.HasValue ? clock.Day(to.Value).End : null, staffId, area, page, pageSize); }
    [HttpGet("users")] public Task<object> Users() => audit.Users();
}
