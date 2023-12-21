using System.Text.Json.Serialization;

namespace Tomsoft.DeveloperTask.Data.Models.Turnover.ByBusinessUnit;

public class TurnoverCalculationsResponse
{
    [JsonPropertyName("result")]
    public List<TurnoverCalculationsResult>? Results { get; set; }

    public IEnumerable<TurnoverCalculation>? Calculations => Results?.SelectMany(result => result.TurnoverCalculations);
}