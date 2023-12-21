using System.Text.Json.Serialization;

namespace Tomsoft.DeveloperTask.Data.Models.Turnover.ByArticle;

public class TurnoverCalculationsByArticleResponse
{
    [JsonPropertyName("result")]
    public List<TurnoverCalculationsByArticleResult>? Results { get; set; }

    public IEnumerable<TurnoverCalculationByArticle>? Calculations => Results?.SelectMany(result => result.TurnoverCalculations);
}