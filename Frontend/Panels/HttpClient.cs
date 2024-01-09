using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.NetworkInformation;
using MesseauftrittDatenerfassung_UI.Dtos.PictureDtos;
using MesseauftrittDatenerfassung_UI.Dtos.BusinessDtos;
using MesseauftrittDatenerfassung_UI.Dtos.CustomerProductGroupDto;
using MesseauftrittDatenerfassung_UI.Dtos.CustomerDtos;
using MesseauftrittDatenerfassung_UI.Dtos.User;
using System.Net.Http.Headers;

namespace MesseauftrittDatenerfassung_UI
{
    public class CustomerApiClient
    {
        private readonly HttpClient _httpClient;


        public CustomerApiClient()
        {
            _httpClient = new HttpClient();
        }

        // POST: Auth/Login
        public async Task<string> Login(UserLoginDto content)
        {
            var jsonContent = JsonConvert.SerializeObject(content);
            var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Auth/Login", contentString);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<string>(responseContent);
        }
        
        // GET: api/Customer
        public async Task<GetCustomerDto> GetCustomerAsync()
        {
            var response = await _httpClient.GetAsync("api/Customer");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetCustomerDto>(content);
        }

        // POST: api/Customer
        public async Task<GetCustomerDto> CreateCustomerAsync(AddCustomerDto customer)
        {
            var jsonContent = JsonConvert.SerializeObject(customer);
            var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Customer", contentString);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetCustomerDto>(responseContent);
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

        // DELETE: api/Customer/{id}
        public async Task DeleteCustomerAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Customer/{id}");
            response.EnsureSuccessStatusCode();
        }

        // POST: api/Customer/ProductGroup
        public async Task AddProductGroupToCustomerAsync(int customerId, AddCustomerProductGroupDto productGroup)
        {
            var jsonContent = JsonConvert.SerializeObject(productGroup);
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

        public static async Task<CustomerApiClient> CreateAsync()
        {
            var client = new CustomerApiClient();
            await client.SetBaseAddress();
            return client;
        }

        private async Task SetBaseAddress()
        {
            if (await IsInternetAvailableAsync())
            {
                _httpClient.BaseAddress = new Uri("http://localhost:5069/swagger/index.html");
            }
            else
            {
                _httpClient.BaseAddress = new Uri("http://localhost:5222/swagger/index.html");
            }
        }

        private async Task<bool> IsInternetAvailableAsync()
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

