namespace RiverLine.Api.Extensions;

public static class UserQueryExtensions
{
    public static Task<bool> IsEmailConfirmedAsync(this ApplicationDbContext db, Guid userId) =>
        db.Users.Where(u => u.Id == userId).Select(u => u.EmailConfirmed).FirstOrDefaultAsync();
    
    public static async Task<string?> GetDealBlockerAsync(this ApplicationDbContext db, Guid userId)
    {
        var state = await db.Users
            .Where(u => u.Id == userId)
            .Select(u => new { u.EmailConfirmed, HasPhone = u.PhoneNumber != null })
            .FirstOrDefaultAsync();

        if (state is null) return "user.not_found";
        if (!state.EmailConfirmed) return "email.unconfirmed";
        if (!state.HasPhone) return "phone.required";
        return null;
    }
    
}