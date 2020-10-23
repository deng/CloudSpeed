using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CloudSpeed.Managers;
using CloudSpeed.Web.Requests;
using CloudSpeed.Web.Responses;
using Microsoft.AspNetCore.Authorization;

namespace filshareapp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PanController : ApiControllerBase
    {
        private readonly CloudSpeedManager _CloudSpeedManager;

        public PanController(CloudSpeedManager CloudSpeedManager)
        {
            _CloudSpeedManager = CloudSpeedManager;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(string id)
        {
            var data = await _CloudSpeedManager.GetUploadLog(id);
            return Result(data);
        }

        [HttpPost]
        public async Task<ActionResult> Post(PanePostRequest requst)
        {
            var userId = await GetAuthrizedUserId();
            var data = await _CloudSpeedManager.CreateUploadLog(userId, requst);
            return Result(data);
        }

        [HttpPost("{id}/validate")]
        public async Task<ActionResult> Validate(string id, PanValidateRequest requst)
        {
            var data = await _CloudSpeedManager.ValidateUploadLog(id, requst?.Password);
            return Result(data);
        }

        [HttpGet("{id}/download")]
        public async Task<ActionResult> Download(string id, [FromQuery] string password)
        {
            var validated = await _CloudSpeedManager.ValidateUploadLog(id, password);
            if (!validated.Success || !validated.Data)
                return NotFound();

            var info = await _CloudSpeedManager.GetFilDataInfo(id);
            if (!info.Success)
                return NotFound();

            if (!System.IO.File.Exists(info.Data.Path))
                return NotFound();

            return PhysicalFile(info.Data.Path, info.Data.MimeType, info.Data.FileName);
        }
    }
}
