using System.Text.Json.Serialization;

namespace Tomsoft.DeveloperTask.Data.Models.Turnover.ByArticle;

public class TurnoverCalculationsByArticleResult
{
    [JsonPropertyName("obracun_artikli")]
    public List<TurnoverCalculationByArticle> TurnoverCalculations { get; set; }
}