using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CloudSpeed.Services;

namespace CloudSpeed.Entities
{
    [Table("dt_file_deal")]
    public class FileDeal
    {
        [Column("id")]
        [StringLength(StringLengthConstants.StringLengthObjectId)]
        public string Id { get; set; } = SequentialGuid.NewGuidString();

        [Column("name")]
        [StringLength(StringLengthConstants.StringLengthCid)]
        public string Cid { get; set; }

        [Column("deal_id")]
        [StringLength(StringLengthConstants.StringLengthCid)]
        public string DealId { get; set; }
        
        [Column("miner")]
        [StringLength(StringLengthConstants.StringLengthMiner)]
        public string Miner { get; set; }

        [Column("status")]
        public FileDealStatus Status { get; set; } = FileDealStatus.None;

        [Column("error")]
        [StringLength(StringLengthConstants.StringLengthError)]
        public string Error { get; set; }

        [Column("created")]
        public DateTime Created { get; set; } = DateTime.Now;

        [Column("updated")]
        public DateTime Updated { get; set; } = DateTime.Now;
    }
}
