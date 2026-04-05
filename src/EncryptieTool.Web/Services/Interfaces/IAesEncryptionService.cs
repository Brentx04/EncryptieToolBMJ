using EncryptieTool.Web.Models.Enums;
using Microsoft.AspNetCore.Http;

namespace EncryptieTool.Web.Services.Interfaces;

public interface IAesEncryptionService
{
    string EncryptText(string plainText, string keyBase64, string ivBase64, AesMode mode);
    string DecryptText(string cipherTextBase64, string keyBase64, string ivBase64, AesMode mode);
    Task<byte[]> EncryptFileAsync(IFormFile file, string keyBase64, string ivBase64, AesMode mode);
    Task<(byte[] Data, string OriginalFileName)> DecryptFileAsync(IFormFile encryptedFile, string keyBase64, string ivBase64, AesMode mode);
}
