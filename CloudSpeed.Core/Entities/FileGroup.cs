using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CloudSpeed.Services;

namespace CloudSpeed.Entities
{
    [Table("dt_file_group")]
    public class FileGroup
    {
        [Column("id")]
        [StringLength(StringLengthConstants.StringLengthObjectId)]
        public string Id { get; set; } = SequentialGuid.NewGuidString();

        [Column("name")]
        [StringLength(StringLengthConstants.StringLengthName)]
        public string name { get; set; }

        [Column("group_id")]
        [StringLength(StringLengthConstants.StringLengthObjectId)]
        public string GroupId { get; set; }

        [Column("created")]
        public DateTime Created { get; set; } = DateTime.Now;

        [Column("updated")]
        public DateTime Updated { get; set; } = DateTime.Now;
    }
}
