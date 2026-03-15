using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublicController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public PublicController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("app-info")]
    public ActionResult<PublicAppInfoResponse> GetAppInfo()
    {
        return Ok(new PublicAppInfoResponse(
            _configuration["PublicApp:AppName"] ?? "eSchool",
            _configuration["PublicApp:Tagline"] ?? "School management made simple.",
            _configuration["PublicApp:SupportEmail"] ?? "support@eschool.app",
            _configuration["PublicApp:SupportPhone"] ?? string.Empty,
            _configuration["PublicApp:SupportWhatsApp"] ?? string.Empty,
            _configuration["PublicApp:HelpMessage"] ?? "Contact support for onboarding.",
            _configuration["PublicApp:LoginHint"] ?? "Use your login credentials to continue."
        ));
    }
}
