using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;

namespace Tomsoft.DeveloperTask.Data;

public class LuceedApiClient : IDisposable
{
    private const string ROOT = "http://apidemo.luceed.hr/datasnap/rest";
    
    private readonly HttpClient _httpClient = new();

    public LuceedApiClient(LuceedApiCredentials credentials)
    {
        _httpClient.DefaultRequestHeaders.Authorization = credentials.AuthenticationHeaderValue;
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
    }

    private string _url = string.Empty;
    public LuceedApiClient WithRouteParts(params string?[] routeParts)
    {
        var endpoint = string.Join('/', routeParts.Where(part => !string.IsNullOrWhiteSpace(part)));
        _url = string.Join('/', ROOT, endpoint);
        return this;
    }

    public async Task<bool> CanConnect(CancellationToken cancellationToken = default)
    {
        try
        {
            WithRouteParts("artikli", "naziv", "test");
            using var response = await _httpClient.GetAsync(_url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            Console.WriteLine($"Failed to connect: {(int)response.StatusCode} ({response.StatusCode})");
            Console.WriteLine($"Headers: {JsonSerializer.Serialize(response.Content.Headers)}");
            var responseText = await response.Content.ReadAsStringAsync(cancellationToken);
            Console.WriteLine($"Response: {responseText}");
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Failed while trying to connect: {exception}");
        }

        return false;
    }

    public async Task<T?> Get<T>(CancellationToken cancellationToken = default)
    {
        HttpResponseMessage? response = null;
        try
        {
            response = await _httpClient.GetAsync(_url, cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
            Console.WriteLine(
                $"Got result from '{_url}': {(result is null ? "Null" : JsonSerializer.Serialize(result, typeof(T), new JsonSerializerOptions { WriteIndented = true }))}");
            return result;
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Failed to get data from '{_url}': {exception}");
            
            if (response is not null)
            {
                var stringContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine($"Response content: {stringContent}");
            }
            
            return default;
        }
        finally
        {
            response?.Dispose();   
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}