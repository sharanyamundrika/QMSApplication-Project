using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QMSApplication.Models
{
    public class ImportBatch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [MaxLength(300)]
        public string SourceFileName { get; set; } = string.Empty;

        public DateTime ImportedAtUtc { get; set; }

        [MaxLength(100)]
        public string? ImportedBy { get; set; }

        public int TotalRecords { get; set; }

        public int SuccessfulRecords { get; set; }

        public int ExceptionCount { get; set; }

        [MaxLength(30)]
        public string Status { get; set; } = "Completed";
    }
}