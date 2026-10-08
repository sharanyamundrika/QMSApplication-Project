using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QMSApplication.Models
{
    public class MDAuditLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public DateTime TimestampUtc { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string RecordCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Action { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string ChangedField { get; set; } = string.Empty;

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }
    }
}