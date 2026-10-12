using LittleSteps.Api.Data;
using Microsoft.EntityFrameworkCore;
using LittleSteps.Api.Services;
using LittleSteps.Api.Domain;
using LittleSteps.Api.Security;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o=>{
 o.AddSecurityDefinition("Bearer",new OpenApiSecurityScheme {Type=SecuritySchemeType.Http,Scheme="bearer",Description="Paste the session token returned by login."});
 o.AddSecurityRequirement(new OpenApiSecurityRequirement {{new OpenApiSecurityScheme{Reference=new OpenApiReference{Type=ReferenceType.SecurityScheme,Id="Bearer"}},Array.Empty<string>()}});
});
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<IAuditService,AuditService>();
builder.Services.AddSingleton<SchoolClock>();
builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("LittleSteps")));
// Open CORS: for local validation only. Restrict before any real deployment.
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
var app = builder.Build();
app.UseMiddleware<ErrorMiddleware>();
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/api/health", () => Results.Ok(new { status = "ready" }));
var webRoot=Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath,"../../web"));
if(Directory.Exists(webRoot)) {
 var files=new PhysicalFileProvider(webRoot);
 app.UseDefaultFiles(new DefaultFilesOptions {FileProvider=files,DefaultFileNames={"login.html"}});
 app.UseStaticFiles(new StaticFileOptions {FileProvider=files});
}
app.MapControllers();
app.Run();
