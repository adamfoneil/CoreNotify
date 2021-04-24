using CoreNotify.Attributes;
using CoreNotify.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CoreNotify.TestApp.Controllers
{
    public class EmailController : Controller
    {
        [CoreNotifyAuthorize]
        public IActionResult Content()
        {
            Response.Headers.AddSubjectLine("This is my subject line");
            return View();
        }
    }
}
