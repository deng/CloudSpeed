using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CloudSpeed.Services;

namespace CloudSpeed.Entities
{
    public enum FileCidStatus : byte
    {
        None = 0,

        Success = 1,

        Failed = 2
    }
}
