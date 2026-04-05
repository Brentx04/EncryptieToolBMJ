using System.Security.Cryptography;
using System.Text;
using EncryptieTool.Web.Models.Api;
using EncryptieTool.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EncryptieTool.Web.Controllers.Api;

[ApiController]
[Route("api/signature")]
public class SignatureApiController : ControllerBase
{
    private readonly IDigitalSignatureService _signatureService;

    public SignatureApiController(IDigitalSignatureService signatureService)
    {
        _signatureService = signatureService;
    }

    [HttpPost("sign")]
    public IActionResult Sign([FromBody] SignRequest request)
    {
        try
        {
            var signature = _signatureService.Sign(Encoding.UTF8.GetBytes(request.Data), request.PrivateKeyPem);
            return Ok(new SignResponse(signature));
        }
        catch (Exception ex) when (ex is CryptographicException or FormatException or ArgumentException)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("verify")]
    public IActionResult Verify([FromBody] VerifySignatureRequest request)
    {
        try
        {
            var isValid = _signatureService.Verify(Encoding.UTF8.GetBytes(request.Data), request.SignatureBase64, request.PublicKeyPem);
            return Ok(new VerifyResponse(isValid));
        }
        catch (Exception ex) when (ex is CryptographicException or FormatException or ArgumentException)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
