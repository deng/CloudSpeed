using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CloudSpeed.Managers;
using CloudSpeed.AdminWeb.Requests;
using CloudSpeed.AdminWeb.Managers;

namespace CloudSpeed.AdminWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DealsController : ApiControllerBase
    {
        private readonly DealsManager _DealsManager;

        public DealsController(DealsManager dealsManager)
        {
            _DealsManager = dealsManager;
        }

        [HttpPost("GetList")]
        public async Task<IActionResult> GetList([FromBody] DealsGetListRequest request)
        {
            var data = await _DealsManager.GetDeals(request);
            return Result(data);
        }
    }
}
