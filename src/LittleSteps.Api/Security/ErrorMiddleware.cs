using LittleSteps.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;
namespace LittleSteps.Api.Security;
public sealed class ErrorMiddleware(RequestDelegate next,ILogger<ErrorMiddleware> logger)
{
    public async Task Invoke(HttpContext context){try{await next(context);}catch(DomainException e){await Error(context,e.Status,e.Message);}catch(DbUpdateException e) when(e.InnerException is PostgresException){var p=(PostgresException)e.InnerException;var conflict=p.SqlState is "23505" or "23514" or "40001";await Error(context,conflict?409:400,p.SqlState=="23505"?"This username, room, teacher or Emirates ID is already in use.":p.SqlState=="23514"?"The change violates a school rule (capacity, role or required values).":"The referenced record does not exist or changed. Please reload.");}catch(PostgresException e) when(e.SqlState=="40001"){await Error(context,409,"Another user changed this record. Reload and try again.");}catch(Exception e){logger.LogError(e,"Request failed");await Error(context,500,"An unexpected error occurred. Please try again.");}}
    private static async Task Error(HttpContext context,int status,string message){context.Response.StatusCode=status;await context.Response.WriteAsJsonAsync(new{message});}
}
