using System.Security.Cryptography;
using EncryptieTool.Web.Services.Interfaces;

namespace EncryptieTool.Web.Services;

public class RsaEncryptionService : IRsaEncryptionService
{
    public string EncryptWithPublicKey(byte[] data, string publicKeyPem)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyPem);
        return Convert.ToBase64String(rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256));
    }

    public byte[] DecryptWithPrivateKey(string cipherTextBase64, string privateKeyPem)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyPem);
        return rsa.Decrypt(Convert.FromBase64String(cipherTextBase64), RSAEncryptionPadding.OaepSHA256);
    }
}
