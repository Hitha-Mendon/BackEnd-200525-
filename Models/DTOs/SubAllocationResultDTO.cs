public class SubAllocationResultDTO
{
    // The root "assets" dictionary that contains each asset class
    public Dictionary<string, AssetDTO> Assets { get; set; }

    // Method to convert to Dictionary<string, Dictionary<string, double>>
    public Dictionary<string, Dictionary<string, double>> ToDictionary()
    {
        var result = new Dictionary<string, Dictionary<string, double>>();

        // Loop through assets to extract subAssets and their percentages
        foreach (var assetClass in Assets)
        {
            var subAllocations = assetClass.Value.SubAssets.ToDictionary(
                subAsset => subAsset.Key, 
                subAsset => subAsset.Value
            );
            
            result[assetClass.Key] = subAllocations;
        }

        return result;
    }
}

public class AssetDTO
{
    // Percentage for the asset class
    public double Percentage { get; set; }

    // Sub-assets and their allocations (as a dictionary)
    public Dictionary<string, double> SubAssets { get; set; }
}
