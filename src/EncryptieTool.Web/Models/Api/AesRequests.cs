namespace EncryptieTool.Web.Models.Api;

public record AesEncryptTextRequest(string PlainText, string KeyBase64, string IvBase64, string Mode = "CBC");
public record AesDecryptTextRequest(string CipherTextBase64, string KeyBase64, string IvBase64, string Mode = "CBC");
public record AesTextResponse(string Result);
