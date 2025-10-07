using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using eticket.Models;
using eticket.DTO;
using eticket.Core.Interfaces;

namespace eticket.Controllers;

[Authorize]
[Route("/{Controller}")]
public class AccountController(ILogger<HomeController> l, IResumeService rs) : Controller
{
    private readonly ILogger<HomeController> logger = l;
    private readonly IResumeService resumeService = rs;

    [HttpGet("AccessDenied")]
    public IActionResult AccessDenied([FromQuery] string? returnUrl)
    {
        return View();
    }

}