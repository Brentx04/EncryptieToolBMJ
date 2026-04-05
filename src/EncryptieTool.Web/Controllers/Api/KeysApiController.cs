using EncryptieTool.Web.Models.Api;
using EncryptieTool.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EncryptieTool.Web.Controllers.Api;

[ApiController]
[Route("api/keys")]
public class KeysApiController : ControllerBase
{
    private readonly IKeyGenerationService _keyService;

    public KeysApiController(IKeyGenerationService keyService)
    {
        _keyService = keyService;
    }

    [HttpPost("aes")]
    public IActionResult GenerateAes([FromBody] GenerateAesKeyRequest request)
    {
        if (request.KeySize is not (128 or 192 or 256))
            return BadRequest(new { error = "Sleutelgrootte moet 128, 192 of 256 zijn." });

        var (keyBase64, keyHex, ivBase64, ivHex) = _keyService.GenerateAesKey(request.KeySize);
        return Ok(new AesKeyResponse(keyBase64, keyHex, ivBase64, ivHex, request.KeySize));
    }

    [HttpPost("rsa")]
    public IActionResult GenerateRsa([FromBody] GenerateRsaKeyRequest request)
    {
        if (request.KeySize is not (1024 or 2048 or 4096))
            return BadRequest(new { error = "Sleutelgrootte moet 1024, 2048 of 4096 zijn." });

        var (publicPem, privatePem) = _keyService.GenerateRsaKeyPair(request.KeySize);
        return Ok(new RsaKeyResponse(publicPem, privatePem, request.KeySize));
    }
}
