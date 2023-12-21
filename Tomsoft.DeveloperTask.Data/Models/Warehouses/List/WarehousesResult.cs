using System.Text.Json.Serialization;

namespace Tomsoft.DeveloperTask.Data.Models.Warehouses.List;

public class WarehousesResult
{
    [JsonPropertyName("skladista")]
    public List<Warehouse> Warehouses { get; set; }
}