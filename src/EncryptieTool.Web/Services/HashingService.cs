using System.Security.Cryptography;
using System.Text;
using EncryptieTool.Web.Models.Enums;
using EncryptieTool.Web.Services.Interfaces;

namespace EncryptieTool.Web.Services;

public class HashingService : IHashingService
{
    public string HashText(string input, HashAlgorithmType algorithm)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        using var hasher = CreateHashAlgorithm(algorithm);
        return Convert.ToHexString(hasher.ComputeHash(bytes)).ToLower();
    }

    public async Task<string> HashStreamAsync(Stream stream, HashAlgorithmType algorithm)
    {
        using var hasher = CreateHashAlgorithm(algorithm);
        return Convert.ToHexString(await hasher.ComputeHashAsync(stream)).ToLower();
    }

    public string GenerateHmac(string message, string keyBase64, HashAlgorithmType algorithm)
    {
        var key = Convert.FromBase64String(keyBase64);
        var messageBytes = Encoding.UTF8.GetBytes(message);
        return Convert.ToBase64String(ComputeHmac(messageBytes, key, algorithm));
    }

    public bool VerifyHmac(string message, string keyBase64, string hmacBase64, HashAlgorithmType algorithm)
    {
        var key = Convert.FromBase64String(keyBase64);
        var messageBytes = Encoding.UTF8.GetBytes(message);
        var expectedHmac = Convert.FromBase64String(hmacBase64);
        var actualHmac = ComputeHmac(messageBytes, key, algorithm);
        return CryptographicOperations.FixedTimeEquals(actualHmac, expectedHmac);
    }

    private static HashAlgorithm CreateHashAlgorithm(HashAlgorithmType algorithm) => algorithm switch
    {
        HashAlgorithmType.MD5    => MD5.Create(),
        HashAlgorithmType.SHA1   => SHA1.Create(),
        HashAlgorithmType.SHA256 => SHA256.Create(),
        HashAlgorithmType.SHA384 => SHA384.Create(),
        HashAlgorithmType.SHA512 => SHA512.Create(),
        _ => throw new ArgumentOutOfRangeException(nameof(algorithm))
    };

    private static byte[] ComputeHmac(byte[] data, byte[] key, HashAlgorithmType algorithm)
    {
        using HMAC hmac = algorithm switch
        {
            HashAlgorithmType.SHA384 => new HMACSHA384(key),
            HashAlgorithmType.SHA512 => new HMACSHA512(key),
            _                        => new HMACSHA256(key)
        };
        return hmac.ComputeHash(data);
    }
}
