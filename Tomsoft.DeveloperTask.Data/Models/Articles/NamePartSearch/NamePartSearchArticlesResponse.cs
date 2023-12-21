using System.Text.Json.Serialization;

namespace Tomsoft.DeveloperTask.Data.Models.Articles.NamePartSearch;

public class NamePartSearchArticlesResponse
{
    [JsonPropertyName("result")] public List<ArticlesResult> Results { get; set; }

    public IEnumerable<Article> Articles => Results.SelectMany(result => result.Articles);
}