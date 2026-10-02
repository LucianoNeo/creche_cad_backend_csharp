namespace creche_cad.Domain.Entities;
public class SchoolUser {
 public Guid Id { get; set; } = Guid.NewGuid();
 public string Username { get; set; } = "";
 public string PasswordHash { get; set; } = "";
 public string Role { get; set; } = "Secretary";
 public bool Active { get; set; } = true;
 public string SecurityStamp { get; set; } = Guid.NewGuid().ToString("N");
}
public class AuditEntry {
 public long Id { get; set; }
 public DateTime At { get; set; } = DateTime.UtcNow;
 public string Actor { get; set; } = "system";
 public string Action { get; set; } = "";
 public string Resource { get; set; } = "";
 public string ResourceId { get; set; } = "";
}
public class AccessRecovery {
 public Guid Id { get; set; } = Guid.NewGuid();
 public string Username { get; set; } = "";
 public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
 public bool Resolved { get; set; }
}
