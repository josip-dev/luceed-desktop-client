using System.Text.Json.Serialization;

namespace Tomsoft.DeveloperTask.Data.Models.Articles.NamePartSearch;

public class ArticlesResult
{
    [JsonPropertyName("artikli")] public List<Article> Articles { get; set; }
}