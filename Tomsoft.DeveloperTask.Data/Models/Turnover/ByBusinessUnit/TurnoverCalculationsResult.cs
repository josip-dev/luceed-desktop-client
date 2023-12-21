using System.Text.Json.Serialization;

namespace Tomsoft.DeveloperTask.Data.Models.Turnover.ByBusinessUnit;

public class TurnoverCalculationsResult
{
    [JsonPropertyName("obracun_placanja")]
    public List<TurnoverCalculation> TurnoverCalculations { get; set; }
}