using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PetProjectOne.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        internal int GetStatusCode(int statusCode)
        {
            return statusCode switch
            {
                200 => StatusCodes.Status200OK,
                400 => StatusCodes.Status400BadRequest,
                401 => StatusCodes.Status401Unauthorized,
                404 => StatusCodes.Status404NotFound,
                409 => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };
        }
    }
}
