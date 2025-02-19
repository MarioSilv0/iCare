using System.Text;
using System.Text.Json;

namespace backend.Services
{
    public class TacoApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public TacoApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            // Configura o cliente HTTP para apontar para a TACO API
            _httpClient.BaseAddress = new Uri("http://localhost:4000/graphql");
        }

        public async Task<string> GetFoodDataAsync(string query)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "")
            {
                Content = new StringContent(JsonSerializer.Serialize(new { query }),
                    Encoding.UTF8,
                    "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
