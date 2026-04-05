using System.Security.Cryptography;
using System.Text;
using EncryptieTool.Web.Models.Enums;
using EncryptieTool.Web.Services;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace EncryptieTool.Tests.Services;

public class AesEncryptionServiceTests
{
    private readonly AesEncryptionService _service = new();
    private readonly KeyGenerationService _keyService = new();

    private (string key, string iv) GenerateKeyIv(int bits = 256)
    {
        var (keyBase64, _, ivBase64, _) = _keyService.GenerateAesKey(bits);
        return (keyBase64, ivBase64);
    }

    [Theory]
    [InlineData(AesMode.CBC)]
    [InlineData(AesMode.ECB)]
    public void EncryptText_ThenDecryptText_ReturnsOriginal(AesMode mode)
    {
        var (key, iv) = GenerateKeyIv();
        const string original = "Hallo wereld! Dit is een test.";

        var cipherText = _service.EncryptText(original, key, iv, mode);
        var decrypted = _service.DecryptText(cipherText, key, iv, mode);

        Assert.Equal(original, decrypted);
    }

    [Fact]
    public void EncryptText_GcmMode_RoundTrip()
    {
        var (key, iv) = GenerateKeyIv();
        const string original = "GCM geauthenticeerde encryptie test.";

        var cipherText = _service.EncryptText(original, key, iv, AesMode.GCM);
        var decrypted = _service.DecryptText(cipherText, key, iv, AesMode.GCM);

        Assert.Equal(original, decrypted);
    }

    [Fact]
    public void DecryptText_WithWrongKey_ThrowsCryptographicException()
    {
        var (key, iv) = GenerateKeyIv();
        var (wrongKey, _) = GenerateKeyIv();
        const string original = "Geheime boodschap.";

        var cipherText = _service.EncryptText(original, key, iv, AesMode.CBC);

        Assert.ThrowsAny<Exception>(() => _service.DecryptText(cipherText, wrongKey, iv, AesMode.CBC));
    }

    [Fact]
    public async Task EncryptFile_ThenDecryptFile_PreservesFilenameAndContent()
    {
        var (key, iv) = GenerateKeyIv();
        const string originalContent = "Dit is de inhoud van het testbestand.";
        const string originalFileName = "testbestand.txt";

        var contentBytes = Encoding.UTF8.GetBytes(originalContent);
        var formFile = CreateFormFile(contentBytes, originalFileName);

        var encrypted = await _service.EncryptFileAsync(formFile, key, iv, AesMode.CBC);
        var encryptedFile = CreateFormFile(encrypted, originalFileName + ".enc");
        var (decryptedBytes, recoveredName) = await _service.DecryptFileAsync(encryptedFile, key, iv, AesMode.CBC);

        Assert.Equal(originalFileName, recoveredName);
        Assert.Equal(originalContent, Encoding.UTF8.GetString(decryptedBytes));
    }

    private static IFormFile CreateFormFile(byte[] content, string fileName)
    {
        var stream = new MemoryStream(content);
        return new FormFile(stream, 0, content.Length, "file", fileName);
    }
}
