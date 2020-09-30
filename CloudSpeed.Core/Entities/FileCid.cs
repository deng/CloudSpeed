using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CloudSpeed.Services;

namespace CloudSpeed.Entities
{
    [Table("dt_file_cid")]
    public class FileCid
    {
        [Column("id")]
        [StringLength(StringLengthConstants.StringLengthObjectId)]
        public string Id { get; set; }

        [Column("name")]
        [StringLength(StringLengthConstants.StringLengthCid)]
        public string Cid { get; set; }

        [Column("status")]
        public FileCidStatus Status { get; set; } = FileCidStatus.None;

        [Column("created")]
        public DateTime Created { get; set; } = DateTime.Now;
        
        [Column("updated")]
        public DateTime Updated { get; set; } = DateTime.Now;
    }
}
