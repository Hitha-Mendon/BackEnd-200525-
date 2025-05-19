using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using WisVestAPI.Configuration;
using WisVestAPI.Models.Matrix;

namespace WisVestAPI.Services
{
public class ProductRepositoryService
{
private readonly string _jsonFilePath;
    public ProductRepositoryService(IOptions<AppSettings> options)
    {
        _jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), options.Value.ProductJsonFilePath);
    }

    public async Task<Dictionary<string, Dictionary<string, List<Product>>>?> LoadProductMatrixAsync()
    {
        if (!File.Exists(_jsonFilePath))
            return null;

        var json = await File.ReadAllTextAsync(_jsonFilePath);
        return JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<Product>>>>(json);
    }
}
}
