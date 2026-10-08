using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QMSApplication.Models
{
    public class ImportException
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long ImportBatchId { get; set; }

        [MaxLength(20)]
        public string? RecordID { get; set; }

        [MaxLength(50)]
        public string? SourceSNo { get; set; }

        [MaxLength(200)]
        public string? FieldName { get; set; }

        [MaxLength(1000)]
        public string? SourceValue { get; set; }

        [Required]
        [MaxLength(1000)]
        public string ExceptionReason { get; set; } = string.Empty;

        public bool IsReviewed { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? ReviewedAtUtc { get; set; }

        [MaxLength(100)]
        public string? ReviewedBy { get; set; }
    }
}