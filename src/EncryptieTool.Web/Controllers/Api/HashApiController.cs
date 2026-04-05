using System.Security.Cryptography;
using EncryptieTool.Web.Models.Api;
using EncryptieTool.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using HashAlgorithmType = EncryptieTool.Web.Models.Enums.HashAlgorithmType;

namespace EncryptieTool.Web.Controllers.Api;

[ApiController]
[Route("api/hash")]
public class HashApiController : ControllerBase
{
    private readonly IHashingService _hashService;

    public HashApiController(IHashingService hashService)
    {
        _hashService = hashService;
    }

    [HttpPost("text")]
    public IActionResult HashText([FromBody] HashTextRequest request)
    {
        try
        {
            var hash = _hashService.HashText(request.Text, ParseAlgorithm(request.Algorithm));
            return Ok(new HashResponse(hash, request.Algorithm.ToUpper()));
        }
        catch (Exception ex) when (ex is ArgumentException or CryptographicException)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("file")]
    [RequestSizeLimit(10_485_760)]
    public async Task<IActionResult> HashFile(IFormFile file, [FromForm] string algorithm = "SHA256")
    {
        if (file == null) return BadRequest(new { error = "Geen bestand geselecteerd." });
        try
        {
            using var stream = file.OpenReadStream();
            var hash = await _hashService.HashStreamAsync(stream, ParseAlgorithm(algorithm));
            return Ok(new HashResponse(hash, algorithm.ToUpper()));
        }
        catch (Exception ex) when (ex is ArgumentException or CryptographicException)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("hmac/generate")]
    public IActionResult GenerateHmac([FromBody] HmacGenerateRequest request)
    {
        try
        {
            var hmac = _hashService.GenerateHmac(request.Message, request.KeyBase64, ParseAlgorithm(request.Algorithm));
            return Ok(new HmacResponse(hmac));
        }
        catch (Exception ex) when (ex is FormatException or ArgumentException or CryptographicException)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("hmac/verify")]
    public IActionResult VerifyHmac([FromBody] HmacVerifyRequest request)
    {
        try
        {
            var isValid = _hashService.VerifyHmac(request.Message, request.KeyBase64, request.HmacBase64, ParseAlgorithm(request.Algorithm));
            return Ok(new VerifyResponse(isValid));
        }
        catch (Exception ex) when (ex is FormatException or ArgumentException or CryptographicException)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private static HashAlgorithmType ParseAlgorithm(string algorithm) => algorithm.ToUpper() switch
    {
        "MD5"    => HashAlgorithmType.MD5,
        "SHA1"   => HashAlgorithmType.SHA1,
        "SHA256" => HashAlgorithmType.SHA256,
        "SHA384" => HashAlgorithmType.SHA384,
        "SHA512" => HashAlgorithmType.SHA512,
        _        => throw new ArgumentException($"Onbekend algoritme: {algorithm}.")
    };
}
