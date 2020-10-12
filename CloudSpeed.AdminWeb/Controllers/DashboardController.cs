using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CloudSpeed.Managers;
using CloudSpeed.AdminWeb.Requests;
using CloudSpeed.AdminWeb.Managers;

namespace CloudSpeed.AdminWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ApiControllerBase
    {
        private readonly JobsManager _JobsManager;

        private readonly DealsManager _DealsManager;

        public DashboardController(JobsManager jobsManager, DealsManager dealsManager)
        {
            _JobsManager = jobsManager;
            _DealsManager = dealsManager;
        }

        [HttpPost("Jobs")]
        public async Task<IActionResult> Jobs([FromBody] DashboardJobsRequest request)
        {
            var data = await _JobsManager.GetDashboardInfo(request);
            return Result(data);
        }

        [HttpPost("Deals")]
        public async Task<IActionResult> Deals([FromBody] DashboardDealsRequest request)
        {
            var data = await _DealsManager.GetDashboardInfo(request);
            return Result(data);
        }
    }
}
