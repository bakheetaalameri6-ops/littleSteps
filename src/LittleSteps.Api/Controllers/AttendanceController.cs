using LittleSteps.Api.Domain;
using LittleSteps.Api.Security;
using LittleSteps.Api.Services;
using LittleSteps.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace LittleSteps.Api.Controllers;
[ApiController,Route("api/attendance"),RequireModule(Module.Attendance)]
public sealed class AttendanceController(IAttendanceService attendance):SchoolController
{
 [HttpGet]public Task<object> Get(DateOnly? date,int? classId)=>attendance.Get(Actor,date,classId);
 [HttpPut]public async Task<IActionResult> Save(AttendanceRequest r){await attendance.Save(Actor,r);return NoContent();}
}
