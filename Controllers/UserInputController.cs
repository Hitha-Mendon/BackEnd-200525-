using Microsoft.AspNetCore.Mvc;
using WisVestAPI.Models.DTOs;
using WisVestAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using WisVestAPI.Constants;
using System.Linq;
using System.Threading.Tasks;

namespace WisVestAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserInputController : ControllerBase
    {
        private readonly IUserInputService _userInputService;
        private readonly ILogger<UserInputController> _logger;

        public UserInputController(IUserInputService userInputService, ILogger<UserInputController> logger)
        {
            _userInputService = userInputService;
            _logger = logger;
        }

        [HttpPost("submit-input")]
        public async Task<IActionResult> SubmitInput([FromBody] UserInputDTO input)
        {
            if (input == null)
            {
                _logger.LogWarning(ResponseMessages.LogNullInputReceived);
                return BadRequest(new { message = ResponseMessages.NullInput });
            }

            if (input.Age < AppConstants.MinAge || input.Age > AppConstants.MaxAge)
            {
                _logger.LogWarning(ResponseMessages.LogInvalidAge, input.Age);
                return BadRequest(new 
                { 
                    message = input.Age < AppConstants.MinAge ? ResponseMessages.InvalidAgeUnder : ResponseMessages.InvalidAgeOver 
                });
            }

            if (input.InvestmentHorizon < AppConstants.MinInvestmentHorizon || input.InvestmentHorizon > AppConstants.MaxInvestmentHorizon)
            {
                _logger.LogWarning(ResponseMessages.LogInvalidInvestmentHorizon, input.InvestmentHorizon);
                return BadRequest(new
                {
                    message = input.InvestmentHorizon < AppConstants.MinInvestmentHorizon ? ResponseMessages.InvalidInvestmentHorizonUnder : ResponseMessages.InvalidInvestmentHorizonOver
                });
            }

            if (string.IsNullOrEmpty(input.RiskTolerance) || !AppConstants.ValidRiskTolerances.Contains(input.RiskTolerance))
            {
                _logger.LogWarning(ResponseMessages.LogInvalidRiskTolerance, input.RiskTolerance);
                return BadRequest(new { message = ResponseMessages.InvalidRiskTolerance });
            }

            if (input.TargetAmount < AppConstants.MinTargetAmount || input.TargetAmount > AppConstants.MaxTargetAmount)
            {
                _logger.LogWarning(ResponseMessages.LogInvalidTargetAmount, input.TargetAmount);
                return BadRequest(new
                {
                    message = input.TargetAmount < AppConstants.MinTargetAmount ? ResponseMessages.InvalidTargetAmountUnder : ResponseMessages.InvalidTargetAmountOver
                });
            }

            try
            {
                var result = await _userInputService.HandleUserInput(input);
                return Ok(result);
            }
            catch (System.ArgumentException ex)
            {
                _logger.LogWarning(ex, ResponseMessages.LogInvalidInputData);
                return BadRequest(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ResponseMessages.LogInternalServerError);
                return StatusCode(500, new { message = ResponseMessages.InternalServerError, error = ex.Message });
            }
        }
    }
}


// using Microsoft.AspNetCore.Mvc;
// using WisVestAPI.Models.DTOs;
// using WisVestAPI.Services.Interfaces;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.Extensions.Logging;
// using WisVestAPI.Constants;

// namespace WisVestAPI.Controllers
// {
//     [Authorize]
//     [ApiController]
//     [Route("api/[controller]")]
//     public class UserInputController : ControllerBase
//     {
//         private readonly IUserInputService _userInputService;
//         private readonly ILogger<UserInputController> _logger;

//         public UserInputController(IUserInputService userInputService, ILogger<UserInputController> logger)
//         {
//             _userInputService = userInputService;
//             _logger = logger;
//         }

//         /// <summary>
//         /// Handles user input and returns allocation results.
//         /// </summary>
//         // [HttpPost("submit-input")]
//         // public async Task<IActionResult> SubmitInput([FromBody] UserInputDTO input)
//         // {
//         //     if (input == null)
//         //     {
//         //         _logger.LogWarning("Null input received.");
//         //         return BadRequest(ResponseMessages.NullInput);
//         //     }

//         //     try
//         //     {
//         //         var result = await _userInputService.HandleUserInput(input);
//         //         return Ok(result);
//         //     }
//         //     catch (ArgumentException ex)
//         //     {
//         //         _logger.LogWarning(ex, "Invalid input data.");
//         //         return BadRequest(ex.Message);
//         //     }
//         //     catch (Exception ex)
//         //     {
//         //         _logger.LogError(ex, ResponseMessages.InternalServerError);
//         //         return StatusCode(500, new { message = ResponseMessages.InternalServerError, error = ex.Message });
//         //     }
//         // }

//                 [HttpPost("submit-input")]
//         public async Task<IActionResult> SubmitInput([FromBody] UserInputDTO input)
//         {
//             if (input == null)
//             {
//                 _logger.LogWarning("Null input received.");
//                 return BadRequest(new { message = "Input cannot be null." });
//             }
        
//             // Validate Age
//             if (input.Age < 18 || input.Age > 100)
//             {
//                 _logger.LogWarning("Invalid age: {Age}", input.Age);
//                 return BadRequest(new { message = input.Age < 18 ? "Minimum age is 18." : "Maximum age is 100." });
//             }
        
//             // Validate Investment Horizon
//             if (input.InvestmentHorizon < 1 || input.InvestmentHorizon > 30)
//             {
//                 _logger.LogWarning("Invalid investment horizon: {Horizon}", input.InvestmentHorizon);
//                 return BadRequest(new { message = input.InvestmentHorizon < 1 ? "Minimum investment horizon is 1 year." : "Maximum investment horizon is 30 years." });
//             }
        
//             // Validate Risk Tolerance
//             var validRiskTolerances = new[] { "Low", "Medium", "High" };
//             if (string.IsNullOrEmpty(input.RiskTolerance) || !validRiskTolerances.Contains(input.RiskTolerance))
//             {
//                 _logger.LogWarning("Invalid risk tolerance: {RiskTolerance}", input.RiskTolerance);
//                 return BadRequest(new { message = "Risk tolerance must be one of: Low, Medium, High." });
//             }
        
//             // Validate Target Amount
//             if (input.TargetAmount < 10000 || input.TargetAmount > 100000000)
//             {
//                 _logger.LogWarning("Invalid target amount: {TargetAmount}", input.TargetAmount);
//                 return BadRequest(new { message = input.TargetAmount < 10000 ? "Minimum target amount is ₹10,000." : "Maximum target amount is ₹10,00,00,000." });
//             }
        
//             try
//             {
//                 var result = await _userInputService.HandleUserInput(input);
//                 return Ok(result);
//             }
//             catch (ArgumentException ex)
//             {
//                 _logger.LogWarning(ex, "Invalid input data.");
//                 return BadRequest(new { message = ex.Message });
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "Internal server error.");
//                 return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
//             }
//         }
//     }
// }

// // using Microsoft.AspNetCore.Mvc;
// // using WisVestAPI.Models.DTOs;
// // using WisVestAPI.Services.Interfaces;
// // using Microsoft.AspNetCore.Authorization;
// // using Microsoft.Extensions.Logging;
// // using WisVestAPI;
 
// // namespace WisVestAPI.Controllers
// // {
// //     [Authorize]
// //     [ApiController]
// //     [Route("api/[controller]")]
// //     public class UserInputController : ControllerBase
// //     {
// //         private readonly IUserInputService _userInputService;
// //         private readonly ILogger<UserInputController> _logger;
 
// //         public UserInputController(IUserInputService userInputService, ILogger<UserInputController> logger)
// //         {
// //             _userInputService = userInputService;
// //             _logger = logger;
// //         }
 
// //         [HttpPost("submit-input")]
// //         public async Task<IActionResult> SubmitInput([FromBody] UserInputDTO input)
// //         {
// //             if (input == null)
// //             {
// //                 _logger.LogWarning("Null input received.");
// //                 return BadRequest(new { message = ResponseMessages.NullInput });
// //             }
 
// //             // Validate Age
// //             if (input.Age < 18 || input.Age > 100)
// //             {
// //                 _logger.LogWarning("Invalid age: {Age}", input.Age);
// //                 return BadRequest(new
// //                 {
// //                     message = input.Age < 18
// //                         ? ResponseMessages.AgeRange.Split(".")[0] + "."
// //                         : ResponseMessages.AgeRange.Split(".")[1].Trim() == "" ? ResponseMessages.AgeRange : ResponseMessages.AgeRange.Split(".")[1].Trim()
// //                 });
// //             }
 
// //             // Validate Investment Horizon
// //             if (input.InvestmentHorizon < 1 || input.InvestmentHorizon > 30)
// //             {
// //                 _logger.LogWarning("Invalid investment horizon: {Horizon}", input.InvestmentHorizon);
// //                 return BadRequest(new
// //                 {
// //                     message = input.InvestmentHorizon < 1
// //                         ? ResponseMessages.InvestmentHorizonRange.Split(".")[0] + "."
// //                         : ResponseMessages.InvestmentHorizonRange.Split(".")[1].Trim() == "" ? ResponseMessages.InvestmentHorizonRange : ResponseMessages.InvestmentHorizonRange.Split(".")[1].Trim()
// //                 });
// //             }
 
// //             // Validate Risk Tolerance
// //             var validRiskTolerances = ResponseMessages.RiskTolerancePattern.Split('|');
// //             if (string.IsNullOrEmpty(input.RiskTolerance) || !validRiskTolerances.Contains(input.RiskTolerance))
// //             {
// //                 _logger.LogWarning("Invalid risk tolerance: {RiskTolerance}", input.RiskTolerance);
// //                 return BadRequest(new { message = ResponseMessages.RiskToleranceInvalid });
// //             }
 
// //             // Validate Target Amount
// //             if (input.TargetAmount < 10000 || input.TargetAmount > 100000000)
// //             {
// //                 _logger.LogWarning("Invalid target amount: {TargetAmount}", input.TargetAmount);
// //                 return BadRequest(new
// //                 {
// //                     message = input.TargetAmount < 10000
// //                         ? ResponseMessages.TargetAmountRange.Split("to")[0].Trim() + "."
// //                         : "Maximum target amount is ₹10,00,00,000."
// //                 });
// //             }
 
// //             try
// //             {
// //                 var result = await _userInputService.HandleUserInput(input);
// //                 return Ok(result);
// //             }
// //             catch (ArgumentException ex)
// //             {
// //                 _logger.LogWarning(ex, "Invalid input data.");
// //                 return BadRequest(new { message = ex.Message });
// //             }
// //             catch (Exception ex)
// //             {
// //                 _logger.LogError(ex, ResponseMessages.InternalServerError);
// //                 return StatusCode(500, new { message = ResponseMessages.InternalServerError, error = ex.Message });
// //             }
// //         }
// //     }
// // }
