using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Domain.Entities;

public class Log
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Action { get; set; } = default!;
    public string Description { get; set; } = default!;
    public long ModifiedBy { get; set; }
    public DateTime Timestamp { get; set; }
    public User? User { get; set; }
    public User? UserThatModified { get; set; }
}
