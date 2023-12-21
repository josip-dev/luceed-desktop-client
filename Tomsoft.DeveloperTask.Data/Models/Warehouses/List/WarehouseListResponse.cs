using System.Text.Json.Serialization;

namespace Tomsoft.DeveloperTask.Data.Models.Warehouses.List;

public class WarehouseListResponse
{
    [JsonPropertyName("result")]
    public List<WarehousesResult> Results { get; set; }

    public IEnumerable<Warehouse> Warehouses => Results.SelectMany(result => result.Warehouses).OrderBy(x => x.BusinessUnit);
}