using EncryptieTool.Web.Services;
using Xunit;

namespace EncryptieTool.Tests.Services;

public class KeyGenerationServiceTests
{
    private readonly KeyGenerationService _service = new();

    [Fact]
    public void GenerateAesKey_256Bits_ReturnsValidBase64()
    {
        var (keyBase64, keyHex, ivBase64, ivHex) = _service.GenerateAesKey(256);

        Assert.False(string.IsNullOrEmpty(keyBase64));
        Assert.False(string.IsNullOrEmpty(ivBase64));
        // 256 bits = 32 bytes => Base64 length = 44
        var keyBytes = Convert.FromBase64String(keyBase64);
        Assert.Equal(32, keyBytes.Length);
    }

    [Fact]
    public void GenerateAesKey_DifferentCalls_ReturnsDifferentKeys()
    {
        var (key1, _, _, _) = _service.GenerateAesKey(256);
        var (key2, _, _, _) = _service.GenerateAesKey(256);

        Assert.NotEqual(key1, key2);
    }

    [Theory]
    [InlineData(128, 16)]
    [InlineData(192, 24)]
    [InlineData(256, 32)]
    public void GenerateAesKey_CorrectKeySize(int bits, int expectedBytes)
    {
        var (keyBase64, _, _, _) = _service.GenerateAesKey(bits);
        var bytes = Convert.FromBase64String(keyBase64);
        Assert.Equal(expectedBytes, bytes.Length);
    }

    [Fact]
    public void GenerateRsaKeyPair_ContainsPemHeaders()
    {
        var (publicPem, privatePem) = _service.GenerateRsaKeyPair(2048);

        Assert.Contains("BEGIN RSA PUBLIC KEY", publicPem);
        Assert.Contains("END RSA PUBLIC KEY", publicPem);
        Assert.Contains("BEGIN RSA PRIVATE KEY", privatePem);
        Assert.Contains("END RSA PRIVATE KEY", privatePem);
    }

    [Fact]
    public void GenerateRsaKeyPair_DifferentCalls_ReturnsDifferentKeys()
    {
        var (pub1, _) = _service.GenerateRsaKeyPair(2048);
        var (pub2, _) = _service.GenerateRsaKeyPair(2048);

        Assert.NotEqual(pub1, pub2);
    }
}
