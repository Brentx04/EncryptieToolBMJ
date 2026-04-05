namespace EncryptieTool.Web.Services.Interfaces;

public interface IDigitalSignatureService
{
    string Sign(byte[] data, string privateKeyPem);
    bool Verify(byte[] data, string signatureBase64, string publicKeyPem);
}
