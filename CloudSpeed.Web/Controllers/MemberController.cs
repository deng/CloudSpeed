using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CloudSpeed.Managers;
using CloudSpeed.Web.Requests;

namespace filshareapp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemberController : ApiControllerBase
    {
        private readonly MemberManager _MemberManager;

        public MemberController(MemberManager memberManager)
        {
            _MemberManager = memberManager;
        }

        [HttpPost("check")]
        public async Task<ActionResult> Check(MemberCheckRequest request)
        {
            var data = await _MemberManager.CheckMember(request);
            return Result(data);
        }

        [HttpPost("create")]
        public async Task<ActionResult> Create(MemberCreateRequest requst)
        {
            var data = await _MemberManager.CreateMember(requst);
            return Result(data);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(MemberLoginRequest requst)
        {
            var data = await _MemberManager.Login(requst);
            return Result(data);
        }
    }
}
