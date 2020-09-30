using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CloudSpeed.Managers;

namespace filshareapp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RewardController : ApiControllerBase
    {
        private readonly CloudSpeedManager _CloudSpeedManager;

        public RewardController(CloudSpeedManager CloudSpeedManager)
        {
            _CloudSpeedManager = CloudSpeedManager;
        }

        [HttpGet("alipay/{id}")]
        public ActionResult Alipay(string id)
        {
            var data = _CloudSpeedManager.GetRewardPath(id);
            return PhysicalFile(data, "image/png");
        }

        [HttpGet("wxpay/{id}")]
        public ActionResult Wxpay(string id)
        {
            
            var data = _CloudSpeedManager.GetRewardPath(id);
            return PhysicalFile(data, "image/png");
        }
    }
}
