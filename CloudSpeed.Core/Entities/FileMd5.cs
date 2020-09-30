using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CloudSpeed.Services;

namespace CloudSpeed.Entities
{
    [Table("dt_file_md5")]
    public class FileMd5
    {
        [Column("id")]
        [StringLength(StringLengthConstants.StringLengthObjectId)]
        public string Id { get; set; }

        [Column("data_key")]
        [StringLength(StringLengthConstants.StringLengthKey)]
        public string DataKey { get; set; }

        [Column("created")]
        public DateTime Created { get; set; } = DateTime.Now;
    }
}
