using System.Text.Json.Serialization;

namespace Tomsoft.DeveloperTask.Data.Models.Articles;

public class Article
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("naziv")] public string Name { get; set; }
}