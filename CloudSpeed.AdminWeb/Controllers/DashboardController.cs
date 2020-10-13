using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CloudSpeed.Managers;
using CloudSpeed.AdminWeb.Requests;
using CloudSpeed.AdminWeb.Responses;
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

        [HttpPost("Info")]
        public IActionResult Info()
        {
            var info = new DashboardInfo();
            info.Jobs = _JobsManager.GetDashboardInfo();
            info.Deals = _DealsManager.GetDashboardInfo();
            return Ok(info);
        }

        [HttpPost("Jobs")]
        public IActionResult Jobs()
        {
            var data = _JobsManager.GetDashboardInfo();
            return Ok(data);
        }

        [HttpPost("Deals")]
        public IActionResult Deals()
        {
            var data = _DealsManager.GetDashboardInfo();
            return Ok(data);
        }
    }
}
