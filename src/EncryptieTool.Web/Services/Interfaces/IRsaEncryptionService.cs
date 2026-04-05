namespace EncryptieTool.Web.Services.Interfaces;

public interface IRsaEncryptionService
{
    string EncryptWithPublicKey(byte[] data, string publicKeyPem);
    byte[] DecryptWithPrivateKey(string cipherTextBase64, string privateKeyPem);
}
