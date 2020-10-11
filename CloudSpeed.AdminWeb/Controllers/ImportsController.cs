using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CloudSpeed.Managers;
using CloudSpeed.AdminWeb.Requests;
using CloudSpeed.AdminWeb.Managers;

namespace CloudSpeed.AdminWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportsController : ApiControllerBase
    {
        private readonly ImportsManager _ImportsManager;

        public ImportsController(ImportsManager importsManager)
        {
            _ImportsManager = importsManager;
        }

        [HttpPost("GetList")]
        public async Task<IActionResult> GetList([FromBody] ImportsGetListRequest request)
        {
            var data = await _ImportsManager.GetImports(request);
            return Result(data);
        }
    }
}
