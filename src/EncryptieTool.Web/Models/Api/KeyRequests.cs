namespace EncryptieTool.Web.Models.Api;

public record GenerateAesKeyRequest(int KeySize = 256);
public record GenerateRsaKeyRequest(int KeySize = 2048);

public record AesKeyResponse(
    string KeyBase64, string KeyHex,
    string IvBase64, string IvHex,
    int KeySizeBits);

public record RsaKeyResponse(string PublicKeyPem, string PrivateKeyPem, int KeySizeBits);
