using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CloudSpeed.Services;

namespace CloudSpeed.Entities
{
    [Table("dt_upload_log")]
    public class UploadLog
    {
        [Column("id")]
        [StringLength(StringLengthConstants.StringLengthObjectId)]
        public string Id { get; set; } = SequentialGuid.NewGuidString();

        [Column("description")]
        public string Description { get; set; }

        [Column("data_key")]
        [StringLength(StringLengthConstants.StringLengthKey)]
        public string DataKey { get; set; }

        [Column("alipay_file_key")]
        [StringLength(StringLengthConstants.StringLengthKey)]
        public string AlipayFileKey { get; set; }

        [Column("wxpay_file_key")]
        [StringLength(StringLengthConstants.StringLengthKey)]
        public string WxpayFileKey { get; set; }

        [Column("hashed_password")]
        [StringLength(StringLengthConstants.StringLengthKey)]
        public string HashedPassword { get; set; }

        [Column("created")]
        public DateTime Created { get; set; } = DateTime.Now;
    }
}
