using LittleSteps.Api.Data;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("LittleSteps")));
// Open CORS: for local validation only. Restrict before any real deployment.
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
var app = builder.Build();
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/api/health", () => Results.Ok(new { status = "ready" }));
app.MapGet("/api/auth/me", () => Results.Unauthorized());
app.MapControllers();
app.Run();
