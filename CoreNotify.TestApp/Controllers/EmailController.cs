using CoreNotify.Attributes;
using Microsoft.AspNetCore.Mvc;
using CoreNotify.Extensions;
using System.Threading.Tasks;

namespace CoreNotify.TestApp.Controllers
{
    public class EmailController : Controller
    {
        [CoreNotifyAuthorize]
        public async Task<IActionResult> Content()
        {
            Response.Headers.AddSubjectLine("This is my subject line");
            return View();
        }
    }
}
