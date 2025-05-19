using System.Text.Json.Serialization;

namespace WisVestAPI.Models.DTOs
{
    public class ProductDTO
{
    public string? ProductName { get; set; }
    public double AnnualReturn { get; set; }
    public string? AssetClass { get; set; }
    public string? SubAssetClass { get; set; }
    public string? Liquidity { get; set; }
    public List<string> Pros { get; set; }
    public List<string> Cons { get; set; }
    public string? description { get; set; }
    public string? RiskLevel { get; set; }

    // public double PercentageSplit { get; set; }
    // public double InvestmentAmount { get; set; }
}
}
