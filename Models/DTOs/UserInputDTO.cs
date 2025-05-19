using System.Text.Json.Serialization; 
using System.ComponentModel.DataAnnotations;
namespace WisVestAPI.Models.DTOs
{
    public class UserInputDTO
    {

        public string? RiskTolerance { get; set; }    

        [JsonPropertyName("investmentHorizon")]       
        public int InvestmentHorizon { get; set; }     
        public int Age { get; set; }
        public string? Goal { get; set; }                  
        public decimal TargetAmount { get; set; }           
    }
}

// using System.Text.Json.Serialization; 
// using System.ComponentModel.DataAnnotations;
// namespace WisVestAPI.Models.DTOs
// {
//     public class UserInputDTO
//     {
//         [Required(ErrorMessage = "Risk tolerance is required.")]
//         [RegularExpression("Low|Medium|High", ErrorMessage = "Risk tolerance must be one of: Low, Medium, High.")]
//         public string? RiskTolerance { get; set; }    
//         [Required(ErrorMessage = "Investment horizon is required.")]
//         [Range(1, 30, ErrorMessage = "Investment horizon must be between 1 and 30 years.")]
//         [JsonPropertyName("investmentHorizon")]       
//         public int InvestmentHorizon { get; set; }  
//         [Required(ErrorMessage = "Age is required.")]
//         [Range(18, 100, ErrorMessage = "Age must be between 18 and 100.")]     
//         public int Age { get; set; }
//         [Required(ErrorMessage = "Goal is required.")]
//         public string? Goal { get; set; }  
//         [Required(ErrorMessage = "Target amount is required.")]
//         [Range(10000, 100000000, ErrorMessage = "Target amount must be between 10,000 and 100,000,000.")]                  
//         public decimal TargetAmount { get; set; }           
//     }
// }

// using System.ComponentModel.DataAnnotations;
// using System.Text.Json.Serialization;
// using WisVestAPI.Constants;
// using WisVestAPI;
 
// namespace WisVestAPI.Models.DTOs
// {
//     public class UserInputDTO
//     {
//         [Required(ErrorMessage = ResponseMessages.RiskToleranceRequired)]
//         [RegularExpression(ResponseMessages.RiskTolerancePattern, ErrorMessage = ResponseMessages.RiskToleranceInvalid)]
//         public string? RiskTolerance { get; set; }
 
//         [Required(ErrorMessage = ResponseMessages.InvestmentHorizonRequired)]
//         [Range(1, 30, ErrorMessage = ResponseMessages.InvestmentHorizonRange)]
//         [JsonPropertyName("investmentHorizon")]
//         public int InvestmentHorizon { get; set; }
 
//         [Required(ErrorMessage = ResponseMessages.AgeRequired)]
//         [Range(18, 100, ErrorMessage = ResponseMessages.AgeRange)]
//         public int Age { get; set; }
 
//         [Required(ErrorMessage = ResponseMessages.GoalRequired)]
//         public string? Goal { get; set; }
 
//         [Required(ErrorMessage = ResponseMessages.TargetAmountRequired)]
//         [Range(10000, 100000000, ErrorMessage = ResponseMessages.TargetAmountRange)]
//         public decimal TargetAmount { get; set; }
//     }
// }

    