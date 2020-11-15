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

        [Column("piece_cid")]
        [StringLength(StringLengthConstants.StringLengthCid)]
        public string PieceCid { get; set; }

        [Column("piece_size")]
        public long PieceSize { get; set; }

        [Column("deal_size")]
        public long DealSize { get; set; }

        [Column("payload_size")]
        public long PayloadSize { get; set; }

        [Column("status")]
        public FileCidStatus Status { get; set; } = FileCidStatus.None;

        [Column("error")]
        [StringLength(StringLengthConstants.StringLengthError)]
        public string Error { get; set; }

        [Column("created")]
        public DateTime Created { get; set; } = DateTime.Now;

        [Column("updated")]
        public DateTime Updated { get; set; } = DateTime.Now;
    }
}
