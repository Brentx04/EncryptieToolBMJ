namespace EncryptieTool.Web.Models.Api;

public record RsaEncryptRequest(string AesKeyBase64, string PublicKeyPem);
public record RsaDecryptRequest(string EncryptedKeyBase64, string PrivateKeyPem);
public record RsaKeyResultResponse(string ResultBase64);
