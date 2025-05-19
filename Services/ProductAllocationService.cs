using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WisVestAPI.Configuration;
using WisVestAPI.Constants;
using WisVestAPI.Models.DTOs;
using WisVestAPI.Models.Matrix;

namespace WisVestAPI.Services
{
    public class ProductAllocationService
    {
        private readonly string _productJsonFilePath;
        private readonly string _outputJsonFilePath;
        private readonly InvestmentAmountService _investmentAmountService;
        private readonly ILogger<ProductAllocationService> _logger;

        public ProductAllocationService(
            ILogger<ProductAllocationService> logger,
            InvestmentAmountService investmentAmountService,
            IOptions<AppSettings> options)
        {
            _logger = logger;
            _investmentAmountService = investmentAmountService;
            _productJsonFilePath = options.Value.ProductJsonFilePath ?? throw new ArgumentNullException(nameof(options.Value.ProductJsonFilePath));
            _outputJsonFilePath = options.Value.OutputJsonFilePath ?? throw new ArgumentNullException(nameof(options.Value.OutputJsonFilePath));
        }

        public async Task<Dictionary<string, Dictionary<string, Dictionary<string, Product>>>> CalculateProductAllocations(
            Dictionary<string, Dictionary<string, double>> subAllocationResult,
            double targetAmount,
            int investmentHorizon)
        {
            try
            {
                _logger.LogInformation(ResponseMessages.StartingAllocation);
                var productData = await LoadProductDataAsync();
                var productAllocations = new Dictionary<string, Dictionary<string, Dictionary<string, Product>>>();

                foreach (var assetClass in subAllocationResult)
                {
                    foreach (var subAsset in assetClass.Value)
                    {
                        var percentageSplit = subAsset.Value;
                        // Skip sub-assets with zero percentage split
        if (percentageSplit == 0)
        {
            _logger.LogInformation("Skipping sub-asset {SubAsset} in asset class {AssetClass} due to zero percentage split.",
                subAsset.Key, assetClass.Key);
            continue;
        }
                        var products = GetProductsForAssetClass(productData, assetClass.Key, subAsset.Key);

                        if (products is null || products.Count == 0)
                        {
                            _logger.LogWarning(ResponseMessages.NoProductsFound, assetClass.Key, subAsset.Key);
                            continue;
                        }

                        var totalReturns = products.Sum(p => p.AnnualReturn);
                        if (totalReturns <= 0)
                        {
                            _logger.LogWarning(ResponseMessages.TotalReturnsZero, subAsset.Key);
                            continue;
                        }

                        var productSplit = new Dictionary<string, Product>();
                        foreach (var product in products)
                        {
                            var splitRatio = product.AnnualReturn / totalReturns;
                            var allocation = Math.Round(splitRatio * percentageSplit, 2);
                            product.PercentageSplit = allocation;

                            product.InvestmentAmount = _investmentAmountService.CalculateInvestmentAmount(
                                allocation, targetAmount, product.AnnualReturn, investmentHorizon);

                            productSplit[product.ProductName] = product;
                        }

                        if (!productAllocations.ContainsKey(assetClass.Key))
                            productAllocations[assetClass.Key] = new();

                        productAllocations[assetClass.Key][subAsset.Key] = productSplit;
                    }
                }

                await SaveProductAllocationsToFileAsync(productAllocations);
                return productAllocations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResponseMessages.AllocationFailure);
                throw new ApplicationException(ResponseMessages.AllocationFailure, ex);
            }
        }

        private async Task<Dictionary<string, Dictionary<string, List<Product>>>> LoadProductDataAsync()
        {
            try
            {
                if (!File.Exists(_productJsonFilePath))
                    throw new FileNotFoundException(ResponseMessages.ProductFileNotFound, _productJsonFilePath);

                var json = await File.ReadAllTextAsync(_productJsonFilePath);
                var data = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<Product>>>>(json);

                if (data == null)
                    throw new InvalidOperationException(message: ResponseMessages.ProductDataNull);

                return data.ToDictionary(
                    ac => NormalizeKey(ac.Key),
                    ac => ac.Value.ToDictionary(
                        sac => sac.Key.Trim(),
                        sac => sac.Value
                    )
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResponseMessages.LoadProductFailure);
                throw new ApplicationException(ResponseMessages.LoadProductFailure, ex);
            }
        }

        private List<Product> GetProductsForAssetClass(
            Dictionary<string, Dictionary<string, List<Product>>> productData,
            string assetClass,
            string subAssetClass)
        {
            try
            {
                var normalizedAssetClass = NormalizeKey(assetClass);
                var trimmedSubAsset = subAssetClass.Trim();

                if (!productData.TryGetValue(normalizedAssetClass, out var subAssets) ||
                    !subAssets.TryGetValue(trimmedSubAsset, out var products))
                {
                    _logger.LogWarning(ResponseMessages.ProductDataNotFound, assetClass, subAssetClass);
                    return new List<Product>();
                }

                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResponseMessages.ProductFetchError);
                return new List<Product>();
            }
        }

        private async Task SaveProductAllocationsToFileAsync(
            Dictionary<string, Dictionary<string, Dictionary<string, Product>>> productAllocations)
        {
            try
            {
                var json = JsonSerializer.Serialize(productAllocations, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_outputJsonFilePath, json);
                _logger.LogInformation(ResponseMessages.SavingAllocations, _outputJsonFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError( ResponseMessages.SaveAllocationFailure1);
                throw new ApplicationException(ResponseMessages.SaveAllocationFailure1, ex);
            }
        }

        public async Task<(Dictionary<string, Dictionary<string, Dictionary<string, Product>>> ProductAllocations, double TotalInvestment)> GetProductAllocationsAsync()
        {
            try
            {
                if (!File.Exists(_outputJsonFilePath))
                {
                    _logger.LogWarning(ResponseMessages.NoOutputFile, _outputJsonFilePath);
                    return (new Dictionary<string, Dictionary<string, Dictionary<string, Product>>>(), 0);
                }

                var json = await File.ReadAllTextAsync(_outputJsonFilePath);
                var allocations = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, Product>>>>(json)
                                  ?? new Dictionary<string, Dictionary<string, Dictionary<string, Product>>>();

                var totalInvestment = allocations.SelectMany(a => a.Value.Values)
                                                 .SelectMany(p => p.Values)
                                                 .Sum(p => p.InvestmentAmount);

                return (allocations, totalInvestment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResponseMessages.ReadAllocationFailure);
                throw new ApplicationException(ResponseMessages.ReadAllocationFailure, ex);
            }
        }

        private string NormalizeKey(string input)
            => input.Trim().ToLower().Replace(" ", "");
    }
}