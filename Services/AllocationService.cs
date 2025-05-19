// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Text.Json;
// using System.Threading.Tasks;
// using Microsoft.Extensions.Logging;
// using WisVestAPI.Constants;
// using WisVestAPI.Models.DTOs;
// using WisVestAPI.Models.Matrix;
// using WisVestAPI.Repositories.Matrix;
// using WisVestAPI.Services.Interfaces;

// namespace WisVestAPI.Services
// {
//     public class AllocationService : IAllocationService
//     {
//         private readonly MatrixRepository _matrixRepository;
//         private readonly ILogger<AllocationService> _logger;

//         public AllocationService(MatrixRepository matrixRepository, ILogger<AllocationService> logger)
//         {
//             _matrixRepository = matrixRepository;
//             _logger = logger;
//         }

//         private async Task SaveFinalAllocationToFileAsync(Dictionary<string, object> finalAllocation)
//         {
//             try
//             {
//                 var filePath = AppConstants.FinalAllocationFilePath;
//                 var options = new JsonSerializerOptions { WriteIndented = true };
//                 string jsonString = JsonSerializer.Serialize(finalAllocation, options);
//                 await File.WriteAllTextAsync(filePath, jsonString);
//                 _logger.LogInformation(ResponseMessages.FinalAllocationSaved, filePath);
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, ResponseMessages.AllocationFileSaveError);
//                 throw;
//             }
//         }

//         public async Task<Dictionary<string, object>> CalculateFinalAllocation(UserInputDTO input)
//         {
//             try
//             {
//                 _logger.LogInformation(ResponseMessages.AllocationCalculationStarted);

//                 var allocationMatrix = await _matrixRepository.LoadMatrixDataAsync();
//                 if (allocationMatrix == null)
//                 {
//                     _logger.LogError(ResponseMessages.AllocationMatrixNull);
//                     throw new InvalidOperationException(ResponseMessages.AllocationMatrixNull);
//                 }

//                 _logger.LogInformation(ResponseMessages.AllocationMatrixLoaded);

//                 var riskToleranceMap = new Dictionary<string, string>
//                 {
//                     { AppConstants.RiskLow, AppConstants.RiskLow },
//                     { AppConstants.RiskMedium, AppConstants.RiskMidMapped },
//                     { AppConstants.RiskHigh, AppConstants.RiskHigh }
//                 };

//                 var investmentHorizonMap = new Dictionary<string, string>
//                 {
//                     { AppConstants.HorizonShort, AppConstants.HorizonShortMapped },
//                     { AppConstants.HorizonModerate, AppConstants.HorizonModerateMapped },
//                     { AppConstants.HorizonLong, AppConstants.HorizonLongMapped }
//                 };

//                 if (input.InvestmentHorizon <= 0)
//                     throw new ArgumentException(ResponseMessages.HorizonMissing);

//                 var riskToleranceKey = riskToleranceMap[input.RiskTolerance ?? throw new ArgumentException(ResponseMessages.RiskToleranceMissing)];
//                 var horizonGroup = GetHorizonGroup(input.InvestmentHorizon);
//                 var investmentHorizonKey = investmentHorizonMap[horizonGroup];
//                 var riskHorizonKey = $"{riskToleranceKey}+{investmentHorizonKey}";

//                 _logger.LogInformation(ResponseMessages.BaseAllocationLookup, riskHorizonKey);

//                 if (!allocationMatrix.Risk_Horizon_Allocation.TryGetValue(riskHorizonKey, out var baseAllocation))
//                 {
//                     _logger.LogError(ResponseMessages.BaseAllocationNotFound, riskHorizonKey);
//                     throw new ArgumentException(string.Format(ResponseMessages.BaseAllocationInvalidCombo, riskHorizonKey));
//                 }

//                 _logger.LogInformation(ResponseMessages.BaseAllocationFound, JsonSerializer.Serialize(baseAllocation));

//                 var finalAllocation = new Dictionary<string, double>(baseAllocation);

//                 try
//                 {
//                     var ageRuleKey = GetAgeGroup(input.Age);
//                     _logger.LogInformation(ResponseMessages.AgeAdjustmentLookup, ageRuleKey);

//                     if (allocationMatrix.Age_Adjustment_Rules.TryGetValue(ageRuleKey, out var ageAdjustments))
//                     {
//                         _logger.LogInformation(ResponseMessages.AgeAdjustmentsFound, JsonSerializer.Serialize(ageAdjustments));
//                         foreach (var adjustment in ageAdjustments)
//                         {
//                             if (finalAllocation.ContainsKey(adjustment.Key))
//                             {
//                                 finalAllocation[adjustment.Key] += adjustment.Value;
//                             }
//                         }
//                     }
//                     else
//                     {
//                         _logger.LogWarning(ResponseMessages.AgeAdjustmentNotFound, ageRuleKey);
//                     }
//                 }
//                 catch (Exception ex)
//                 {
//                     _logger.LogError(ex, ResponseMessages.AgeAdjustmentError, ex.Message);
//                     throw;
//                 }

//                 try
//                 {
//                     _logger.LogInformation(ResponseMessages.GoalTuningLookup, input.Goal);
//                     if (string.IsNullOrEmpty(input.Goal))
//                     {
//                         throw new ArgumentException(ResponseMessages.GoalMissing);
//                     }
//                     if (allocationMatrix.Goal_Tuning.TryGetValue(input.Goal, out var goalTuning))
//                     {
//                         // Logic continues...
// _logger.LogInformation(ResponseMessages.GoalTuningFound, JsonSerializer.Serialize(goalTuning));
// switch (input.Goal)
// {
//     case AppConstants.Goals.EmergencyFund:
//         if (finalAllocation.ContainsKey(AppConstants.AssetKeys.Cash) && finalAllocation[AppConstants.AssetKeys.Cash] < AppConstants.Thresholds.EmergencyFundCashMinimum)
//         {
//             var cashDeficit = AppConstants.Thresholds.EmergencyFundCashMinimum - finalAllocation[AppConstants.AssetKeys.Cash];
//             finalAllocation[AppConstants.AssetKeys.Cash] += cashDeficit;

//             var categoriesToReduce = new[]
//             {
//                 AppConstants.AssetKeys.Equity,
//                 AppConstants.AssetKeys.FixedIncome,
//                 AppConstants.AssetKeys.Commodities,
//                 AppConstants.AssetKeys.RealEstate
//             };

//             var totalReduction = categoriesToReduce
//                 .Where(finalAllocation.ContainsKey)
//                 .Sum(category => Math.Max(0, finalAllocation[category]));

//             if (totalReduction < cashDeficit)
//             {
//                 cashDeficit = totalReduction;
//                 finalAllocation[AppConstants.AssetKeys.Cash] += cashDeficit;
//             }

//             foreach (var category in categoriesToReduce)
//             {
//                 if (finalAllocation.ContainsKey(category) && finalAllocation[category] > 0)
//                 {
//                     var reduction = Math.Min(finalAllocation[category], (cashDeficit / totalReduction) * finalAllocation[category]);
//                     finalAllocation[category] = Math.Max(0, finalAllocation[category] - reduction);
//                 }
//             }
//         }
//         break;

//     case AppConstants.Goals.Retirement:
//         if (goalTuning.TryGetValue(AppConstants.GoalTuningKeys.FixedIncomeBoost, out var fixedIncomeBoost) &&
//             finalAllocation.ContainsKey(AppConstants.AssetKeys.FixedIncome))
//         {
//             finalAllocation[AppConstants.AssetKeys.FixedIncome] =
//                 Math.Max(0, finalAllocation[AppConstants.AssetKeys.FixedIncome] + GetDoubleFromObject(fixedIncomeBoost));
//         }

//         if (goalTuning.TryGetValue(AppConstants.GoalTuningKeys.RealEstateBoost, out var realEstateBoost) &&
//             finalAllocation.ContainsKey(AppConstants.AssetKeys.RealEstate))
//         {
//             finalAllocation[AppConstants.AssetKeys.RealEstate] =
//                 Math.Max(0, finalAllocation[AppConstants.AssetKeys.RealEstate] + GetDoubleFromObject(realEstateBoost));
//         }
//         break;

//     case AppConstants.Goals.WealthAccumulation:
//         if (finalAllocation.ContainsKey(AppConstants.AssetKeys.Equity) &&
//             finalAllocation.Values.Any() &&
//             finalAllocation[AppConstants.AssetKeys.Equity] < finalAllocation.Values.Max())
//         {
//             finalAllocation[AppConstants.AssetKeys.Equity] += AppConstants.Adjustments.EquityBoost;

//             var sumAfterEquityBoost = finalAllocation.Values.Sum();
//             var remainingAdjustment = AppConstants.Thresholds.TotalAllocation - sumAfterEquityBoost;

//             var otherKeys = finalAllocation.Keys.Where(k => k != AppConstants.AssetKeys.Equity).ToList();
//             foreach (var key in otherKeys)
//             {
//                 if (finalAllocation.ContainsKey(key))
//                 {
//                     finalAllocation[key] = Math.Max(0, finalAllocation[key] + (remainingAdjustment / otherKeys.Count));
//                 }
//             }

//             var totalAfterAdjustment = finalAllocation.Values.Sum();
//             if (Math.Abs(totalAfterAdjustment - AppConstants.Thresholds.TotalAllocation) > AppConstants.Thresholds.Tolerance)
//             {
//                 var keyToAdjust = finalAllocation.OrderByDescending(kv => kv.Value).First().Key;
//                 finalAllocation[keyToAdjust] += AppConstants.Thresholds.TotalAllocation - totalAfterAdjustment;
//             }
//         }
//         break;
//  case AppConstants.Goals.ChildEducation:
//         if (goalTuning.TryGetValue(AppConstants.GoalTuningKeys.FixedIncomeBoost, out var fixedIncomeBoostChild) && finalAllocation.ContainsKey(AppConstants.AssetKeys.FixedIncome))
//         {
//             finalAllocation[AppConstants.AssetKeys.FixedIncome] += GetDoubleFromObject(fixedIncomeBoostChild);
//         }
//         if (goalTuning.TryGetValue(AppConstants.GoalTuningKeys.EquityReductionModerate, out var equityReduction) && finalAllocation.ContainsKey(AppConstants.AssetKeys.Equity))
//         {
//             var reductionAmount = GetDoubleFromObject(equityReduction);
//             finalAllocation[AppConstants.AssetKeys.Equity] = Math.Max(0, finalAllocation[AppConstants.AssetKeys.Equity] - reductionAmount);
//         }
//         break;

//     case AppConstants.Goals.BigPurchase:
//         if (goalTuning.TryGetValue(AppConstants.GoalTuningKeys.Balanced, out var balancedObj) &&
//             bool.TryParse(balancedObj.ToString(), out var balanced) && balanced)
//         {
//             _logger.LogInformation(ResponseMessages.GoalTuning.BigPurchaseBalancingEnabled);

//             var threshold = AppConstants.Thresholds.BigPurchaseCapPercentage;
//             var keys = finalAllocation.Keys.ToList();
//             double totalExcess = 0.0;

//             foreach (var assetKey in keys)
//             {
//                 if (finalAllocation[assetKey] > threshold)
//                 {
//                     double excess = finalAllocation[assetKey] - threshold;
//                     totalExcess += excess;
//                     finalAllocation[assetKey] = threshold;
//                     _logger.LogInformation(ResponseMessages.GoalTuning.BigPurchaseCapLog, assetKey, threshold, excess);
//                 }
//             }

//             var underThresholdKeys = keys.Where(k => finalAllocation[k] < threshold).ToList();
//             int count = underThresholdKeys.Count;

//             if (count > 0 && totalExcess > 0)
//             {
//                 double share = totalExcess / count;
//                 foreach (var key in underThresholdKeys)
//                 {
//                     finalAllocation[key] += share;
//                     _logger.LogInformation(ResponseMessages.GoalTuning.BigPurchaseShareLog, share, key, finalAllocation[key]);
//                 }
//             }

//             var totalAfterBigPurchase = finalAllocation.Values.Sum();
//             if (Math.Abs(totalAfterBigPurchase - AppConstants.Thresholds.TotalAllocation_c) > AppConstants.Thresholds.Tolerance)
//             {
//                 var keyToAdjust = finalAllocation.OrderByDescending(kv => kv.Value).First().Key;
//                 finalAllocation[keyToAdjust] += AppConstants.Thresholds.TotalAllocation_c - totalAfterBigPurchase;
//             }
//         }
//         break;
// }

// var totalAfterGoalTuning = finalAllocation.Values.Sum();
// if (Math.Abs(totalAfterGoalTuning - AppConstants.Thresholds.TotalAllocation_c) > AppConstants.Thresholds.Tolerance)
// {
//     var keyToAdjust = finalAllocation.OrderByDescending(kv => kv.Value).First().Key;
//     finalAllocation[keyToAdjust] += AppConstants.Thresholds.TotalAllocation_c - totalAfterGoalTuning;
// }

// try
// {
//     var total = finalAllocation.Values.Sum();
//     foreach (var key in finalAllocation.Keys.ToList())
//     {
//         finalAllocation[key] = Math.Max(0, finalAllocation[key]);
//     }

//     total = finalAllocation.Values.Sum();
//     if (Math.Abs(total - AppConstants.Thresholds.TotalAllocation_c) > AppConstants.Thresholds.Tolerance)
//     {
//         var adjustmentFactor = AppConstants.Thresholds.TotalAllocation_c / total;

//         foreach (var key in finalAllocation.Keys.ToList())
//         {
//             finalAllocation[key] = Math.Max(0, finalAllocation[key] * adjustmentFactor);
//         }

//         var adjustedTotal = finalAllocation.Values.Sum();
//         if (Math.Abs(adjustedTotal - AppConstants.Thresholds.TotalAllocation_c) > AppConstants.Thresholds.Tolerance)
//         {
//             var keyToAdjust = finalAllocation.OrderByDescending(kv => kv.Value).First().Key;
//             finalAllocation[keyToAdjust] = Math.Max(0, finalAllocation[keyToAdjust] + AppConstants.Thresholds.TotalAllocation_c- adjustedTotal);
//         }
//     }
// }
// catch (Exception ex)
// {
//     _logger.LogError(ex, ResponseMessages.ErrorNormalizingAllocation);
//     throw;
// }

// try
// {
//     var subMatrix = await LoadSubAllocationMatrixAsync();
//     var subAllocations = ComputeSubAllocations(finalAllocation, input.RiskTolerance!, subMatrix);

//     var finalFormattedResult = new Dictionary<string, object>();

//     foreach (var mainAllocationPair in finalAllocation)
//     {
//         var assetClassName = mainAllocationPair.Key;
//         var assetPercentage = mainAllocationPair.Value;

//         if (subAllocations.TryGetValue(assetClassName, out var subAssetAllocations))
//         {
//             finalFormattedResult[assetClassName] = new Dictionary<string, object>
//             {
//                 [AppConstants.Percentage] = Math.Round(assetPercentage, 2),
//                 [AppConstants.SubAssets] = subAssetAllocations
//             };
//             _logger.LogInformation(ResponseMessages.SubAssetsAdded, assetClassName, JsonSerializer.Serialize(subAllocations[assetClassName]));
//         }
//         else
//         {
//             finalFormattedResult[assetClassName] = new Dictionary<string, object>
//             {
//                 [AppConstants.Percentage] = Math.Round(assetPercentage, 2),
//                 [AppConstants.SubAssets] = new Dictionary<string, double>()
//             };
//             _logger.LogWarning(ResponseMessages.NoSubAssetsFound, assetClassName);
//         }
//     }

//     _logger.LogInformation(ResponseMessages.FinalFormattedResult, JsonSerializer.Serialize(finalFormattedResult));
//     var result = new Dictionary<string, object> { [AppConstants.Assets] = finalFormattedResult };

//     await SaveFinalAllocationToFileAsync(result);
//     _logger.LogInformation(ResponseMessages.FinalFormattedResult, JsonSerializer.Serialize(finalFormattedResult));
//     return result;
// }
// catch (Exception ex)
// {
//     _logger.LogError(ex, ResponseMessages.ErrorComputingSubAllocations);
//     throw;
// }

// catch (Exception ex)
// {
//     _logger.LogError(ex, ResponseMessages.ErrorDuringAllocation);
//     throw;
// }

//         private string GetAgeGroup(int age)
//         {
//             if (age < 30) return AppConstants.UnderThirty;
//             if (age <= 45) return AppConstants.ThirtyToFortyFive;
//             if (age <= 60) return AppConstants.FortyFiveToSixty;
//             return AppConstants.AboveSixty;
//         }

//         private string GetHorizonGroup(int investmentHorizon)
//         {
//             if (investmentHorizon < 0)
//                 throw new ArgumentException(ResponseMessages.InvalidHorizon);

//             if (investmentHorizon < 4) return AppConstants.Short;
//             if (investmentHorizon < 7) return AppConstants.Moderate;
//             return AppConstants.Long;
//         }

//         private double GetDoubleFromObject(object obj)
//         {
//             if (obj is JsonElement jsonElement)
//             {
//                 if (jsonElement.ValueKind == JsonValueKind.Number)
//                 {
//                     return jsonElement.GetDouble();
//                 }
//                 throw new InvalidCastException(string.Format(ResponseMessages.InvalidJsonNumber, jsonElement));
//             }

//             if (obj is IConvertible convertible)
//             {
//                 return convertible.ToDouble(null);
//             }

//             throw new InvalidCastException(string.Format(ResponseMessages.InvalidConversion, obj.GetType()));
//         }

//         public async Task<Dictionary<string, object>> GetFinalAllocationFromFileAsync()
//         {
//             try
//             {
//                 var filePath = AppConstants.FinalAllocationFilePath;

//                 _logger.LogInformation(ResponseMessages.ReadingFinalAllocation, filePath);

//                 if (!File.Exists(filePath))
//                 {
//                     _logger.LogWarning(ResponseMessages.FinalAllocationFileNotFound, filePath);
//                     return null;
//                 }

//                 var json = await File.ReadAllTextAsync(filePath);
//                 _logger.LogInformation(ResponseMessages.FinalAllocationReadSuccess);

//                 var finalAllocation = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

//                 if (finalAllocation == null)
//                 {
//                     _logger.LogWarning(ResponseMessages.FinalAllocationDeserializationNull);
//                     return null;
//                 }

//                 return finalAllocation;
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, ResponseMessages.ErrorReadingFinalAllocation);
//                 throw;
//             }
//         }

//         private async Task<SubAllocationMatrix> LoadSubAllocationMatrixAsync()
//         {
//             try
//             {
//                 var filePath = Path.Combine(AppConstants.RepositoriesFolder, AppConstants.MatrixFolder, AppConstants.SubAllocationMatrixFile);

//                 if (!File.Exists(filePath))
//                     throw new FileNotFoundException(ResponseMessages.SubAllocationMatrixNotFound);

//                 var json = await File.ReadAllTextAsync(filePath);
//                 var intMatrix = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, int>>>>(json);

//                 var doubleMatrix = intMatrix.ToDictionary(
//                     outer => outer.Key,
//                     outer => outer.Value.ToDictionary(
//                         middle => middle.Key,
//                         middle => middle.Value.ToDictionary(
//                             inner => inner.Key,
//                             inner => (double)inner.Value
//                         )
//                     )
//                 );

//                 return new SubAllocationMatrix { Matrix = doubleMatrix };
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, ResponseMessages.ErrorLoadingSubMatrix);
//                 throw;
//             }
//         }

//         private Dictionary<string, Dictionary<string, double>> ComputeSubAllocations(
//             Dictionary<string, double> finalAllocations,
//             string riskLevel,
//             SubAllocationMatrix subMatrix)
//         {
//             try
//             {
//                 var subAllocationsResult = new Dictionary<string, Dictionary<string, double>>();
//                 var assetClassMapping = new Dictionary<string, string>
//                 {
//                     { AppConstants.EquityKey, AppConstants.Equity },
//                     { AppConstants.FixedIncomeKey, AppConstants.FixedIncome },
//                     { AppConstants.CommoditiesKey, AppConstants.Commodities },
//                     { AppConstants.CashKey, AppConstants.CashEquivalence },
//                     { AppConstants.RealEstateKey, AppConstants.RealEstate }
//                 };

//                 foreach (var assetClass in finalAllocations)
//                 {
//                     var className = assetClass.Key;
//                     var totalPercentage = assetClass.Value;

//                     if (!assetClassMapping.TryGetValue(className, out var mappedClassName))
//                     {
//                         _logger.LogWarning(ResponseMessages.MissingMappingForAsset, className);
//                         continue;
//                     }

//                     if (!subMatrix.Matrix.ContainsKey(mappedClassName))
//                     {
//                         _logger.LogWarning(ResponseMessages.NoSubRulesForAsset, mappedClassName);
//                         continue;
//                     }

//                     var subcategories = subMatrix.Matrix[mappedClassName];
//                     var weights = new Dictionary<string, int>();

//                     foreach (var sub in subcategories)
//                     {
//                         if (sub.Value.ContainsKey(riskLevel))
//                         {
//                             weights[sub.Key] = (int)sub.Value[riskLevel];
//                         }
//                     }

//                     var totalWeight = weights.Values.Sum();
//                     if (totalWeight == 0)
//                     {
//                         _logger.LogWarning(ResponseMessages.NoWeightsForRiskLevel, riskLevel, className);
//                         continue;
//                     }

//                     var calculatedSubs = weights.ToDictionary(
//                         kv => kv.Key,
//                         kv => Math.Max(0, Math.Round((kv.Value / (double)totalWeight) * totalPercentage, 2))
//                     );
//                     _logger.LogInformation(ResponseMessages.SubAllocationsComputed, className, JsonSerializer.Serialize(calculatedSubs));
//                     subAllocationsResult[className] = calculatedSubs;
//                 }

//                 return subAllocationsResult;
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, ResponseMessages.ErrorComputingSubAllocations);
//                 throw;
//             }
//         }

//         public Dictionary<string, Dictionary<string, double>> TransformAssetsToSubAllocationResult(dynamic assets)
//         {
//             var subAllocationResult = new Dictionary<string, Dictionary<string, double>>();

//             foreach (var assetClass in assets)
//             {
//                 var assetClassName = assetClass.Name;
//                 var subAssets = assetClass.Value.subAssets;

//                 var subAssetAllocations = new Dictionary<string, double>();
//                 foreach (var subAsset in subAssets)
//                 {
//                     var subAssetName = subAsset.Name;
//                     var allocation = (double)subAsset.Value;
//                     subAssetAllocations[subAssetName] = allocation;
//                 }

//                 subAllocationResult[assetClassName] = subAssetAllocations;
//             }

//             return subAllocationResult;
//         }
//     }
// }
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WisVestAPI.Models.DTOs;
using WisVestAPI.Models.Matrix;
using WisVestAPI.Repositories.Matrix;
using WisVestAPI.Services.Interfaces;
 
namespace WisVestAPI.Services
{
    public class AllocationService : IAllocationService
    {
        private readonly MatrixRepository _matrixRepository;
        private readonly ILogger<AllocationService> _logger;

        public AllocationService(MatrixRepository matrixRepository, ILogger<AllocationService> logger)
        {
            _matrixRepository = matrixRepository;
            _logger = logger;
        }
 
        private const string CashKey = "cash";
        private const string EquityKey = "equity";
        private const string FixedIncomeKey = "fixedIncome";
        private const string CommoditiesKey = "commodities";
        private const string RealEstateKey = "realEstate";


        private async Task SaveFinalAllocationToFileAsync(Dictionary<string, object> finalAllocation)
        
{
    try
    {
        var filePath = "FinalAllocation.json"; // Specify the file path
        var options = new JsonSerializerOptions
        {
            WriteIndented = true // Makes the JSON output more readable
        };
        string jsonString = JsonSerializer.Serialize(finalAllocation, options);

        // Write the JSON string to the specified file (overwrites the file if it exists)
        await File.WriteAllTextAsync(filePath, jsonString);

        _logger.LogInformation("Final allocation saved successfully to {FilePath}", filePath);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error occurred while saving final allocation to file.");
        throw;
    }
}
        public async Task<Dictionary<string, object>> CalculateFinalAllocation(UserInputDTO input)
        {  
             try{
                //Console.WriteLine("Starting allocation calculation...");
                _logger.LogInformation("Starting allocation calculation...");
                // Load the allocation matrix
                var allocationMatrix = await _matrixRepository.LoadMatrixDataAsync();
                if (allocationMatrix == null)
                {
                    //Console.WriteLine("Error: Allocation matrix is null.");
                    _logger.LogError("Allocation matrix is null.");
                    throw new InvalidOperationException("Allocation matrix data is null.");
                }
                //Console.WriteLine("Allocation matrix loaded successfully.");
                _logger.LogInformation("Allocation matrix loaded successfully.");
    
                // Step 1: Map input values to match JSON keys
                var riskToleranceMap = new Dictionary<string, string>
                {
                    { "Low", "Low" },
                    { "Medium", "Mid" },
                    { "High", "High" }
                };
    
                var investmentHorizonMap = new Dictionary<string, string>
                {
                    { "short", "Short" },
                    { "moderate", "Mod" },
                    { "long", "Long" }
                };
if (input.InvestmentHorizon <= 0)
    throw new ArgumentException("InvestmentHorizon is required and must be greater than zero.");

var riskToleranceKey = riskToleranceMap[input.RiskTolerance ?? throw new ArgumentException("RiskTolerance is required")];

// Convert the integer investmentHorizon to its corresponding string value
var horizonGroup = GetHorizonGroup(input.InvestmentHorizon);

var investmentHorizonKey = investmentHorizonMap[horizonGroup];
var riskHorizonKey = $"{riskToleranceKey}+{investmentHorizonKey}";
    
                //Console.WriteLine($"Looking up base allocation for key: {riskHorizonKey}");
                _logger.LogInformation($"Looking up base allocation for key: {riskHorizonKey}");
    
                // Step 2: Determine base allocation
                if (!allocationMatrix.Risk_Horizon_Allocation.TryGetValue(riskHorizonKey, out var baseAllocation))
                {
                    //Console.WriteLine($"Error: No base allocation found for key: {riskHorizonKey}");
                    _logger.LogError("No base allocation found for key: {RiskHorizonKey}", riskHorizonKey);
                    throw new ArgumentException($"Invalid combination of RiskTolerance and InvestmentHorizon: {riskHorizonKey}");
                }
                _logger.LogInformation($"Base allocation found: {JsonSerializer.Serialize(baseAllocation)}");
    
                // Clone the base allocation to avoid modifying the original matrix
                var finalAllocation = new Dictionary<string, double>(baseAllocation);
                try{
                // Step 3: Apply age adjustment rules
                    var ageRuleKey = GetAgeGroup(input.Age);
                    //Console.WriteLine($"Looking up age adjustment rules for key: {ageRuleKey}");
                    _logger.LogInformation("Looking up age adjustment rules for key: {AgeRuleKey}", ageRuleKey);

        
                    if (allocationMatrix.Age_Adjustment_Rules.TryGetValue(ageRuleKey, out var ageAdjustments))
                    {
                        _logger.LogInformation("Age adjustments found: {AgeAdjustments}", JsonSerializer.Serialize(ageAdjustments));
                        //Console.WriteLine($"Age adjustments found: {JsonSerializer.Serialize(ageAdjustments)}");
                        foreach (var adjustment in ageAdjustments)
                        {
                            if (finalAllocation.ContainsKey(adjustment.Key))
                            {
                                finalAllocation[adjustment.Key] += adjustment.Value;
                            }
                        }
                    }
                    else
                    {
                         _logger.LogWarning("No age adjustments found for key: {AgeRuleKey}", ageRuleKey);
                        //Console.WriteLine($"No age adjustments found for key: {ageRuleKey}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error applying age adjustments: {Message}", ex.Message);
                    //Console.WriteLine($"Error applying age adjustments: {ex.Message}");
                    throw; // Re-throw the exception to propagate it to the caller
                }
    
                // Step 4: Apply goal tuning
                try{
                    _logger.LogInformation("Looking up goal tuning for goal: {Goal}", input.Goal);
                    //Console.WriteLine($"Looking up goal tuning for goal: {input.Goal}");
                    if (string.IsNullOrEmpty(input.Goal))
                    {
                        throw new ArgumentException("Goal is required.");
                    }
                    if (allocationMatrix.Goal_Tuning.TryGetValue(input.Goal, out var goalTuning))
                    {
                         _logger.LogInformation("Goal tuning found: {GoalTuning}", JsonSerializer.Serialize(goalTuning));
                        //Console.WriteLine($"Goal tuning found: {JsonSerializer.Serialize(goalTuning)}");
        
                        switch (input.Goal)
                        {
                            // case "Emergency Fund":
                            //     if (finalAllocation.ContainsKey(CashKey) && finalAllocation[CashKey] < 40)
                            //     {
                            //         var cashDeficit = 40 - finalAllocation[CashKey];
                            //         finalAllocation[CashKey] += cashDeficit;
        
                            //         var categoriesToReduce = new[] { EquityKey, FixedIncomeKey, CommoditiesKey, RealEstateKey };
                            //         var reductionPerCategory = cashDeficit / categoriesToReduce.Length;
                            //         foreach (var category in categoriesToReduce)
                            //         {
                            //             if (finalAllocation.ContainsKey(category))
                            //             {
                            //                 finalAllocation[category] -= reductionPerCategory;
                            //             }
                            //         }
                            //     }
                            //     break;

                            case "Emergency Fund":
                                if (finalAllocation.ContainsKey(CashKey) && finalAllocation[CashKey] < 40)
                                {
                                    var cashDeficit = 40 - finalAllocation[CashKey];
                                    finalAllocation[CashKey] += cashDeficit;
                            
                                    var categoriesToReduce = new[] { EquityKey, FixedIncomeKey, CommoditiesKey, RealEstateKey };
                                    var totalReduction = 0.0;
                            
                                    // Calculate the total reduction that can be applied without going negative
                                    foreach (var category in categoriesToReduce)
                                    {
                                        if (finalAllocation.ContainsKey(category))
                                        {
                                            totalReduction += Math.Max(0, finalAllocation[category]);
                                        }
                                    }
                            
                                    // Adjust the cash deficit if the total reduction is less than the required deficit
                                    if (totalReduction < cashDeficit)
                                    {
                                        cashDeficit = totalReduction;
                                        finalAllocation[CashKey] += cashDeficit; // Adjust cash allocation to the maximum possible
                                    }
                            
                                    // Distribute the reduction proportionally across categories
                                    foreach (var category in categoriesToReduce)
                                    {
                                        if (finalAllocation.ContainsKey(category) && finalAllocation[category] > 0)
                                        {
                                            var reduction = Math.Min(finalAllocation[category], (cashDeficit / totalReduction) * finalAllocation[category]);
                                            // finalAllocation[category] -= reduction;
                                            finalAllocation[category] = Math.Max(0, finalAllocation[category] - reduction); // Ensure no negative values
                                        }
                                    }
                                }
                                break;
        
                            // case "Retirement":
                            //     if (goalTuning.TryGetValue("fixedIncome_boost", out var fixedIncomeBoost) && finalAllocation.ContainsKey(FixedIncomeKey))
                            //     {
                            //         finalAllocation[FixedIncomeKey] += GetDoubleFromObject(fixedIncomeBoost);
                            //     }
                            //     if (goalTuning.TryGetValue("realEstate_boost", out var realEstateBoost) && finalAllocation.ContainsKey(RealEstateKey))
                            //     {
                            //         finalAllocation[RealEstateKey] += GetDoubleFromObject(realEstateBoost);
                            //     }
                            //     break;
                            case "Retirement":
                                if (goalTuning.TryGetValue("fixedIncome_boost", out var fixedIncomeBoost) && finalAllocation.ContainsKey(FixedIncomeKey))
                                {
                                    // Ensure no negative values for fixedIncome
                                    finalAllocation[FixedIncomeKey] = Math.Max(0, finalAllocation[FixedIncomeKey] + GetDoubleFromObject(fixedIncomeBoost));
                                }
                                if (goalTuning.TryGetValue("realEstate_boost", out var realEstateBoost) && finalAllocation.ContainsKey(RealEstateKey))
                                {
                                    // Ensure no negative values for realEstate
                                    finalAllocation[RealEstateKey] = Math.Max(0, finalAllocation[RealEstateKey] + GetDoubleFromObject(realEstateBoost));
                                }
                                break;
        
                            // case "Wealth Accumulation":
                            //     if (finalAllocation.ContainsKey(EquityKey) && finalAllocation.Values.Any() && finalAllocation[EquityKey] < finalAllocation.Values.Max())
                            //     {
                            //         finalAllocation[EquityKey] += 10;
                            //         var sumAfterEquityBoost = finalAllocation.Values.Sum();
                            //         var remainingAdjustment = 100 - sumAfterEquityBoost;
                            //         var otherKeys = finalAllocation.Keys.Where(k => k != EquityKey).ToList();
                            //         if (otherKeys.Any())
                            //         {
                            //             foreach (var key in otherKeys)
                            //             {
                            //                 finalAllocation[key] += remainingAdjustment / otherKeys.Count();
                            //             }
                            //         }
                            //     }
                            //     break;

                                                        case "Wealth Accumulation":
                                if (finalAllocation.ContainsKey(EquityKey) && finalAllocation.Values.Any() && finalAllocation[EquityKey] < finalAllocation.Values.Max())
                                {
                                    finalAllocation[EquityKey] += 10;
                            
                                    var sumAfterEquityBoost = finalAllocation.Values.Sum();
                                    var remainingAdjustment = 100 - sumAfterEquityBoost;
                            
                                    var otherKeys = finalAllocation.Keys.Where(k => k != EquityKey).ToList();
                                    if (otherKeys.Any())
                                    {
                                        foreach (var key in otherKeys)
                                        {
                                            if (finalAllocation.ContainsKey(key))
                                            {
                                                // Ensure no negative values
                                                finalAllocation[key] = Math.Max(0, finalAllocation[key] + (remainingAdjustment / otherKeys.Count()));
                                            }
                                        }
                                    }
                            
                                    // Recalculate the total and normalize if necessary
                                    var totalAfterAdjustment = finalAllocation.Values.Sum();
                                    if (Math.Abs(totalAfterAdjustment - 100) > 0.01)
                                    {
                                        var keyToAdjust = finalAllocation.OrderByDescending(kv => kv.Value).First().Key;
                                        finalAllocation[keyToAdjust] += 100 - totalAfterAdjustment;
                                    }
                                }
                                break;
        
                            // case "Child Education":
                            //     if (goalTuning.TryGetValue("fixedIncome_boost", out var fixedIncomeBoostChild) && finalAllocation.ContainsKey(FixedIncomeKey))
                            //     {
                            //         finalAllocation[FixedIncomeKey] += GetDoubleFromObject(fixedIncomeBoostChild);
                            //     }
                            //     if (goalTuning.TryGetValue("equityReduction_moderate", out var equityReduction) && finalAllocation.ContainsKey(EquityKey))
                            //     {
                            //         finalAllocation[EquityKey] -= GetDoubleFromObject(equityReduction);
                            //     }
                            //     break;

                                                        case "Child Education":
                                if (goalTuning.TryGetValue("fixedIncome_boost", out var fixedIncomeBoostChild) && finalAllocation.ContainsKey(FixedIncomeKey))
                                {
                                    finalAllocation[FixedIncomeKey] += GetDoubleFromObject(fixedIncomeBoostChild);
                                }
                                if (goalTuning.TryGetValue("equityReduction_moderate", out var equityReduction) && finalAllocation.ContainsKey(EquityKey))
                                {
                                    var reductionAmount = GetDoubleFromObject(equityReduction);
                                    finalAllocation[EquityKey] = Math.Max(0, finalAllocation[EquityKey] - reductionAmount);
                                }
                                break;
        
                            case "Big Purchase":
                                if (goalTuning.TryGetValue("balanced", out var balancedObj) &&
                                    bool.TryParse(balancedObj.ToString(), out var balanced) && balanced)
                                {
                                    Console.WriteLine("Big Purchase goal tuning: balancing enabled.");
        
                                    double threshold = 30.0;
                                    var keys = finalAllocation.Keys.ToList();
                                    double totalExcess = 0.0;
        
                                    foreach (var assetKey in keys)
                                    {
                                        if (finalAllocation[assetKey] > threshold)
                                        {
                                            double excess = finalAllocation[assetKey] - threshold;
                                            totalExcess += excess;
                                            finalAllocation[assetKey] = threshold;
                                            Console.WriteLine($"{assetKey} capped at {threshold}, excess {excess}% collected.");
                                        }
                                    }
        
                                    var underThresholdKeys = keys.Where(k => finalAllocation[k] < threshold).ToList();
                                    int count = underThresholdKeys.Count();
        
                                    if (count > 0 && totalExcess > 0)
                                    {
                                        double share = totalExcess / count;
                                        foreach (var key in underThresholdKeys)
                                        {
                                            finalAllocation[key] += share;
                                            Console.WriteLine($"{share}% added to {key}. New value: {finalAllocation[key]}%");
                                        }
                                    }
        
                                    // Normalize after potential balancing
                                    var totalAfterBigPurchase = finalAllocation.Values.Sum();
                                    if (Math.Abs(totalAfterBigPurchase - 100) > 0.01)
                                    {
                                        var keyToAdjust = finalAllocation.OrderByDescending(kv => kv.Value).First().Key;
                                        finalAllocation[keyToAdjust] += 100 - totalAfterBigPurchase;
                                    }
                                }
                                break;
                        }
        
                        // Normalize after goal tuning adjustments
                        var totalAfterGoalTuning = finalAllocation.Values.Sum();
                        if (Math.Abs(totalAfterGoalTuning - 100) > 0.01)
                        {
                            var keyToAdjust = finalAllocation.OrderByDescending(kv => kv.Value).First().Key;
                            finalAllocation[keyToAdjust] += 100 - totalAfterGoalTuning;
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No goal tuning found for goal: {Goal}", input.Goal);
                       // Console.WriteLine($"No goal tuning found for goal: {input.Goal}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error applying goal tuning.");
                    //Console.WriteLine($"Error applying goal tuning: {ex.Message}");
                    throw; // Re-throw the exception to propagate it to the caller
                }
    
                // Step 5: Normalize allocation to ensure it adds up to 100%
                // try{
                //     var total = finalAllocation.Values.Sum();
                //     if (Math.Abs(total - 100) > 0.01)
                //     {
                //         var adjustmentFactor = 100 / total;
                //         foreach (var key in finalAllocation.Keys.ToList())
                //         {
                //             finalAllocation[key] *= adjustmentFactor;
                //         }
                //     }
                // }
                // catch (Exception ex)
                // {
                //     _logger.LogError(ex, "Error normalizing allocation.");
                //     //Console.WriteLine($"Error normalizing allocation: {ex.Message}");
                //     throw; // Re-throw the exception to propagate it to the caller
                // }
        try
    {
        var total = finalAllocation.Values.Sum();
        foreach (var key in finalAllocation.Keys.ToList())
{
    finalAllocation[key] = Math.Max(0, finalAllocation[key]);
}

// Recalculate total after removing negative values
total = finalAllocation.Values.Sum();
        if (Math.Abs(total - 100) > 0.01)
        {
            var adjustmentFactor = 100 / total;
    
            foreach (var key in finalAllocation.Keys.ToList())
            {
                finalAllocation[key] = Math.Max(0, finalAllocation[key] * adjustmentFactor);
            }
    
            // Recalculate the total after adjustment
            var adjustedTotal = finalAllocation.Values.Sum();
    
            // If the total is still not 100%, adjust the largest allocation to make up the difference
            if (Math.Abs(adjustedTotal - 100) > 0.01)
            {
                var keyToAdjust = finalAllocation.OrderByDescending(kv => kv.Value).First().Key;
                finalAllocation[keyToAdjust] = Math.Max(0, finalAllocation[keyToAdjust] + 100 - adjustedTotal);
            }
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error normalizing allocation.");
        throw; // Re-throw the exception to propagate it to the caller
    }
                // Step 6: Compute and add sub-allocations
                try{
                    var subMatrix = await LoadSubAllocationMatrixAsync();
                    var subAllocations = ComputeSubAllocations(finalAllocation, input.RiskTolerance!, subMatrix);
        
                    // Step 7: Format the final result according to the expected structure
                    var finalFormattedResult = new Dictionary<string, object>();
        
                    foreach (var mainAllocationPair in finalAllocation)
                    {
                        var assetClassName = mainAllocationPair.Key;
                        var assetPercentage = mainAllocationPair.Value;
        
                        if (subAllocations.TryGetValue(assetClassName, out var subAssetAllocations))
                        {
                            finalFormattedResult[assetClassName] = new Dictionary<string, object>
                            {
                                ["percentage"] = Math.Round(assetPercentage, 2),
                                ["subAssets"] = subAssetAllocations
                            };
                           // Console.WriteLine($"Added sub-assets for {assetClassName}: {JsonSerializer.Serialize(subAllocations[assetClassName])}");
                           _logger.LogInformation("Added sub-assets for {AssetClassName}: {SubAssets}", assetClassName, JsonSerializer.Serialize(subAllocations[assetClassName]));
                        }
                        else
                        {
                            finalFormattedResult[assetClassName] = new Dictionary<string, object>
                            {
                                ["percentage"] = Math.Round(assetPercentage, 2),
                                ["subAssets"] = new Dictionary<string, double>() // Empty if no sub-allocations
                            };

                            //Console.WriteLine($"No sub-assets for {assetClassName}. Added empty sub-assets.");
                            _logger.LogWarning("No sub-assets for {AssetClassName}. Added empty sub-assets.", assetClassName);
                        }
                    }
                    _logger.LogInformation("Final formatted allocation: {FinalFormattedResult}", JsonSerializer.Serialize(finalFormattedResult));
                    //Console.WriteLine($"Final formatted allocation: {JsonSerializer.Serialize(finalFormattedResult)}");
                    var result = new Dictionary<string, object> { ["assets"] = finalFormattedResult };

        // Save the result to a JSON file
                     await SaveFinalAllocationToFileAsync(result);

                     _logger.LogInformation("Final formatted allocation: {FinalFormattedResult}", JsonSerializer.Serialize(finalFormattedResult));
                     return result;
                }
                catch (Exception ex)
                {
                     _logger.LogError(ex, "Error computing sub-allocations.");
                    //Console.WriteLine($"Error computing sub-allocations: {ex.Message}");
                    throw; // Re-throw the exception to propagate it to the caller
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error during allocation calculation.");
                //Console.WriteLine($"Error during allocation calculation: {ex.Message}");
                throw; // Re-throw the exception to propagate it to the caller
            }
        }
        
 
        private string GetAgeGroup(int age)
        {
            if (age < 30) return "<30";
            if (age <= 45) return "30-45";
            if (age <= 60) return "45-60";
            return "60+";
        }

        private string GetHorizonGroup(int investmentHorizon)
        {
            if (investmentHorizon < 0)
                throw new ArgumentException("Investment horizon cannot be negative.");
        
            if (investmentHorizon < 4) return "short";
            if (investmentHorizon < 7) return "moderate";
            return "long";
        }
 
        private double GetDoubleFromObject(object obj)
        {
            if (obj is JsonElement jsonElement)
            {
                if (jsonElement.ValueKind == JsonValueKind.Number)
                {
                    return jsonElement.GetDouble();
                }
                throw new InvalidCastException($"JsonElement is not a number: {jsonElement}");
            }
 
            if (obj is IConvertible convertible)
            {
                return convertible.ToDouble(null);
            }
 
            throw new InvalidCastException($"Unable to convert object of type {obj.GetType()} to double.");
        }

                        // filepath: c:\Users\divya.t1\Desktop\7-5-25-WisVest-Backend-main\Services\AllocationService.cs
                public async Task<Dictionary<string, object>> GetFinalAllocationFromFileAsync()
                {
                    try
                    {
                        var filePath = "FinalAllocation.json"; // Path to the saved JSON file
                
                        _logger.LogInformation("Attempting to read final allocation from JSON file: {FilePath}", filePath);
                
                        if (!File.Exists(filePath))
                        {
                            _logger.LogWarning("Final allocation file not found at {FilePath}. Returning null.", filePath);
                            return null;
                        }
                
                        var json = await File.ReadAllTextAsync(filePath);
                        _logger.LogInformation("Final allocation JSON read successfully.");
                
                        var finalAllocation = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                
                        if (finalAllocation == null)
                        {
                            _logger.LogWarning("Deserialized final allocation is null. Returning null.");
                            return null;
                        }
                
                        return finalAllocation;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred while reading final allocation from file.");
                        throw;
                    }
                }
 
        private async Task<SubAllocationMatrix> LoadSubAllocationMatrixAsync()
        {
            try{
            var filePath = Path.Combine("Repositories", "Matrix", "SubAllocationMatrix.json");

            if (!File.Exists(filePath))
                throw new FileNotFoundException("SubAllocationMatrix.json not found.");

            var json = await File.ReadAllTextAsync(filePath);
            var intMatrix = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, int>>>>(json);

            var doubleMatrix = intMatrix.ToDictionary(
                outer => outer.Key,
                outer => outer.Value.ToDictionary(
                    middle => middle.Key,
                    middle => middle.Value.ToDictionary(
                        inner => inner.Key,
                        inner => (double)inner.Value
                    )
                )
            );

            return new SubAllocationMatrix { Matrix = doubleMatrix };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading sub-allocation matrix.");
                //Console.WriteLine($"Error loading sub-allocation matrix: {ex.Message}");
                throw; // Re-throw the exception to propagate it to the caller
            }
        }

        
        private Dictionary<string, Dictionary<string, double>> ComputeSubAllocations(
    Dictionary<string, double> finalAllocations,
    string riskLevel,
    SubAllocationMatrix subMatrix)
    {
        try{
        var subAllocationsResult = new Dictionary<string, Dictionary<string, double>>();
        var assetClassMapping = new Dictionary<string, string>
        {
            { "equity", "Equity" },
            { "fixedIncome", "Fixed Income" },
            { "commodities", "Commodities" },
            { "cash", "Cash Equivalence" },
            { "realEstate", "Real Estate" }
        };

        foreach (var assetClass in finalAllocations)
        {
            var className = assetClass.Key;
            var totalPercentage = assetClass.Value;

            if (!assetClassMapping.TryGetValue(className, out var mappedClassName))
            {
                _logger.LogWarning("No mapping found for asset class: {ClassName}", className);
                //Console.WriteLine($"No mapping found for asset class: {className}");
                continue;
            }

            if (!subMatrix.Matrix.ContainsKey(mappedClassName))
            {
                _logger.LogWarning("No sub-allocation rules found for asset class: {MappedClassName}", mappedClassName);
                //Console.WriteLine($"No sub-allocation rules found for asset class: {mappedClassName}");
                continue; // Skip if no suballocation rules for this asset class
            }

            var subcategories = subMatrix.Matrix[mappedClassName];
            var weights = new Dictionary<string, int>();

            // Collect weights for this risk level
            foreach (var sub in subcategories)
            {
                if (sub.Value.ContainsKey(riskLevel))
                {
                    weights[sub.Key] = (int)sub.Value[riskLevel];
                }
            }

            var totalWeight = weights.Values.Sum();
            if (totalWeight == 0)
            {
                _logger.LogWarning("No weights found for risk level '{RiskLevel}' in asset class '{ClassName}'", riskLevel, className);
                //Console.WriteLine($"No weights found for risk level '{riskLevel}' in asset class '{className}'");
                continue;
            }

            // Calculate suballocation % based on weight
            var calculatedSubs = weights.ToDictionary(
                kv => kv.Key,
                // kv => Math.Round((kv.Value / (double)totalWeight) * totalPercentage, 2)
                kv =>
    {
        var subAllocation = Math.Round((kv.Value / (double)totalWeight) * totalPercentage, 2);
        return Math.Max(0, subAllocation); // Ensure no negative sub-allocations
    }
            );
            _logger.LogInformation("Sub-allocations for {ClassName}: {CalculatedSubs}", className, JsonSerializer.Serialize(calculatedSubs));
            //Console.WriteLine($"Sub-allocations for {className}: {JsonSerializer.Serialize(calculatedSubs)}");
            subAllocationsResult[className] = calculatedSubs;
        }

        return subAllocationsResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error computing sub-allocations.");
            //Console.WriteLine($"Error computing sub-allocations: {ex.Message}");
            throw; // Re-throw the exception to propagate it to the caller
        }
}

public Dictionary<string, Dictionary<string, double>> TransformAssetsToSubAllocationResult(dynamic assets)
{
    var subAllocationResult = new Dictionary<string, Dictionary<string, double>>();

    foreach (var assetClass in assets)
    {
        var assetClassName = assetClass.Name; // e.g., "equity", "fixedIncome"
        var subAssets = assetClass.Value.subAssets;

        var subAssetAllocations = new Dictionary<string, double>();
        foreach (var subAsset in subAssets)
        {
            var subAssetName = subAsset.Name; // e.g., "Large Cap", "Gov Bonds"
            var allocation = (double)subAsset.Value; // Allocation percentage
            subAssetAllocations[subAssetName] = allocation;
        }

        subAllocationResult[assetClassName] = subAssetAllocations;
    }

    return subAllocationResult;
}
    }
}

