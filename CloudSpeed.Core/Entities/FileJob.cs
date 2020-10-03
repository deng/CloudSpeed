using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudSpeed.Entities
{
    [Table("dt_file_job")]
    public class FileJob
    {
        [Column("id")]
        [StringLength(StringLengthConstants.StringLengthObjectId)]
        public string Id { get; set; }

        [Column("name")]
        [StringLength(StringLengthConstants.StringLengthCid)]
        public string Cid { get; set; }

        [Column("job_id")]
        [StringLength(StringLengthConstants.StringLengthCid)]
        public string JobId { get; set; }

        [Column("status")]
        public FileJobStatus Status { get; set; } = FileJobStatus.None;

        [Column("created")]
        public DateTime Created { get; set; } = DateTime.Now;

        [Column("updated")]
        public DateTime Updated { get; set; } = DateTime.Now;
    }
}
