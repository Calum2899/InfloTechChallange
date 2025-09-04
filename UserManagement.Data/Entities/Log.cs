using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Models;

public class Log
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public long UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    [Required]
    public string Action { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
    public long ModifiedBy { get; set; }
    public User UserThatModified { get; set; } = null!;
}
