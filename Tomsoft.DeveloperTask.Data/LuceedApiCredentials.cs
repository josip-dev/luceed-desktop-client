using System.Net;
using System.Net.Http.Headers;

namespace Tomsoft.DeveloperTask.Data;

public class LuceedApiCredentials
{
    private static readonly string ContainingDirectory = Project.RelativePath("Credentials");
    private static readonly string CredentialsFilePath = Path.Combine(ContainingDirectory, "credentials");
    
    private static readonly Cryptography Cryptography = new(Path.Combine(ContainingDirectory, "key"));
    
    public string? Username { get; private set; }
    public string? Password { get; private set; }

    public string BasicAuthString => $"{Username}:{Password}";

    public AuthenticationHeaderValue AuthenticationHeaderValue => new(
        AuthenticationSchemes.Basic.ToString(),
        Convert.ToBase64String(Cryptography.Encoding.GetBytes(BasicAuthString))
    );

    public LuceedApiCredentials()
    {
        Directory.CreateDirectory(ContainingDirectory);
    }

    public LuceedApiCredentials(string username, string password) : this()
    {
        Username = username;
        Password = password;
    }

    public async Task Load(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(CredentialsFilePath))
        {
            return;
        }
        
        await using var file = File.OpenRead(CredentialsFilePath);
        using var reader = new StreamReader(file);

        var username = await ReadValue(reader, cancellationToken);
        if (username is not null)
        {
            Username = username;
        }

        var password = await ReadValue(reader, cancellationToken);
        if (password is not null)
        {
            Password = password;
        }
    }

    public async Task Save(CancellationToken cancellationToken = default)
    {
        if (Username is null && Password is null)
        {
            return;
        }
        
        await using var file = File.Create(CredentialsFilePath);
        await using var writer = new StreamWriter(file);

        await WriteValue(Username, writer, cancellationToken);
        await WriteValue(Password, writer, cancellationToken);
    }

    private static async Task WriteValue(string? value, TextWriter writer, CancellationToken cancellationToken)
    {
        try
        {
            if (value is null)
            {
                return;
            }

            var encryptedString = Cryptography.Encrypt(value);
            await writer.WriteLineAsync(encryptedString.AsMemory(), cancellationToken);
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Failed to write the value: {exception}");
        }
    }

    private static async Task<string?> ReadValue(TextReader reader, CancellationToken cancellationToken)
    {
        try
        {
            var value = await reader.ReadLineAsync(cancellationToken);
            if (value is null)
            {
                return null;
            }

            var decryptedValue = Cryptography.Decrypt(value);
            return decryptedValue;
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Failed to read the value: {exception}");
            return null;
        }
    }
}