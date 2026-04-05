namespace EncryptieTool.Web.Models.Api;

public record SignRequest(string Data, string PrivateKeyPem);
public record VerifySignatureRequest(string Data, string SignatureBase64, string PublicKeyPem);
public record SignResponse(string SignatureBase64);
