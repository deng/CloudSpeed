using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CloudSpeed.Services;

namespace CloudSpeed.Entities
{
    [Table("dt_file_import")]
    public class FileImport
    {
        [Column("id")]
        [StringLength(StringLengthConstants.StringLengthObjectId)]
        public string Id { get; set; } = SequentialGuid.NewGuidString();

        [Column("path")]
        [StringLength(StringLengthConstants.StringLengthPath)]
        public string Path { get; set; }

        [Column("status")]
        public FileImportStatus Status { get; set; } = FileImportStatus.None;

        [Column("total")]
        public long Total { get; set; }

        [Column("success")]
        public long Success { get; set; }

        [Column("failed")]
        public long Failed { get; set; }

        [Column("error")]
        [StringLength(StringLengthConstants.StringLengthError)]
        public string Error { get; set; }

        [Column("created")]
        public DateTime Created { get; set; } = DateTime.Now;

        [Column("updated")]
        public DateTime Updated { get; set; } = DateTime.Now;
    }
}
