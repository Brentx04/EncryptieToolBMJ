using System.Security.Cryptography;
using EncryptieTool.Web.Models.Api;
using EncryptieTool.Web.Models.Enums;
using EncryptieTool.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EncryptieTool.Web.Controllers.Api;

[ApiController]
[Route("api/aes")]
public class AesApiController : ControllerBase
{
    private readonly IAesEncryptionService _aesService;

    public AesApiController(IAesEncryptionService aesService)
    {
        _aesService = aesService;
    }

    [HttpPost("encrypt-text")]
    public IActionResult EncryptText([FromBody] AesEncryptTextRequest request)
    {
        try
        {
            var result = _aesService.EncryptText(request.PlainText, request.KeyBase64, request.IvBase64, ParseMode(request.Mode));
            return Ok(new AesTextResponse(result));
        }
        catch (Exception ex) when (ex is CryptographicException or FormatException or ArgumentException)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("decrypt-text")]
    public IActionResult DecryptText([FromBody] AesDecryptTextRequest request)
    {
        try
        {
            var result = _aesService.DecryptText(request.CipherTextBase64, request.KeyBase64, request.IvBase64, ParseMode(request.Mode));
            return Ok(new AesTextResponse(result));
        }
        catch (Exception ex) when (ex is CryptographicException or FormatException or ArgumentException)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("encrypt-file")]
    [RequestSizeLimit(10_485_760)]
    public async Task<IActionResult> EncryptFile(IFormFile file, [FromForm] string keyBase64, [FromForm] string ivBase64, [FromForm] string mode = "CBC")
    {
        if (file == null) return BadRequest(new { error = "Geen bestand geselecteerd." });
        try
        {
            var encrypted = await _aesService.EncryptFileAsync(file, keyBase64, ivBase64, ParseMode(mode));
            return File(encrypted, "application/octet-stream", file.FileName + ".enc");
        }
        catch (Exception ex) when (ex is CryptographicException or FormatException or ArgumentException)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("decrypt-file")]
    [RequestSizeLimit(10_485_760)]
    public async Task<IActionResult> DecryptFile(IFormFile file, [FromForm] string keyBase64, [FromForm] string ivBase64, [FromForm] string mode = "CBC")
    {
        if (file == null) return BadRequest(new { error = "Geen bestand geselecteerd." });
        try
        {
            var (data, originalName) = await _aesService.DecryptFileAsync(file, keyBase64, ivBase64, ParseMode(mode));
            return File(data, "application/octet-stream", originalName);
        }
        catch (Exception ex) when (ex is CryptographicException or FormatException or ArgumentException)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private static AesMode ParseMode(string mode) => mode.ToUpper() switch
    {
        "CBC" => AesMode.CBC,
        "ECB" => AesMode.ECB,
        "GCM" => AesMode.GCM,
        _     => throw new ArgumentException($"Onbekende modus: {mode}.")
    };
}
