namespace EncryptieTool.Web.Services.Interfaces;

public interface IKeyGenerationService
{
    (string KeyBase64, string KeyHex, string IvBase64, string IvHex) GenerateAesKey(int keySizeBits);
    (string PublicKeyPem, string PrivateKeyPem) GenerateRsaKeyPair(int keySizeBits);
}
