using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CloudSpeed.Services;

namespace CloudSpeed.Entities.DTO
{
    public class FileDealParamMap
    {
        public FileDealStatus? Status { get; set; }
    }
}
