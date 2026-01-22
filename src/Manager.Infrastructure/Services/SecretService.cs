using System.Security.Cryptography;
using System.Text;

namespace Manager.Infrastructure.Services;

public interface ISecretService
{
    string Encrypt(string plainText);
    string Decrypt(string encryptedText);
}

public class SecretService : ISecretService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public SecretService(string encryptionKey)
    {
        if (string.IsNullOrEmpty(encryptionKey) || encryptionKey.Length < 32)
            throw new ArgumentException("Encryption key must be at least 32 characters", nameof(encryptionKey));

        using var sha256 = SHA256.Create();
        _key = sha256.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey));
        _iv = sha256.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey)).Take(16).ToArray();
    }

    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(plainText);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public string Decrypt(string encryptedText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(Convert.FromBase64String(encryptedText));
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}
