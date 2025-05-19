using WisVestAPI.Models.DTOs;

namespace WisVestAPI.Services.Interfaces
{
    public interface IUserInputService
    {
        Task<AllocationResultDTO> HandleUserInput(UserInputDTO input);
    }
}
