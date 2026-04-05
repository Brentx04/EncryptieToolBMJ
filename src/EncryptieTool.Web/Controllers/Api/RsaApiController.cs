using System.Security.Cryptography;
using EncryptieTool.Web.Models.Api;
using EncryptieTool.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EncryptieTool.Web.Controllers.Api;

[ApiController]
[Route("api/rsa")]
public class RsaApiController : ControllerBase
{
    private readonly IRsaEncryptionService _rsaService;

    public RsaApiController(IRsaEncryptionService rsaService)
    {
        _rsaService = rsaService;
    }

    [HttpPost("encrypt")]
    public IActionResult Encrypt([FromBody] RsaEncryptRequest request)
    {
        try
        {
            var aesKeyBytes = Convert.FromBase64String(request.AesKeyBase64);
            return Ok(new RsaKeyResultResponse(_rsaService.EncryptWithPublicKey(aesKeyBytes, request.PublicKeyPem)));
        }
        catch (Exception ex) when (ex is CryptographicException or FormatException or ArgumentException)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("decrypt")]
    public IActionResult Decrypt([FromBody] RsaDecryptRequest request)
    {
        try
        {
            var decrypted = _rsaService.DecryptWithPrivateKey(request.EncryptedKeyBase64, request.PrivateKeyPem);
            return Ok(new RsaKeyResultResponse(Convert.ToBase64String(decrypted)));
        }
        catch (Exception ex) when (ex is CryptographicException or FormatException or ArgumentException)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
