using EncryptieTool.Web.Models.Enums;
using EncryptieTool.Web.Services;
using Xunit;

namespace EncryptieTool.Tests.Services;

public class HashingServiceTests
{
    private readonly HashingService _service = new();

    [Fact]
    public void HashText_SHA256_KnownValue_ReturnsExpectedHash()
    {
        // SHA-256 van lege string is een bekende waarde
        const string input = "";
        const string expectedHash = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";

        var result = _service.HashText(input, HashAlgorithmType.SHA256);

        Assert.Equal(expectedHash, result);
    }

    [Theory]
    [InlineData(HashAlgorithmType.MD5, 32)]
    [InlineData(HashAlgorithmType.SHA1, 40)]
    [InlineData(HashAlgorithmType.SHA256, 64)]
    [InlineData(HashAlgorithmType.SHA384, 96)]
    [InlineData(HashAlgorithmType.SHA512, 128)]
    public void HashText_ReturnsCorrectHashLength(HashAlgorithmType algorithm, int expectedHexLength)
    {
        var result = _service.HashText("test", algorithm);
        Assert.Equal(expectedHexLength, result.Length);
    }

    [Fact]
    public void GenerateHmac_ThenVerifyHmac_ReturnsTrue()
    {
        var keyBytes = new byte[32];
        System.Security.Cryptography.RandomNumberGenerator.Fill(keyBytes);
        var keyBase64 = Convert.ToBase64String(keyBytes);
        const string message = "Dit bericht moet geverifieerd worden.";

        var hmac = _service.GenerateHmac(message, keyBase64, HashAlgorithmType.SHA256);
        var isValid = _service.VerifyHmac(message, keyBase64, hmac, HashAlgorithmType.SHA256);

        Assert.True(isValid);
    }

    [Fact]
    public void VerifyHmac_WithWrongKey_ReturnsFalse()
    {
        var key1 = Convert.ToBase64String(new byte[32]);
        var key2 = Convert.ToBase64String(Enumerable.Repeat((byte)1, 32).ToArray());
        const string message = "Testbericht";

        var hmac = _service.GenerateHmac(message, key1, HashAlgorithmType.SHA256);
        var isValid = _service.VerifyHmac(message, key2, hmac, HashAlgorithmType.SHA256);

        Assert.False(isValid);
    }

    [Fact]
    public void VerifyHmac_WithTamperedMessage_ReturnsFalse()
    {
        var keyBase64 = Convert.ToBase64String(new byte[32]);
        const string original = "Origineel bericht";
        const string tampered = "Gewijzigd bericht";

        var hmac = _service.GenerateHmac(original, keyBase64, HashAlgorithmType.SHA256);
        var isValid = _service.VerifyHmac(tampered, keyBase64, hmac, HashAlgorithmType.SHA256);

        Assert.False(isValid);
    }
}
