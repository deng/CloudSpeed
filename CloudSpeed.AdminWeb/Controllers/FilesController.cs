using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CloudSpeed.Managers;
using CloudSpeed.AdminWeb.Requests;
using CloudSpeed.AdminWeb.Managers;

namespace CloudSpeed.AdminWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ApiControllerBase
    {
        private readonly FilesManager _FilesManager;

        public FilesController(FilesManager filesManager)
        {
            _FilesManager = filesManager;
        }

        [HttpPost("GetList")]
        public async Task<IActionResult> GetList([FromBody] FilesGetListRequest request)
        {
            var data = await _FilesManager.GetFiles(request);
            return Result(data);
        }

        [HttpPost("Import")]
        public async Task<IActionResult> Import([FromBody] FilesImportRequest request)
        {
            var data = await _FilesManager.CreateImport(request);
            return Result(data);
        }
    }
}
