using System.Runtime.Serialization;

namespace WisVestAPI.Constants
{

    public static class ResponseMessages
    {
        public const string NullUserInput = "User input cannot be null.";
        public const string AllocationComputationFailed = "Allocation could not be computed or formatted correctly.";
        public const string AllocationFormatError = "Error: Final allocation data format is incorrect.";
        public const string NoCalculatedAllocationsFound = "No calculated allocations found.";
        public const string ErrorRetrievingAllocations = "An error occurred while retrieving the calculated allocations: ";
 
        // Validation messages for UserInputDTO
        public const string RiskToleranceRequired = "Risk tolerance is required.";
        public const string RiskToleranceInvalid = "Risk tolerance must be one of: Low, Medium, High.";
        public const string RiskTolerancePattern = @"Low|Medium|High";
 
        public const string InvestmentHorizonRequired = "Investment horizon is required.";
        public const string InvestmentHorizonRange = "Investment horizon must be between 1 and 30 years.";
 
        public const string AgeRequired = "Age is required.";
        public const string AgeRange = "Age must be between 18 and 100.";
 
        public const string GoalRequired = "Goal is required.";
 
        public const string TargetAmountRequired = "Target amount is required.";
        public const string TargetAmountRange = "Target amount must be between 10,000 and 100,000,000.";


        public const string EmailAlreadyExists = "Email already exists.";
        public const string RegistrationSuccess = "Registration successful.";
        public const string LoginSuccess = "Login successful.";
        public const string InvalidCredentials = "Invalid credentials.";
        public const string LoginError = "An error occurred during login.";
        public const string RegistrationError = "An error occurred during registration.";
        // public const string ProductAllocationsFetchError = "An error occurred while retrieving product allocations.";
        public const string AllocationResultNull = "Allocation result cannot be null.";
        // public const string TargetAmountInvalid = "Target amount must be greater than zero.";
        public const string HorizonInvalid = "Investment horizon must be greater than zero.";
        public const string NoProductAllocationsFound = "No product allocations found.";
        public const string ProductAllocationCalculationError = "Error occurred while calculating product allocations.";

       
        public const string ProductJsonDeserializationFailed = "Failed to deserialize product data. Ensure the JSON structure is correct.";
        public const string JsonReadError = "Error reading product JSON file.";
        public const string UnexpectedError = "An unexpected error occurred while loading products.";
        public const string NullInput = "User input cannot be null.";
        // public const string InternalServerError = "An unexpected error occurred while processing user input.";




        // public const string InvalidPercentageSplit = "Percentage split must be between 1 and 100.";

        public const string InvalidTargetAmount = "Target amount must be greater than zero.";
        public const string InvalidAnnualReturn = "Annual return cannot be negative.";
        public const string InvalidInvestmentHorizon = "Investment horizon cannot be negative or zero.";
        public const string DenominatorZero = "Denominator in investment calculation is zero.";

        public const string CalculationStarted = "Calculating investment amount with PercentageSplit: {PercentageSplit}, TargetAmount: {TargetAmount}, AnnualReturn: {AnnualReturn}, InvestmentHorizon: {InvestmentHorizon}.";
        public const string CalculationCompleted = "Calculated investment amount: {InvestmentAmount}.";

        public const string InvalidInputLog = "Invalid input detected: {Message}";
        public const string InvalidInputError = "Error processing input parameters";

        public const string DivideByZeroLog = "Mathematical error encountered: {Message}";
        public const string DivideByZeroError = "Investment calculation failed due to divide by zero.";

        public const string OverflowLog = "Overflow error detected: {Message}";
        public const string OverflowError = "Calculation resulted in an overflow error.";

        public const string UnexpectedErrorLog = "Unexpected error occurred: {Message}";
        // public const string UnexpectedError = "An unexpected error occurred during investment calculation.";




        public const string InputCannotBeNull = "User input cannot be null.";
        public const string AllocationCalculationFailed = "Allocation calculation failed.";
        public const string AllocationCalculationError = "Error during allocation calculation.";
        public const string AllocationProcessingError = "Error while processing allocation.";
        public const string SerializationError = "Error while serializing result.";
        public const string InvalidDictionaryCast = "Value for key '{0}' is not a valid Dictionary<string, double>.";



        public const string UserFilePathNotConfigured = "UserFilePath is not configured in appsettings.json.";
        public const string FileAccessDenied = "Access denied to the file at {0}.";
        public const string FileReadError = "An I/O error occurred while reading the file at {0}.";
        public const string FileWriteError = "An I/O error occurred while writing to the file at {0}.";
        public const string InvalidJson = "The file at {0} contains invalid JSON.";
        public const string JsonSerializationError = "Failed to serialize the user data to JSON.";
        public const string UserExistsCheckFailed = "Failed to check if the user with email {0} exists.";
        public const string UserRetrievalFailed = "Failed to retrieve the user with email {0}.";
        public const string UserAddFailed = "Failed to add the user with email {0}.";
        public const string UserUpdateFailed = "Failed to update the user with ID {0}.";
        public const string UserNotFound = "User with ID {0} was not found.";
        public const string UserValidationFailed = "Failed to validate login for the user with email {0}.";





        public const string AllocationSuccess = "Product allocation successful.";
        public const string AppSettingsNull = "AppSettings section is missing in configuration.";
        public const string ProductDataNull = "Product data is null or empty.";
        public const string AllocationError = "Error occurred while calculating product allocation.";

        public static SerializationInfo SaveAllocationFailure { get; internal set; }


        public const string StartingAllocation = "Starting product allocation calculation.";
        public const string NoProductsFound = "No products found for {Asset} -> {SubAsset}";
        public const string TotalReturnsZero = "Total returns for {SubAsset} is zero or negative.";
        public const string SavingAllocations = "Product allocations saved to {Path}";
        public const string NoOutputFile = "No output file found at {Path}.";
        public const string ProductDataNotFound = "Product data not found for {Asset} -> {SubAsset}";

        // Exception messages
        public const string ProductFileNotFound = "Product JSON file not found.";
        public const string ProductDataNull1 = "Product JSON deserialization returned null.";
        public const string AllocationFailure = "Failed to calculate product allocations.";
        public const string LoadProductFailure = "Unable to load product data from JSON file.";
        public const string SaveAllocationFailure1 = "Unable to write product allocations to file.";
        public const string ReadAllocationFailure = "Unable to load product allocation results.";
        public const string ProductFetchError = "Error fetching products.";

        //AllocatiionController
        // public const string InputCannotBeNull = "Input cannot be null.";
        public const string AllocationNotComputed = "Allocation could not be computed or formatted correctly.";
        public const string AllocationDataFormatIncorrect = "Final allocation data format is incorrect.";
        
        public const string AllocationRetrievalError = "An error occurred while retrieving the calculated allocations.";
       
        //ProductAllocController
        public const string ProductAllocationsFetchError = "An error occurred while fetching product allocations.";
    public const string AllocationResultCannotBeNull = "Allocation result cannot be null.";
    public const string InvalidPercentageSplit = "Percentage split must be between 0 and 100.";
    
    public const string ProductAllocationsCalculationError = "Error occurred while calculating product allocations.";

    // Log Messages
    public const string ProductAllocationsFetchErrorLog = "Error occurred while fetching product allocations.";
    public const string ProductAllocationsCalculationErrorLog = "Error occurred while calculating product allocations.";
    public const string SubAllocationDataReceivedLog = "Sub-allocation Data Received: {Data}";
    public const string TargetAmountLog = "Target Amount: {Amount}";
    public const string InvestmentHorizonLog = "Investment Horizon: {Horizon}";
    public const string InvalidPercentageSplitLog = "Invalid PercentageSplit detected: {Value} for sub-asset {SubAsset} in asset {Asset}";
    //ProductsController

        
        // public const string JsonReadError = "An error occurred while reading the JSON file.";
       
         public const string ProductJsonNotFound = "Product JSON file not found at the configured path. ";
      
      
        



        // User input validation messages
        // public const string NullInput = "Input cannot be null.";
        public const string InvalidAgeUnder = "Minimum age is 18.";
        public const string InvalidAgeOver = "Maximum age is 100.";
        public const string InvalidInvestmentHorizonUnder = "Minimum investment horizon is 1 year.";
        public const string InvalidInvestmentHorizonOver = "Maximum investment horizon is 30 years.";
        public const string InvalidRiskTolerance = "Risk tolerance must be one of: Low, Medium, High.";
        public const string InvalidTargetAmountUnder = "Minimum target amount is ₹10,000.";
        public const string InvalidTargetAmountOver = "Maximum target amount is ₹10,00,00,000.";

        // Server error
        public const string InternalServerError = "An error occurred while processing your request.";

        // Logging messages
        public const string LogNullInputReceived = "Null input received.";
        public const string LogInvalidAge = "Invalid age: {Age}";
        public const string LogInvalidInvestmentHorizon = "Invalid investment horizon: {Horizon}";
        public const string LogInvalidRiskTolerance = "Invalid risk tolerance: {RiskTolerance}";
        public const string LogInvalidTargetAmount = "Invalid target amount: {TargetAmount}";
        public const string LogInvalidInputData = "Invalid input data.";
        public const string LogInternalServerError = "Internal server error.";
        //AllocationService
//          public const string AllocationCalculationStarted = "Starting allocation calculation...";
//         public const string AllocationMatrixNull = "Allocation matrix is null.";
//         public const string AllocationMatrixLoaded = "Allocation matrix loaded successfully.";
//         public const string FinalAllocationSaved = "Final allocation saved successfully to {0}";
//         public const string AllocationFileSaveError = "Error occurred while saving final allocation to file.";
//         public const string HorizonMissing = "InvestmentHorizon is required and must be greater than zero.";
//         public const string RiskToleranceMissing = "RiskTolerance is required";
//         public const string BaseAllocationLookup = "Looking up base allocation for key: {0}";
//         public const string BaseAllocationNotFound = "No base allocation found for key: {0}";
//         public const string BaseAllocationInvalidCombo = "Invalid combination of RiskTolerance and InvestmentHorizon: {0}";
//         public const string BaseAllocationFound = "Base allocation found: {0}";
//         public const string AgeAdjustmentLookup = "Looking up age adjustment rules for key: {0}";
//         public const string AgeAdjustmentsFound = "Age adjustments found: {0}";
//         public const string AgeAdjustmentNotFound = "No age adjustments found for key: {0}";
//         public const string AgeAdjustmentError = "Error applying age adjustments: {0}";
//         public const string GoalTuningLookup = "Looking up goal tuning for goal: {0}";
//         public const string GoalMissing = "Goal is required.";
//         public const string AllocationSuccess = "Asset allocation completed successfully.";
//     public const string AllocationFailed = "Asset allocation failed due to invalid inputs.";
//     public const string GoalNotRecognized = "The specified goal is not recognized.";
//     public static class GoalTuning
//     {
//         public const string BigPurchaseBalancingEnabled = "Big Purchase goal tuning: balancing enabled.";
//         public const string BigPurchaseCapLog = "{AssetKey} capped at {Threshold}%, excess {Excess}% collected.";
//         public const string BigPurchaseShareLog = "{Share}% added to {AssetKey}. New value: {NewValue}%";
//     }

//     public static class Warnings
//     {
//         public const string NoGoalTuningFound = "No goal tuning found for goal: {Goal}";
//     }

//     public static class Errors
//     {
//         public const string NormalizationError = "Error normalizing allocation.";
//     }
//     public const string NormalizationError = "Error normalizing allocation.";
// public const string SubAssetsAdded = "Added sub-assets for {0}: {1}";
// public const string NoSubAssets = "No sub-assets for {0}. Added empty sub-assets.";
// public const string FinalAllocationFormatted = "Final formatted allocation: {0}";
// public const string AllocationCalculationError = "Error during allocation calculation.";
// public const string ReadingFinalAllocation = "Attempting to read final allocation from JSON file: {0}";
// public const string FinalAllocationFileNotFound = "Final allocation file not found at {0}. Returning null.";
// public const string FinalAllocationReadSuccess = "Final allocation JSON read successfully.";
// public const string FinalAllocationNull = "Deserialized final allocation is null. Returning null.";
// public const string FinalAllocationReadError = "Error occurred while reading final allocation from file.";
// public const string SubAllocationMatrixLoadError = "Error loading sub-allocation matrix.";
// public const string NoMappingFoundForAssetClass = "No mapping found for asset class: {0}";
// public const string NoSubAllocationRulesFound = "No sub-allocation rules found for asset class: {0}";
// public const string NoWeightsForRiskLevel = "No weights found for risk level '{0}' in asset class '{1}'";
// public const string ComputedSubAllocations = "Sub-allocations for {0}: {1}";
// public const string SubAllocationComputationError = "Error computing sub-allocations.";


        public const string find = "JwtSettings:SecretKey";

    }

}