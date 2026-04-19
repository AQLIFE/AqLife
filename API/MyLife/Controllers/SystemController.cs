using Microsoft.AspNetCore.Mvc;
using MyLife.Core.API;
using MyLife.Core.Define;

namespace MyLife.Controllers
{

    public class SystemController
    {
        // private readonly IFilePolicy _filePolicy = filePolicy;

        [HttpGet("system/status")]
        public ActionResult<bool> Status()
        {
            return true;
        }
    }
}