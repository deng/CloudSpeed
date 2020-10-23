using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CloudSpeed.Services;

namespace CloudSpeed.Entities
{
    [Table("dt_file_name")]
    public class FileName
    {
        [Column("id")]
        [StringLength(StringLengthConstants.StringLengthObjectId)]
        public string Id { get; set; }

        [Column("name")]
        [StringLength(StringLengthConstants.StringLengthName)]
        public string Name { get; set; }

        [Column("format")]
        [StringLength(StringLengthConstants.StringLengthFormat)]
        public string Format { get; set; }

        [Column("size")]
        public long Size { get; set; }

        [Column("group_id")]
        [StringLength(StringLengthConstants.StringLengthObjectId)]
        public string GroupId { get; set; }

        [Column("created")]
        public DateTime Created { get; set; } = DateTime.Now;
    }
}
