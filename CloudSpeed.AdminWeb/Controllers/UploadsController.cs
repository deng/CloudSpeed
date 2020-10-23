using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CloudSpeed.Managers;
using CloudSpeed.AdminWeb.Requests;
using CloudSpeed.AdminWeb.Managers;

namespace CloudSpeed.AdminWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadsController : ApiControllerBase
    {
        private readonly UploadsManager _UploadsManager;

        public UploadsController(UploadsManager uploadsManager)
        {
            _UploadsManager = uploadsManager;
        }

        [HttpPost("GetList")]
        public async Task<IActionResult> GetList([FromBody] UploadsGetListRequest request)
        {
            var data = await _UploadsManager.GetUploads(request);
            return Result(data);
        }
    }
}
