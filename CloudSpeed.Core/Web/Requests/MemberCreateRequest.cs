using System.ComponentModel.DataAnnotations;

namespace CloudSpeed.Web.Requests
{
    public class MemberCreateRequest
    {
        [Required]
        public string Address { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
