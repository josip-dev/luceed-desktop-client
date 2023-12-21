using System.Text.Json.Serialization;

namespace Tomsoft.DeveloperTask.Data.Models.Turnover;

public class TurnoverCalculationByArticle
{
    [JsonPropertyName("artikl_uid")]
    public string ArticleUid { get; set; }

    [JsonPropertyName("naziv_artikla")]
    public string Article { get; set; }

    [JsonPropertyName("kolicina")]
    public int Quantity { get; set; }

    [JsonPropertyName("iznos")]
    public decimal Amount { get; set; }

    [JsonPropertyName("usluga")]
    public string Service { get; set; }
}