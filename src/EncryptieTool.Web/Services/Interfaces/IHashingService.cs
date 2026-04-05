using EncryptieTool.Web.Models.Enums;

namespace EncryptieTool.Web.Services.Interfaces;

public interface IHashingService
{
    string HashText(string input, HashAlgorithmType algorithm);
    Task<string> HashStreamAsync(Stream stream, HashAlgorithmType algorithm);
    string GenerateHmac(string message, string keyBase64, HashAlgorithmType algorithm);
    bool VerifyHmac(string message, string keyBase64, string hmacBase64, HashAlgorithmType algorithm);
}
