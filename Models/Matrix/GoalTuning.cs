using System.Text.Json;
using System.Text.Json.Serialization;
namespace WisVestAPI.Models.Matrix
{
    public class GoalTuning
    {
        public EmergencyFund EmergencyFund { get; set; }
        public Retirement Retirement { get; set; }
        public WealthAccumulation WealthAccumulation { get; set; }
        public ChildEducation ChildEducation { get; set; }
        public BigPurchase BigPurchase { get; set; }

        public static GoalTuning FromDictionary(Dictionary<string, Dictionary<string, object>> raw)
        {
            return new GoalTuning
            {
                EmergencyFund = raw.ContainsKey("Emergency Fund") 
            ? JsonSerializer.Deserialize<EmergencyFund>(JsonSerializer.Serialize(raw["Emergency Fund"])) 
            : new EmergencyFund(),
        Retirement = raw.ContainsKey("Retirement") 
            ? JsonSerializer.Deserialize<Retirement>(JsonSerializer.Serialize(raw["Retirement"])) 
            : new Retirement(),
        WealthAccumulation = raw.ContainsKey("Wealth Accumulation") 
            ? JsonSerializer.Deserialize<WealthAccumulation>(JsonSerializer.Serialize(raw["Wealth Accumulation"])) 
            : new WealthAccumulation(),
        ChildEducation = raw.ContainsKey("Child Education") 
            ? JsonSerializer.Deserialize<ChildEducation>(JsonSerializer.Serialize(raw["Child Education"])) 
            : new ChildEducation(),
        BigPurchase = raw.ContainsKey("Big Purchase") 
            ? JsonSerializer.Deserialize<BigPurchase>(JsonSerializer.Serialize(raw["Big Purchase"])) 
            : new BigPurchase()
            };
        }
    }

    public class EmergencyFund
{
    [JsonPropertyName("cash_min")]
    public double CashMinimum { get; set; }
}

public class Retirement
{
    [JsonPropertyName("fixedIncome_boost")]
    public double FixedIncomeBoost { get; set; }

    [JsonPropertyName("realEstate_boost")]
    public double RealEstateBoost { get; set; }
}

public class WealthAccumulation
{
    [JsonPropertyName("equity_priority")]
    public bool EquityPriority { get; set; }

    public double EquityBoost { get; set; } = 2;
    public double EquityThreshold { get; set; } = 40;
}

public class ChildEducation
{
    [JsonPropertyName("fixedIncome_boost")]
    public double FixedIncomeBoost { get; set; }

    [JsonPropertyName("equity_threshold")]
    public double EquityThreshold { get; set; }

    [JsonPropertyName("equityReduction_high")]
    public double EquityReductionHigh { get; set; }

    [JsonPropertyName("equityReduction_moderate")]
    public double EquityReductionModerate { get; set; }
}

public class BigPurchase
{
    [JsonPropertyName("balanced")]
    public bool Balanced { get; set; }
}

}
