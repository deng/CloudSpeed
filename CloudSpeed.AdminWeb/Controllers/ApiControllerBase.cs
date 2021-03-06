using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.DependencyInjection;
using CloudSpeed.Web.Responses;

namespace CloudSpeed.AdminWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected OkObjectResult Ok<T>(T data)
        {
            return base.Ok(ApiResponse.Ok(data).ToAdminResponse());
        }

        protected ObjectResult Result<T>(ApiResponse<T> response)
        {
            return base.Ok(response.ToAdminResponse());
        }
    }
}