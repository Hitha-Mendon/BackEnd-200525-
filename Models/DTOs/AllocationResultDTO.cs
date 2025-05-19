
using System.Collections.Generic;
 
namespace WisVestAPI.Models.DTOs
{
    public class AllocationResultDTO
    {
        public Dictionary<string, AssetAllocation> Assets { get; set; } = new Dictionary<string, AssetAllocation>();
    }
 
    public class AssetAllocation
    {
        public double Percentage { get; set; }
        public Dictionary<string, double> SubAssets { get; set; } = new Dictionary<string, double>();
    }
}