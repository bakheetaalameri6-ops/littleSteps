using System.Security.Cryptography;
using LittleSteps.Api.Data;
using LittleSteps.Api.Data.Entities;
using LittleSteps.Api.Domain;
using LittleSteps.Api.Mapping;
using LittleSteps.Api.Security;
using Microsoft.EntityFrameworkCore;
namespace LittleSteps.Api.Services;
public interface IAuthService {Task<object> Login(string username,string password);Task<Data.Entities.Session?> Resolve(string token);Task Logout(Data.Entities.Session session);Task ChangePassword(Data.Entities.Session session,string current,string next);object Profile(Data.Entities.Staff staff);}
public sealed class AuthService(AppDbContext db,IAuditService audit,IConfiguration config):IAuthService
{
    public object Profile(Data.Entities.Staff s) {var domain=StaffFactory.FromEntity(s);return new{id=s.StaffId,s.FullName,s.Username,s.Role,s.MustChangePassword,classId=s.Class?.ClassId,className=s.Class?.Name,allowedModules=Enum.GetValues<Module>().Where(domain.CanAccess).Select(m=>m.ToString())};}
    public async Task<object> Login(string username,string password) {
        username=Rules.Text(username,"Username",50);if(password.Length>128)throw new DomainException("Invalid credentials.",401);
        var staff=await db.Staff.Include(s=>s.Class).SingleOrDefaultAsync(s=>s.Username==username);
        // A dummy derivation makes unknown usernames follow the same expensive path.
        var valid=PasswordHasher.Verify(password,staff?.PasswordHash??new byte[32],staff?.PasswordSalt??new byte[16]);
        if(staff is null || !staff.IsActive || !valid) {audit.Record("Access","Sign in","Invalid credentials","Refused",username:username);await db.SaveChangesAsync();throw new DomainException("Invalid username or password.",401);}
        var session=new Data.Entities.Session{StaffId=staff.StaffId,Token=Convert.ToHexString(RandomNumberGenerator.GetBytes(32)),CreatedAt=DateTime.UtcNow,ExpiresAt=DateTime.UtcNow.AddMinutes(config.GetValue("Auth:SessionMinutes",480))};db.Sessions.Add(session);audit.Record("Access","Sign in",null,staff:staff);await db.SaveChangesAsync();return new{session.Token,session.ExpiresAt,staff=Profile(staff)};
    }
    public async Task<Data.Entities.Session?> Resolve(string token) {if(token.Length!=64)return null;var s=await db.Sessions.Include(s=>s.Staff).ThenInclude(s=>s.Class).SingleOrDefaultAsync(s=>s.Token==token);return s is not null && s.Staff.IsActive && new Domain.Session(s.Token,s.ExpiresAt,s.RevokedAt).IsValid(DateTime.UtcNow)?s:null;}
    public async Task Logout(Data.Entities.Session session){session.RevokedAt=DateTime.UtcNow;audit.Record("Access","Sign out",null,staff:session.Staff);await db.SaveChangesAsync();}
    public async Task ChangePassword(Data.Entities.Session session,string current,string next){if(!PasswordHasher.Verify(current,session.Staff.PasswordHash,session.Staff.PasswordSalt))throw new DomainException("Current password is incorrect.");if(current==next)throw new DomainException("Choose a different password.");var (hash,salt)=PasswordHasher.Create(next);session.Staff.PasswordHash=hash;session.Staff.PasswordSalt=salt;session.Staff.MustChangePassword=false;var others=await db.Sessions.Where(s=>s.StaffId==session.StaffId && s.SessionId!=session.SessionId && s.RevokedAt==null).ToListAsync();foreach(var s in others)s.RevokedAt=DateTime.UtcNow;audit.Record("Access","Password changed",null,staff:session.Staff);await db.SaveChangesAsync();}
}
