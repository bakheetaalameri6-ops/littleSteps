using LittleSteps.Api.Security;
using LittleSteps.Api.Services;
using LittleSteps.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace LittleSteps.Api.Controllers;
[ApiController,Route("api/auth")]
public sealed class AuthController(IAuthService auth):SchoolController
{
    [HttpPost("login")]public Task<object> Login(LoginRequest request)=>auth.Login(request.Username,request.Password);
    [HttpGet("me"),RequireModule(AllowPasswordChange=true)]public object Me()=>auth.Profile(Actor);
    [HttpPost("logout"),RequireModule(AllowPasswordChange=true)]public async Task<IActionResult> Logout(){await auth.Logout(Session);return NoContent();}
    [HttpPost("change-password"),RequireModule(AllowPasswordChange=true)]public async Task<IActionResult> ChangePassword(PasswordRequest request){await auth.ChangePassword(Session,request.Current,request.New);return NoContent();}
}
