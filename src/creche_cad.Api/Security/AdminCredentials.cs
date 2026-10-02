using Microsoft.AspNetCore.Identity;
namespace creche_cad.Api.Security;
public sealed class AdminCredentials {
    private readonly PasswordHasher<string> hasher = new();
    private readonly string passwordHash;
    public string Username { get; }
    public AdminCredentials(IConfiguration config) {
        Username = config["Admin:Username"] ?? throw new InvalidOperationException("Configure Admin__Username.");
        var password = config["Admin:Password"] ?? throw new InvalidOperationException("Configure Admin__Password.");
        if (password.Length < 12) throw new InvalidOperationException("Admin password must have at least 12 characters.");
        passwordHash = hasher.HashPassword(Username, password);
    }
    public bool Verify(string username, string password) {
        var valid = hasher.VerifyHashedPassword(Username, passwordHash, password) != PasswordVerificationResult.Failed;
        return string.Equals(username, Username, StringComparison.Ordinal) && valid;
    }
}
