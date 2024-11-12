using WizardSoftTestTaskAPI.DTO.Catalog;
using WizardSoftTestTaskAPI.DTO.Paginations;
using Newtonsoft.Json;

namespace WizardSoftTestTaskClient.Repositories
{
    public class CatalogRepository
    {
        private readonly HttpClient _httpClient;

        private const string _serverAddress = "http://localhost:5000/api/v1/";

        public CatalogRepository()
        {
            _httpClient = new HttpClient();
        }

        public async Task<ResponsePaginationDTO<CatalogDTO>?> GetCatalogs(long? parentCatalogId = null, int pageNumber = 1)
        {
            string parentCatalogIdQuery = string.Empty;
            if (parentCatalogId != null)
            {
                parentCatalogIdQuery = $"&parentId={parentCatalogId}";
            }

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, $"{_serverAddress}Catalogs?PageNumber={pageNumber}{parentCatalogIdQuery}");
                using var response = await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<ResponsePaginationDTO<CatalogDTO>>(responseBody);

                return responseObject;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }

        public async Task<CatalogDTO?> GetCatalogById(long catalogId)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, $"{_serverAddress}Catalogs/{catalogId}");
                using var response = await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<CatalogDTO>(responseBody);

                return responseObject;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }

        public async Task<CatalogDTO?> CreateCatalog(long? parentId, string name)
        {
            var parentIdStr = parentId == null ? "null" : parentId.ToString();

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, $"{_serverAddress}Catalogs");
                request.Content = new StringContent($"{{\r\n  \"name\": \"{name}\",\r\n  \"parentId\": {parentIdStr}\r\n}}", null, "application/json");
                using var response = await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<CatalogDTO>(responseBody);

                return responseObject;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }

        public async Task<bool> DeleteCatalog(long catalogId)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Delete, $"{_serverAddress}Catalogs/{catalogId}");
                using var response = await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }

        public async Task<CatalogDTO?> UpdateCatalog(long catalogId, string? name = null, long? newCatalogId = null)
        {
            var newCatalogIdStr = newCatalogId == null ? "null" : newCatalogId.ToString();

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Put, $"{_serverAddress}Catalogs/{catalogId}");
                request.Content = new StringContent($"{{\r\n  \"name\": \"{name}\",\r\n  \"parentId\": {newCatalogIdStr} \r\n}}", null, "application/json");
                using var response = await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<CatalogDTO>(responseBody);

                return responseObject;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }
    }
}