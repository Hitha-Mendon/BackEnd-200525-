namespace WisVestAPI.Models.DTOs
{
    public class ProductAllocationRequestDTO
    {
        public Dictionary<string, List<ProductDTO>> SubAllocationResult { get; set; }
        public UserInputDTO UserInput { get; set; }
    }
}