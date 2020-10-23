using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.DependencyInjection;
using CloudSpeed.Web.Responses;
using CloudSpeed.Services;
using CloudSpeed.Managers;

namespace filshareapp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        public string AuthrizedUser
        {
            get
            {
                var identity = User.Identity as System.Security.Claims.ClaimsIdentity;
                if (identity == null)
                    return null;
                return identity.Claims.Where(i => i.Type == "account").Select(i => i.Value).FirstOrDefault();
            }
        }

        public async Task<string> GetAuthrizedUserId()
        {
            var authrizedUser = AuthrizedUser;
            if (string.IsNullOrEmpty(authrizedUser)) return string.Empty;
            var memberManager = GlobalServices.ServiceProvider.GetService<MemberManager>();
            return await memberManager.GetUserId(authrizedUser);
        }

        protected OkObjectResult Ok<T>(T data)
        {
            return base.Ok(ApiResponse.Ok(data));
        }

        protected ObjectResult Result<T>(ApiResponse<T> response)
        {
            return base.Ok(response);
        }
    }
}