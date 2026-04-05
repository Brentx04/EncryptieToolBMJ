using System.Security.Cryptography;
using System.Text;
using EncryptieTool.Web.Models.Enums;
using EncryptieTool.Web.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace EncryptieTool.Web.Services;

public class AesEncryptionService : IAesEncryptionService
{
    public string EncryptText(string plainText, string keyBase64, string ivBase64, AesMode mode)
    {
        var key = Convert.FromBase64String(keyBase64);
        var iv = Convert.FromBase64String(ivBase64);
        var plainBytes = Encoding.UTF8.GetBytes(plainText);

        if (mode == AesMode.GCM)
            return Convert.ToBase64String(EncryptBytesGcm(plainBytes, key));

        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = mode == AesMode.ECB ? CipherMode.ECB : CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        cs.Write(plainBytes);
        cs.FlushFinalBlock();
        return Convert.ToBase64String(ms.ToArray());
    }

    public string DecryptText(string cipherTextBase64, string keyBase64, string ivBase64, AesMode mode)
    {
        var key = Convert.FromBase64String(keyBase64);
        var iv = Convert.FromBase64String(ivBase64);
        var cipherBytes = Convert.FromBase64String(cipherTextBase64);

        if (mode == AesMode.GCM)
            return Encoding.UTF8.GetString(DecryptGcm(cipherBytes, key));

        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = mode == AesMode.ECB ? CipherMode.ECB : CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream(cipherBytes);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(cs, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    public async Task<byte[]> EncryptFileAsync(IFormFile file, string keyBase64, string ivBase64, AesMode mode)
    {
        var key = Convert.FromBase64String(keyBase64);
        var iv = Convert.FromBase64String(ivBase64);

        using var inputStream = file.OpenReadStream();
        var fileBytes = new byte[file.Length];
        await inputStream.ReadExactlyAsync(fileBytes);

        var encryptedContent = mode == AesMode.GCM
            ? EncryptBytesGcm(fileBytes, key)
            : EncryptBytes(fileBytes, key, iv, mode);

        var fileNameBytes = Encoding.UTF8.GetBytes(file.FileName);
        using var result = new MemoryStream();
        result.Write(BitConverter.GetBytes(fileNameBytes.Length));
        result.Write(fileNameBytes);
        result.Write(encryptedContent);
        return result.ToArray();
    }

    public async Task<(byte[] Data, string OriginalFileName)> DecryptFileAsync(IFormFile encryptedFile, string keyBase64, string ivBase64, AesMode mode)
    {
        var key = Convert.FromBase64String(keyBase64);
        var iv = Convert.FromBase64String(ivBase64);

        using var inputStream = encryptedFile.OpenReadStream();
        var allBytes = new byte[encryptedFile.Length];
        await inputStream.ReadExactlyAsync(allBytes);

        var nameLength = BitConverter.ToInt32(allBytes, 0);
        if (nameLength < 0 || nameLength > 512)
            throw new ArgumentException("Ongeldig bestandsformaat.");

        var originalFileName = Encoding.UTF8.GetString(allBytes, 4, nameLength);
        var encryptedContent = allBytes[(4 + nameLength)..];

        var decryptedBytes = mode == AesMode.GCM
            ? DecryptGcm(encryptedContent, key)
            : DecryptBytes(encryptedContent, key, iv, mode);

        return (decryptedBytes, originalFileName);
    }

    private static byte[] EncryptBytes(byte[] data, byte[] key, byte[] iv, AesMode mode)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = mode == AesMode.ECB ? CipherMode.ECB : CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        cs.Write(data);
        cs.FlushFinalBlock();
        return ms.ToArray();
    }

    private static byte[] DecryptBytes(byte[] data, byte[] key, byte[] iv, AesMode mode)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = mode == AesMode.ECB ? CipherMode.ECB : CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream(data);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var output = new MemoryStream();
        cs.CopyTo(output);
        return output.ToArray();
    }

    private static byte[] EncryptBytesGcm(byte[] plainBytes, byte[] key)
    {
        var nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
        var tag = new byte[AesGcm.TagByteSizes.MaxSize];
        var ciphertext = new byte[plainBytes.Length];
        RandomNumberGenerator.Fill(nonce);

        using var aesGcm = new AesGcm(key, AesGcm.TagByteSizes.MaxSize);
        aesGcm.Encrypt(nonce, plainBytes, ciphertext, tag);

        var result = new byte[nonce.Length + tag.Length + ciphertext.Length];
        Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, result, nonce.Length, tag.Length);
        Buffer.BlockCopy(ciphertext, 0, result, nonce.Length + tag.Length, ciphertext.Length);
        return result;
    }

    private static byte[] DecryptGcm(byte[] data, byte[] key)
    {
        int nonceSize = AesGcm.NonceByteSizes.MaxSize;
        int tagSize = AesGcm.TagByteSizes.MaxSize;

        if (data.Length < nonceSize + tagSize)
            throw new CryptographicException("Ongeldige GCM-gegevens.");

        var nonce = data[..nonceSize];
        var tag = data[nonceSize..(nonceSize + tagSize)];
        var ciphertext = data[(nonceSize + tagSize)..];
        var plaintext = new byte[ciphertext.Length];

        using var aesGcm = new AesGcm(key, tagSize);
        aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);
        return plaintext;
    }
}
