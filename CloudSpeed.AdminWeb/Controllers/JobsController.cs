using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CloudSpeed.Managers;
using CloudSpeed.AdminWeb.Requests;
using CloudSpeed.AdminWeb.Managers;

namespace CloudSpeed.AdminWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ApiControllerBase
    {
        private readonly JobsManager _JobsManager;

        public JobsController(JobsManager jobsManager)
        {
            _JobsManager = jobsManager;
        }

        [HttpPost("GetList")]
        public async Task<IActionResult> GetList([FromBody] JobsGetListRequest request)
        {
            var data = await _JobsManager.GetJobs(request);
            return Result(data);
        }
    }
}
