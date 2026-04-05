using System.Text;
using EncryptieTool.Web.Services;
using Xunit;

namespace EncryptieTool.Tests.Services;

public class DigitalSignatureServiceTests
{
    private readonly DigitalSignatureService _service = new();
    private readonly KeyGenerationService _keyService = new();

    [Fact]
    public void Sign_ThenVerify_WithMatchingKeys_ReturnsTrue()
    {
        var (publicPem, privatePem) = _keyService.GenerateRsaKeyPair(2048);
        var data = Encoding.UTF8.GetBytes("Dit document is authentiek.");

        var signature = _service.Sign(data, privatePem);
        var isValid = _service.Verify(data, signature, publicPem);

        Assert.True(isValid);
    }

    [Fact]
    public void Verify_WithTamperedData_ReturnsFalse()
    {
        var (publicPem, privatePem) = _keyService.GenerateRsaKeyPair(2048);
        var originalData = Encoding.UTF8.GetBytes("Origineel document.");
        var tamperedData = Encoding.UTF8.GetBytes("Gewijzigd document!");

        var signature = _service.Sign(originalData, privatePem);
        var isValid = _service.Verify(tamperedData, signature, publicPem);

        Assert.False(isValid);
    }

    [Fact]
    public void Verify_WithWrongPublicKey_ReturnsFalse()
    {
        var (_, privatePem) = _keyService.GenerateRsaKeyPair(2048);
        var (wrongPublicPem, _) = _keyService.GenerateRsaKeyPair(2048);
        var data = Encoding.UTF8.GetBytes("Testdata");

        var signature = _service.Sign(data, privatePem);
        var isValid = _service.Verify(data, signature, wrongPublicPem);

        Assert.False(isValid);
    }
}
