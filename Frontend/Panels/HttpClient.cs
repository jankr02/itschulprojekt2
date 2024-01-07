using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MesseauftrittDatenerfassung_UI.Dtos.PictureDtos;
using MesseauftrittDatenerfassung_UI.Dtos.BusinessDtos;
using MesseauftrittDatenerfassung_UI.Dtos.CustomerProductGroupDto;
using MesseauftrittDatenerfassung_UI.Dtos.CustomerDtos;
using System.Net.Http.Headers;
using MesseauftrittDatenerfassung_UI.Enums;
using System.Collections.Generic;

namespace MesseauftrittDatenerfassung_UI
{
    public sealed class CustomerApiClient
    {
        private readonly HttpClient _httpClient;

        private static CustomerApiClient _remoteDatabaseClient;
        private static CustomerApiClient _localDatabaseClient;

        private CustomerApiClient(DatabaseType databaseType)
        {
            _httpClient = new HttpClient();
            SetBaseAddress(databaseType);
        }

        // GET: api/Customer
        public async Task<List<GetCustomerDto>> GetAllCustomersAsync()
        {
            var response = await _httpClient.GetAsync("api/Customer");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ServiceResponse<List<GetCustomerDto>>>(content).Data;
        }

        // POST: api/Customer
        public async Task<GetCustomerDto> CreateCustomerAsync(AddCustomerDto customer)
        {
            var jsonContent = JsonConvert.SerializeObject(customer);
            var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync("api/Customer", contentString).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ServiceResponse<GetCustomerDto>>(responseContent).Data;
        }

        // POST: api/Customer/Multiple
        public async Task<List<GetCustomerDto>> CreateMultipleCustomersAsync(List<AddCompleteCustomerDto> customers)
        {
            var jsonContent = JsonConvert.SerializeObject(customers);
            var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync("api/Customer/Multiple", contentString).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ServiceResponse<List<GetCustomerDto>>>(responseContent).Data;
        }

        // PUT: api/Customer
        public async Task<GetCustomerDto> UpdateCustomerAsync(UpdateCustomerDto customer)
        {
            var jsonContent = JsonConvert.SerializeObject(customer);
            var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("api/Customer", contentString);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetCustomerDto>(responseContent);
        }

        // GET: api/Customer/{id}
        public async Task<GetCustomerDto> GetCustomerByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/Customer/{id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetCustomerDto>(content);
        }

        // DELETE: api/Customer
        public async Task<List<GetCustomerDto>> TruncateAllTablesAsync()
        {
            var response = await _httpClient.DeleteAsync("api/Customer");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ServiceResponse<List<GetCustomerDto>>>(content).Data;
        }

        // DELETE: api/Customer/{id}
        public async Task DeleteCustomerAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Customer/{id}");
            response.EnsureSuccessStatusCode();
        }

        // POST: api/Customer/ProductGroup
        public async Task AddProductGroupsToCustomerAsync(List<AddCustomerProductGroupDto> productGroups)
        {
            var jsonContent = JsonConvert.SerializeObject(productGroups);
            var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"api/Customer/ProductGroup", contentString);
            response.EnsureSuccessStatusCode();
        }

        // POST: api/Customer/Business/{customerId}
        public async Task AddBusinessToCustomerAsync(int customerId, AddBusinessDto business)
        {
            var jsonContent = JsonConvert.SerializeObject(business);
            var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"api/Customer/Business/{customerId}", contentString);
            response.EnsureSuccessStatusCode();
        }

        // DELETE: api/Customer/Business/{businessId}
        public async Task DeleteBusinessAsync(int businessId)
        {
            var response = await _httpClient.DeleteAsync($"api/Customer/Business/{businessId}");
            response.EnsureSuccessStatusCode();
        }

        // POST: api/Customer/Picture/{customerId}
        public async Task AddPictureToCustomerAsync(int customerId, AddPictureDto picture)
        {
            using (var formData = new MultipartFormDataContent())
            {
                if (picture.Image != null)
                {
                    var imageContent = new ByteArrayContent(picture.Image);
                    imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg"); 

                    formData.Add(imageContent, "image", picture.Name); 
                }

                formData.Add(new StringContent(picture.Name), "Name");

                var response = await _httpClient.PostAsync($"api/Customer/Picture/{customerId}", formData);
                response.EnsureSuccessStatusCode();
            }
        }

        public static CustomerApiClient CreateOrGetClient(DatabaseType databaseType)
        {
            if(databaseType == DatabaseType.RemoteDatabase)
            {
                if(_remoteDatabaseClient == null)
                {
                    _remoteDatabaseClient = new CustomerApiClient(DatabaseType.RemoteDatabase);
                }
                return _remoteDatabaseClient;
            }

            if (databaseType == DatabaseType.LocalDatabase)
            {
                if (_localDatabaseClient == null)
                {
                    _localDatabaseClient = new CustomerApiClient(DatabaseType.LocalDatabase);
                }
                return _localDatabaseClient;
            }

            throw new NotImplementedException();
        }

        private void SetBaseAddress(DatabaseType databaseType)
        {
            if (databaseType == DatabaseType.RemoteDatabase)
            {
                _httpClient.BaseAddress = new Uri("http://localhost:5069/");
            }
            else
            {
                _httpClient.BaseAddress = new Uri("http://localhost:5222/");
            }
        }

        public async Task<bool> IsInternetAvailableAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync("http://www.google.com");
                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}

