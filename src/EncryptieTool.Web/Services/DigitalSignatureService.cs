using System.Security.Cryptography;
using EncryptieTool.Web.Services.Interfaces;

namespace EncryptieTool.Web.Services;

public class DigitalSignatureService : IDigitalSignatureService
{
    public string Sign(byte[] data, string privateKeyPem)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyPem);
        return Convert.ToBase64String(rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
    }

    public bool Verify(byte[] data, string signatureBase64, string publicKeyPem)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyPem);
        return rsa.VerifyData(data, Convert.FromBase64String(signatureBase64), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}
