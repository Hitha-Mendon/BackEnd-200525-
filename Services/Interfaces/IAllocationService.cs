using System.Collections.Generic;
using System.Threading.Tasks;
using WisVestAPI.Models.DTOs;

namespace WisVestAPI.Services.Interfaces
{
    public interface IAllocationService
    {
        Task<Dictionary<string, object>> CalculateFinalAllocation(UserInputDTO input);

        // Add the missing method definition
        Task<Dictionary<string, object>> GetFinalAllocationFromFileAsync();
    }
}