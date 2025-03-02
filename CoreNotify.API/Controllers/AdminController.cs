using Microsoft.AspNetCore.Mvc;

namespace CoreNotify.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[VerifyAccount(requireAdmin: true)]
public class AdminController : Controller
{
}
