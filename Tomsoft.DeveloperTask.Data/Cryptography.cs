using System.Security.Cryptography;
using System.Text;

namespace Tomsoft.DeveloperTask.Data;

public class Cryptography
{
    // This constant is used to determine the keysize of the encryption algorithm in bits.
    // We divide this by 8 within the code below to get the equivalent number of bytes.
    private const int KEY_SIZE = 128;
    private const int NUM_BYTES = KEY_SIZE / 8;

    // This constant determines the number of iterations for the password bytes generation function.
    private const int DERIVATION_ITERATIONS = 1000;

    private readonly string _keyFilePath;
    private readonly Lazy<string> _keyLazy;

    private string Key => _keyLazy.Value;

    public Encoding Encoding { get; } = Encoding.UTF8;

    public Cryptography(string keyFilePath)
    {
        _keyFilePath = keyFilePath;
        _keyLazy = new Lazy<string>(GetKey);
    }

    public string? Encrypt(string? plainText)
    {
        if (plainText is null)
        {
            return null;
        }

        var saltStringBytes = RandomNumberGenerator.GetBytes(NUM_BYTES);
        var ivStringBytes = RandomNumberGenerator.GetBytes(NUM_BYTES);
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

        using var password = new Rfc2898DeriveBytes(Key, saltStringBytes, DERIVATION_ITERATIONS, HashAlgorithmName.SHA1);
        var keyBytes = password.GetBytes(NUM_BYTES);

        using var symmetricKey = Aes.Create();
        symmetricKey.BlockSize = KEY_SIZE;
        symmetricKey.Padding = PaddingMode.PKCS7;
        symmetricKey.Mode = CipherMode.CBC;

        using var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes);
        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
        cryptoStream.FlushFinalBlock();

        // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
        var cipherTextBytes = saltStringBytes.Concat(ivStringBytes).Concat(memoryStream.ToArray()).ToArray();

        return Convert.ToBase64String(cipherTextBytes);
    }

    public string? Decrypt(string? encryptedText)
    {
        if (encryptedText is null)
        {
            return null;
        }

        // Get the complete stream of bytes that represent:
        // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
        var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(encryptedText);
        // Get the salt bytes by extracting the first 32 bytes from the supplied cipherText bytes.
        var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(NUM_BYTES).ToArray();
        // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
        var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(NUM_BYTES).Take(NUM_BYTES).ToArray();
        // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
        var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip(NUM_BYTES * 2).Take(cipherTextBytesWithSaltAndIv.Length - NUM_BYTES * 2).ToArray();

        using var password = new Rfc2898DeriveBytes(Key, saltStringBytes, DERIVATION_ITERATIONS, HashAlgorithmName.SHA1);
        var keyBytes = password.GetBytes(NUM_BYTES);
        
        using var symmetricKey = Aes.Create();
        symmetricKey.BlockSize = KEY_SIZE;
        symmetricKey.Padding = PaddingMode.PKCS7;
        symmetricKey.Mode = CipherMode.CBC;
        
        using var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes);
        using var memoryStream = new MemoryStream(cipherTextBytes);
        using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        
        var plainTextBytes = new byte[cipherTextBytes.Length];
        var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
        
        return Encoding.GetString(plainTextBytes, 0, decryptedByteCount);
    }


    private string GetKey()
    {
        if (File.Exists(_keyFilePath))
        {
            return File.ReadAllText(_keyFilePath);
        }

        var key = Guid.NewGuid().ToString("N");
        File.WriteAllText(_keyFilePath, key);
        return key;
    }
}