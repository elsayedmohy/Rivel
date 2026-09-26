namespace RiverLine.Api.Extensions;

public static class UserQueryExtensions
{
    public static Task<bool> IsEmailConfirmedAsync(this ApplicationDbContext db, Guid userId) =>
        db.Users.Where(u => u.Id == userId).Select(u => u.EmailConfirmed).FirstOrDefaultAsync();
}