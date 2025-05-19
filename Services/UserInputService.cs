using WisVestAPI.Services.Interfaces;
using WisVestAPI.Models.DTOs;
using System.Text.Json;
using WisVestAPI.Constants;

namespace WisVestAPI.Services
{
    public class UserInputService : IUserInputService
    {
        private readonly IAllocationService _allocationService;
        private readonly ILogger<UserInputService> _logger;

           public UserInputService(IAllocationService allocationService, ILogger<UserInputService> logger)
    {
        _allocationService = allocationService;
        _logger = logger;
    }

    public async Task<AllocationResultDTO> HandleUserInput(UserInputDTO input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input), ResponseMessages.InputCannotBeNull);

        _logger.LogInformation($"Received input: {JsonSerializer.Serialize(input)}");

        Dictionary<string, Dictionary<string, double>> allocationDictionary;

        try
        {
            allocationDictionary = (await _allocationService.CalculateFinalAllocation(input).ConfigureAwait(false))
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value as Dictionary<string, double> 
                           ?? throw new InvalidCastException(string.Format(ResponseMessages.InvalidDictionaryCast, kvp.Key))
                );

            if (allocationDictionary == null)
                throw new InvalidOperationException(ResponseMessages.AllocationCalculationFailed);
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ResponseMessages.AllocationCalculationError}: {ex.Message}");
            throw new InvalidOperationException(ResponseMessages.AllocationCalculationError, ex);
        }

        var result = new AllocationResultDTO();

        try
        {
            foreach (var allocation in allocationDictionary)
            {
                if (allocation.Value is Dictionary<string, double> subAssets)
                {
                    result.Assets[allocation.Key] = new AssetAllocation
                    {
                        Percentage = subAssets.Values.Sum(),
                        SubAssets = subAssets
                    };
                }
                else
                {
                    throw new InvalidCastException(string.Format(ResponseMessages.InvalidDictionaryCast, allocation.Key));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ResponseMessages.AllocationProcessingError}: {ex.Message}");
            throw new InvalidOperationException(ResponseMessages.AllocationProcessingError, ex);
        }

        try
        {
            _logger.LogInformation($"Calculated allocation: {JsonSerializer.Serialize(result)}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ResponseMessages.SerializationError}: {ex.Message}");
            throw new InvalidOperationException(ResponseMessages.SerializationError, ex);
        }

        return result;
    }
}

}