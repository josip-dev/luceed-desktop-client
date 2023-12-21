using System.Text.Json.Serialization;

namespace Tomsoft.DeveloperTask.Data.Models.Turnover;

public class TurnoverCalculation
{
    [JsonPropertyName("vrste_placanja_uid")]
    public string PaymentTypeUid { get; set; }

    [JsonPropertyName("naziv")]
    public string Name { get; set; }

    [JsonPropertyName("iznos")]
    public decimal Amount { get; set; }

    [JsonPropertyName("nadgrupa_placanja_uid")]
    public string ParentPaymentTypeUid { get; set; }

    [JsonPropertyName("nadgrupa_placanja_naziv")]
    public string ParentPaymentTypeName { get; set; }
}