using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CloudSpeed.Managers;
using CloudSpeed.Web.Requests;
using Microsoft.AspNetCore.Authorization;

namespace filshareapp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize()]
    public class MyFilesController : ApiControllerBase
    {
        private readonly CloudSpeedManager _CloudSpeedManager;

        public MyFilesController(CloudSpeedManager CloudSpeedManager)
        {
            _CloudSpeedManager = CloudSpeedManager;
        }

        [HttpPost]
        public async Task<ActionResult> Post(MyFilesPostRequest requst)
        {
            var userId = await GetAuthrizedUserId();
            var data = await _CloudSpeedManager.GetMyFiles(userId, requst);
            return Result(data);
        }
    }
}
