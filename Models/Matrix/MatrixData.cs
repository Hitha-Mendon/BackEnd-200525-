namespace WisVestAPI.Models.Matrix
{
    public class MatrixData
    {
        public Dictionary<string, Dictionary<string, double>> Risk_Horizon_Allocation { get; set; }
        public Dictionary<string, Dictionary<string, double>> Age_Adjustment_Rules { get; set; }
        public Dictionary<string, Dictionary<string, object>> Goal_Tuning { get; set; }


    }
}
