using System.ComponentModel.DataAnnotations;

namespace CloudSpeed.Web.Requests
{
    public class MemberLoginRequest
    {
        [Required]
        public string Address { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
