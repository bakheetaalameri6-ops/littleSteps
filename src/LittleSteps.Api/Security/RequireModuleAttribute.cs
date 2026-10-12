using LittleSteps.Api.Domain;
using LittleSteps.Api.Mapping;
using LittleSteps.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace LittleSteps.Api.Security;
[AttributeUsage(AttributeTargets.Class|AttributeTargets.Method)]
public sealed class RequireModuleAttribute:Attribute,IAsyncAuthorizationFilter
{
    private readonly Module[] modules;
    public bool AllowPasswordChange {get;set;}
    public RequireModuleAttribute(params Module[] modules)=>this.modules=modules;
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context) {
        var header=context.HttpContext.Request.Headers.Authorization.ToString();
        var session=await context.HttpContext.RequestServices.GetRequiredService<IAuthService>().Resolve(header.StartsWith("Bearer ",StringComparison.Ordinal)?header[7..]:"");
        if(session is null){context.Result=new UnauthorizedObjectResult(new{message="Please sign in."});return;}
        if(session.Staff.MustChangePassword && !AllowPasswordChange){context.Result=new ObjectResult(new{message="Change your temporary password before continuing."}){StatusCode=403};return;}
        if(modules.Length>0 && !modules.Any(StaffFactory.FromEntity(session.Staff).CanAccess)){context.Result=new ObjectResult(new{message="Your role cannot access this module."}){StatusCode=403};return;}
        context.HttpContext.Items["Session"]=session;
    }
}
public abstract class SchoolController:ControllerBase
{
    protected Data.Entities.Session Session=>(Data.Entities.Session)HttpContext.Items["Session"]!;
    protected Data.Entities.Staff Actor=>Session.Staff;
}
