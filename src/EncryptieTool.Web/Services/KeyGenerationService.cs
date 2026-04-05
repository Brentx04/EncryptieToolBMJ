using System.Security.Cryptography;
using EncryptieTool.Web.Services.Interfaces;

namespace EncryptieTool.Web.Services;

public class KeyGenerationService : IKeyGenerationService
{
    public (string KeyBase64, string KeyHex, string IvBase64, string IvHex) GenerateAesKey(int keySizeBits)
    {
        using var aes = Aes.Create();
        aes.KeySize = keySizeBits;
        aes.GenerateKey();
        aes.GenerateIV();

        return (
            Convert.ToBase64String(aes.Key),
            Convert.ToHexString(aes.Key).ToLower(),
            Convert.ToBase64String(aes.IV),
            Convert.ToHexString(aes.IV).ToLower()
        );
    }

    public (string PublicKeyPem, string PrivateKeyPem) GenerateRsaKeyPair(int keySizeBits)
    {
        using var rsa = RSA.Create(keySizeBits);
        return (rsa.ExportRSAPublicKeyPem(), rsa.ExportRSAPrivateKeyPem());
    }
}
