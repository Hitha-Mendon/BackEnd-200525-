using System.Text.Json.Serialization;

namespace WisVestAPI.Models.Matrix
{   
    public class Product
{
    [JsonPropertyName("product_name")]
    public string ProductName { get; set; }

    [JsonPropertyName("annual_return")]
    public double AnnualReturn { get; set; }

    [JsonPropertyName("asset_class")]
    public string AssetClass { get; set; }

    [JsonPropertyName("sub_asset_class")]
    public string SubAssetClass { get; set; }

    [JsonPropertyName("liquidity")]
    public string Liquidity { get; set; }

    [JsonPropertyName("pros")]
    public List<string> Pros { get; set; }

    [JsonPropertyName("cons")]
    public List<string> Cons { get; set; }

    [JsonPropertyName("description")]
    public string description { get; set; } 

    [JsonPropertyName("risk_level")]
    public string RiskLevel { get; set; }

    [JsonPropertyName("percentage_split")]
    public double PercentageSplit { get; set; }
    public double InvestmentAmount { get; set; }

}
}