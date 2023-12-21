using System.Text.Json.Serialization;

namespace Tomsoft.DeveloperTask.Data.Models.Warehouses;

public class Warehouse
{
    [JsonPropertyName("skladiste_uid")]
    public string WarehouseUid { get; set; }

    [JsonPropertyName("skladiste")]
    public string TheWarehouse { get; set; }

    [JsonPropertyName("naziv")]
    public string Name { get; set; }

    [JsonPropertyName("pj_uid")]
    public string BusinessUnitUid { get; set; }

    [JsonPropertyName("pj")]
    public string BusinessUnit { get; set; }

    [JsonPropertyName("pj_naziv")]
    public string BusinessUnitName { get; set; }

    [JsonPropertyName("adresa")]
    public string Address { get; set; }

    [JsonPropertyName("postanski_broj")]
    public string ZipCode { get; set; }

    [JsonPropertyName("mjesto")]
    public string City { get; set; }

    [JsonPropertyName("telefax")]
    public string Telefax { get; set; }

    [JsonPropertyName("e_mail")]
    public string Email { get; set; }
}