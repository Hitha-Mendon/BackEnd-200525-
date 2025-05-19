using System.Text.Json;
using WisVestAPI.Models.Matrix;

namespace WisVestAPI.Repositories.Matrix
{
    public class MatrixRepository
    {
        private readonly string _matrixFilePath;
        public MatrixRepository(string matrixFilePath)
    {
        _matrixFilePath = matrixFilePath;
    }

        public async Task<MatrixData?> LoadMatrixDataAsync()
{
    try
    {
        if (!File.Exists(_matrixFilePath))
            throw new FileNotFoundException($"Matrix file not found at {_matrixFilePath}");

        var json = await File.ReadAllTextAsync(_matrixFilePath);
        return JsonSerializer.Deserialize<MatrixData>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? throw new InvalidOperationException("Failed to deserialize matrix data.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error loading matrix data: {ex.Message}");
        throw;
    }
}
    }
}
