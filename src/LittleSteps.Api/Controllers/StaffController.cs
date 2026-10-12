using LittleSteps.Api.Domain;
using LittleSteps.Api.Security;
using LittleSteps.Api.Services;
using LittleSteps.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace LittleSteps.Api.Controllers;
[ApiController,Route("api/staff"),RequireModule(Module.StaffAndClasses)]
public sealed class StaffController(IStaffService staff):SchoolController
{
 [HttpGet]public Task<object> List(string? q,string? role)=>staff.List(q,role);
 [HttpPost]public Task<object> Create(StaffRequest request)=>staff.Save(null,request,Actor);
 [HttpPut("{id:int}")]public Task<object> Edit(int id,StaffRequest request)=>staff.Save(id,request,Actor);
 [HttpPut("{id:int}/deactivate")]public async Task<IActionResult> Deactivate(int id){await staff.Deactivate(id,Actor);return NoContent();}
}
[ApiController,Route("api/rooms"),RequireModule(Module.StaffAndClasses)]
public sealed class RoomsController(IClassService classes):SchoolController
{
 [HttpGet]public Task<object> List(bool free=false)=>classes.Rooms(free);
 [HttpPost]public Task<object> Create(RoomRequest request)=>classes.CreateRoom(request,Actor);
}
[ApiController,Route("api/classes"),RequireModule(Module.StaffAndClasses)]
public sealed class ClassesController(IClassService classes):SchoolController
{
 [HttpGet]public Task<object> List()=>classes.List();
 [HttpPost]public Task<object> Create(ClassRequest request)=>classes.Save(null,request,Actor);
 [HttpPut("{id:int}")]public Task<object> Edit(int id,ClassRequest request)=>classes.Save(id,request,Actor);
 [HttpPut("{id:int}/teacher")]public async Task<IActionResult> Assign(int id,TeacherRequest request){await classes.Assign(id,request.TeacherId,Actor);return NoContent();}
}
