using Microsoft.AspNetCore.Identity;
using System;

namespace CloudSpeed.Identity
{
    public class Member : IdentityUser
    {
        public DateTime Created { get; set; } = DateTime.Now;
    }
}
