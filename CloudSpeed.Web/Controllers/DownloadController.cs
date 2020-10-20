using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CloudSpeed.Managers;
using CloudSpeed.Settings;

namespace filshareapp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DownloadController : ApiControllerBase
    {
        private readonly ILogger<DownloadController> _logger;
        private readonly UploadSetting _uploadSetting;
        private readonly CloudSpeedManager _CloudSpeedManager;

        public DownloadController(ILogger<DownloadController> logger, UploadSetting uploadSetting, CloudSpeedManager CloudSpeedManager)
        {
            _logger = logger;
            _uploadSetting = uploadSetting;
            _CloudSpeedManager = CloudSpeedManager;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(string id)
        {
            var data = await _CloudSpeedManager.GetFileByCid(id);
            return Result(data);
        }
    }
}
