using System.Net.Http.Json;

namespace RaceDay.Web.Services
{
    public class RaceDayApiClient
    {
        private readonly HttpClient _httpClient;

        public RaceDayApiClient(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("RaceDayApi");
        }
        public async Task<T?> GetAsync<T>(string endpoint)
        {
            using var response = await _httpClient.GetAsync(endpoint);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>();
        }
        public async Task<HttpResponseMessage> PostAsync<T>(
            string endpoint, T data)
        
        {
            return await _httpClient.PostAsJsonAsync(endpoint, data);
        }
        public async Task<HttpResponseMessage> PutAsync<T>(
            string endpoint, T data)
        {
            return await _httpClient.PutAsJsonAsync(endpoint, data);
        }
        public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            return await _httpClient.DeleteAsync(endpoint);
        }

    }
}
