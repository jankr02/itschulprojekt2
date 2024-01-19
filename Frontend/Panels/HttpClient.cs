using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MesseauftrittDatenerfassung_UI.Dtos.CustomerDtos;
using MesseauftrittDatenerfassung_UI.Enums;
using MesseauftrittDatenerfassung_UI.Dtos.User;
using System.Net.Http.Headers;

namespace MesseauftrittDatenerfassung_UI
{
    public sealed class CustomerApiClient
    {
        private readonly HttpClient _httpClient;
        // public HttpClient _httpClient;

        private static CustomerApiClient _remoteDatabaseClient;
        private static CustomerApiClient _localDatabaseClient;

        private CustomerApiClient(DatabaseType databaseType)
        {
            _httpClient = new HttpClient();
            SetBaseAddress(databaseType);
        }

        // POST: Auth/Login
        public async Task<ServiceResponse<string>> Login(UserLoginDto content)
        {
            var jsonContent = JsonConvert.SerializeObject(content);
            var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var serviceResponse = new ServiceResponse<string>();
            try
            {
                var response = await _httpClient.PostAsync("Auth/Login", contentString);
                var responseContent = await response.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<ServiceResponse<string>>(responseContent);
                if (token.Success)
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Data);
                    _localDatabaseClient._httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Data);
                    _remoteDatabaseClient._httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Data);
                }
                serviceResponse.Success = token.Success;
                serviceResponse.Message = token.Message;
                serviceResponse.Data = token.Data;
            }
            catch (Exception)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Verbindung zur Datenbank konnte nicht hergestellt werden.";
            }
            return serviceResponse;
        }

        // GET: Test
        public async Task<bool> TestConnection()
        {
            var response = await _httpClient.GetAsync("Test");
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                return false;
            }
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<bool>(content);
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
        public async Task<GetCustomerDto> CreateCompleteCustomerAsync(AddCompleteCustomerDto customers)
        {
            var jsonContent = JsonConvert.SerializeObject(customers);
            var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync("api/Customer", contentString).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ServiceResponse<GetCustomerDto>>(responseContent).Data;
        }

        // POST: api/Customer
        public async Task<List<GetCustomerDto>> CreateMultipleCompleteCustomersAsync(List<AddCompleteCustomerDto> customers)
        {
            var jsonContent = JsonConvert.SerializeObject(customers);
            var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync("api/Customer", contentString).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ServiceResponse<List<GetCustomerDto>>>(responseContent).Data;
        }

        public static CustomerApiClient CreateOrGetClient(DatabaseType databaseType)
        {
            switch (databaseType)
            {
              case DatabaseType.RemoteDatabase:
              {
                return _remoteDatabaseClient ?? (_remoteDatabaseClient = new CustomerApiClient(DatabaseType.RemoteDatabase));
              }
              case DatabaseType.LocalDatabase:
              {
                return _localDatabaseClient ?? (_localDatabaseClient = new CustomerApiClient(DatabaseType.LocalDatabase));
              }
              default:
                throw new NotImplementedException();
            }
        }

        public async Task<bool> TestConnection(int numberOfConnectionTries)
        {
            for (var i = 0; i < numberOfConnectionTries; i++)
            {
                if(TestConnection().GetAwaiter().GetResult())
                {
                    return true;
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
            return false;
        }

        public static async Task<bool> IsInternetAvailableAsync()
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

        private void SetBaseAddress(DatabaseType databaseType)
        {
            _httpClient.BaseAddress = databaseType == DatabaseType.RemoteDatabase ? new Uri("http://localhost:5069/") : new Uri("http://localhost:5222/");
        }        
    }
}

