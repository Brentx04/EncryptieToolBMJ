namespace EncryptieTool.Web.Models.Api;

public record HashTextRequest(string Text, string Algorithm = "SHA256");
public record HashResponse(string Hash, string Algorithm);
public record HmacGenerateRequest(string Message, string KeyBase64, string Algorithm = "SHA256");
public record HmacVerifyRequest(string Message, string KeyBase64, string HmacBase64, string Algorithm = "SHA256");
public record HmacResponse(string Hmac);
public record VerifyResponse(bool IsValid);
