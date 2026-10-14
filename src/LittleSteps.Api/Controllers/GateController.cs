using LittleSteps.Api.Domain;
using LittleSteps.Api.Security;
using LittleSteps.Api.Services;
using LittleSteps.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace LittleSteps.Api.Controllers;
[ApiController, Route("api/gate"), RequireModule(Module.GateControl)]
public sealed class GateController(IGateService gate) : SchoolController
{
    [HttpGet("visits")] public Task<object> Visits(DateOnly? date) => gate.Visits(date);
    [HttpPost("visits")] public Task<object> Register(VisitRequest r) => gate.RegisterVisit(r, Actor);
    [HttpPut("visits/{id:int}/checkout")] public async Task<IActionResult> CheckOut(int id) { await gate.CheckOut(id, Actor); return NoContent(); }
    [HttpPost("early-exits/check")] public Task<object> Check(ExitCheckRequest r) => gate.Check(r, Actor);
    [HttpPost("early-exits")] public Task<object> Confirm(ExitRequest r) => gate.Confirm(r, Actor);
    [HttpGet("early-exits")] public Task<object> Exits(DateOnly? date) => gate.Exits(date);
}
