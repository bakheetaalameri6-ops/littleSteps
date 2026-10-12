using LittleSteps.Api.Domain;
using LittleSteps.Api.Security;
using LittleSteps.Api.Services;
using LittleSteps.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace LittleSteps.Api.Controllers;
[ApiController,Route("api/students")]
public sealed class StudentsController(IStudentService students):SchoolController
{
 [HttpGet,RequireModule(Module.StudentsAndGuardians,Module.GateControl)]public Task<object> Search(string? q,int? classId,bool? photoMissing)=>students.Search(q,classId,photoMissing,Actor);
 [HttpGet("{id:int}"),RequireModule(Module.StudentsAndGuardians,Module.GateControl)]public Task<object> Detail(int id)=>students.Detail(id,Actor);
 [HttpPost,RequireModule(Module.StudentsAndGuardians)]public Task<object> Create(StudentRequest r)=>students.Save(null,r,Actor);
 [HttpPut("{id:int}"),RequireModule(Module.StudentsAndGuardians)]public Task<object> Edit(int id,StudentRequest r)=>students.Save(id,r,Actor);
 [HttpPost("{id:int}/guardians"),RequireModule(Module.StudentsAndGuardians)]public async Task<IActionResult> Link(int id,GuardianLink r){await students.Link(id,r,Actor);return NoContent();}
 [HttpDelete("{id:int}/guardians/{guardianId:int}"),RequireModule(Module.StudentsAndGuardians)]public async Task<IActionResult> Unlink(int id,int guardianId){await students.Unlink(id,guardianId,Actor);return NoContent();}
 [HttpPost("{id:int}/transfer"),RequireModule(Module.StudentsAndGuardians)]public async Task<IActionResult> Move(int id,TransferRequest r){await students.Move(id,r,Actor);return NoContent();}
 [HttpGet("{id:int}/transfers"),RequireModule(Module.StudentsAndGuardians)]public Task<object> History(int id)=>students.History(id);
 [HttpGet("{id:int}/photo"),RequireModule(Module.StudentsAndGuardians,Module.Attendance,Module.GateControl)]public async Task<IActionResult> Photo(int id){var bytes=await students.Photo(id,Actor);Response.Headers.CacheControl="no-store";return bytes is null?NotFound():File(bytes,Rules.PhotoType(bytes));}
 [HttpPut("{id:int}/photo"),RequireModule(Module.StudentsAndGuardians),RequestSizeLimit(2200000)]public async Task<IActionResult> SetPhoto(int id,IFormFile photo){await using var stream=new MemoryStream();await photo.CopyToAsync(stream);await students.SetPhoto(id,stream.ToArray(),Actor);return NoContent();}
 [HttpDelete("{id:int}/photo"),RequireModule(Module.StudentsAndGuardians)]public async Task<IActionResult> RemovePhoto(int id){await students.SetPhoto(id,null,Actor);return NoContent();}
}
[ApiController,Route("api/guardians")]
public sealed class GuardiansController(IGuardianService guardians):SchoolController
{
 [HttpGet,RequireModule(Module.StudentsAndGuardians)]public Task<object> Search(string? emiratesId)=>guardians.Search(emiratesId);
 [HttpGet("{id:int}"),RequireModule(Module.StudentsAndGuardians)]public Task<object> Detail(int id)=>guardians.Detail(id);
 [HttpPost,RequireModule(Module.StudentsAndGuardians)]public Task<object> Create(GuardianRequest r)=>guardians.Save(null,r,Actor);
 [HttpPut("{id:int}"),RequireModule(Module.StudentsAndGuardians)]public Task<object> Edit(int id,GuardianRequest r)=>guardians.Save(id,r,Actor);
 [HttpGet("{id:int}/photo"),RequireModule(Module.StudentsAndGuardians,Module.GateControl)]public async Task<IActionResult> Photo(int id){var bytes=await guardians.Photo(id);Response.Headers.CacheControl="no-store";return bytes is null?NotFound():File(bytes,Rules.PhotoType(bytes));}
 [HttpPut("{id:int}/photo"),RequireModule(Module.StudentsAndGuardians),RequestSizeLimit(2200000)]public async Task<IActionResult> SetPhoto(int id,IFormFile photo){await using var stream=new MemoryStream();await photo.CopyToAsync(stream);await guardians.SetPhoto(id,stream.ToArray(),Actor);return NoContent();}
 [HttpDelete("{id:int}/photo"),RequireModule(Module.StudentsAndGuardians)]public async Task<IActionResult> RemovePhoto(int id){await guardians.SetPhoto(id,null,Actor);return NoContent();}
}
[ApiController,Route("api/dashboard"),RequireModule(Module.StaffAndClasses)]
public sealed class DashboardController(IDashboardService dashboard):SchoolController {[HttpGet("summary")]public Task<object> Summary()=>dashboard.Summary();}
