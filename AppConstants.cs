namespace WisVestAPI.Constants
{
    public static class AppConstants
    {
        // Age limits
        public const int MinAge = 18;
        public const int MaxAge = 100;

        // Investment horizon limits (years)
        public const int MinInvestmentHorizon = 1;
        public const int MaxInvestmentHorizon = 30;

        // Target amount limits (INR)
        public const int MinTargetAmount = 10_000;
        public const int MaxTargetAmount = 100_000_000;

        // Allowed risk tolerances
        public static readonly string[] ValidRiskTolerances = { "Low", "Medium", "High" };

        //AllocationService
    //      public const string FinalAllocationFilePath = "FinalAllocation.json";

    //     public const string RiskLow = "Low";
    //     public const string RiskMedium = "Medium";
    //     public const string RiskHigh = "High";
    //     public const string RiskMidMapped = "Mid";

    //     public const string HorizonShort = "short";
    //     public const string HorizonModerate = "moderate";
    //     public const string HorizonLong = "long";

    //     public const string HorizonShortMapped = "Short";
    //     public const string HorizonModerateMapped = "Mod";
    //     public const string HorizonLongMapped = "Long";
    //      public static class Goals
    // {
    //     public const string EmergencyFund = "EmergencyFund";
    //     public const string Retirement = "Retirement";
    //     public const string WealthAccumulation = "WealthAccumulation";
    //     public const string ChildEducation = "Child Education";
    // }

    // public static class AssetKeys
    // {
    //     public const string Cash = "Cash";
    //     public const string Equity = "Equity";
    //     public const string FixedIncome = "FixedIncome";
    //     public const string Commodities = "Commodities";
    //     public const string RealEstate = "RealEstate";
    // }

    // public static class GoalTuningKeys
    // {
    //     public const string FixedIncomeBoost = "fixedIncome_boost";
    //     public const string EquityReductionModerate = "equityReduction_moderate";
    //     // public const string FixedIncomeBoost = "FixedIncomeBoost";
    //     public const string RealEstateBoost = "RealEstateBoost";
    //     // public const string EquityReductionModerate = "EquityReductionModerate";
    //     public const string Balanced = "balanced";
    // }

    // public static class Thresholds
    // {
    //     public const double EmergencyFundCashMinimum = 0.2;
    //     public const double TotalAllocation = 1.0;
    //     public const double BigPurchaseCapPercentage = 30.0;
    //     public const double TotalAllocation_c= 100.0;
    //     public const double Tolerance = 0.01;
    // }

    // public static class Adjustments
    // {
    //     public const double EquityBoost = 0.05;
    // }

    }
}
