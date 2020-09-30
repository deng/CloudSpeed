using System.ComponentModel.DataAnnotations;

namespace CloudSpeed.Web.Requests
{
    public class PanValidateRequest
    {
        [Required]
        public string Password { get; set; }
    }
}
