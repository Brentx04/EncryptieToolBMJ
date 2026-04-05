using System.Security.Cryptography;
using EncryptieTool.Web.Services;
using Xunit;

namespace EncryptieTool.Tests.Services;

public class RsaEncryptionServiceTests
{
    private readonly RsaEncryptionService _service = new();
    private readonly KeyGenerationService _keyService = new();

    [Fact]
    public void EncryptWithPublicKey_ThenDecryptWithPrivateKey_ReturnsOriginalData()
    {
        var (publicPem, privatePem) = _keyService.GenerateRsaKeyPair(2048);
        var originalData = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        var encrypted = _service.EncryptWithPublicKey(originalData, publicPem);
        var decrypted = _service.DecryptWithPrivateKey(encrypted, privatePem);

        Assert.Equal(originalData, decrypted);
    }

    [Fact]
    public void EncryptWithPublicKey_EncryptAesKey_ThenDecrypt_ReturnsOriginalKey()
    {
        var (publicPem, privatePem) = _keyService.GenerateRsaKeyPair(2048);
        var (aesKeyBase64, _, _, _) = new KeyGenerationService().GenerateAesKey(256);
        var aesKeyBytes = Convert.FromBase64String(aesKeyBase64);

        var encryptedKey = _service.EncryptWithPublicKey(aesKeyBytes, publicPem);
        var decryptedKeyBytes = _service.DecryptWithPrivateKey(encryptedKey, privatePem);

        Assert.Equal(aesKeyBase64, Convert.ToBase64String(decryptedKeyBytes));
    }

    [Fact]
    public void DecryptWithWrongPrivateKey_ThrowsCryptographicException()
    {
        var (publicPem, _) = _keyService.GenerateRsaKeyPair(2048);
        var (_, wrongPrivatePem) = _keyService.GenerateRsaKeyPair(2048);
        var data = new byte[] { 42, 43, 44 };

        var encrypted = _service.EncryptWithPublicKey(data, publicPem);

        Assert.Throws<CryptographicException>(() => _service.DecryptWithPrivateKey(encrypted, wrongPrivatePem));
    }
}
