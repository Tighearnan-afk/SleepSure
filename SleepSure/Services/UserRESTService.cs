using SleepSure.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SleepSure.Services
{
    public class UserRESTService : IUserRESTService
    {
        HttpClient _client;
        JsonSerializerOptions _serializerOptions;

        public List<User> Users { get; private set; }

        public UserRESTService()
        {
            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<List<User>> RefreshUsersAsync()
        {
            Users = [];
            string usersEndPoint = string.Concat(Constants.RestUrl, $"users/{{0}}");

            Uri uri = new Uri(string.Format(usersEndPoint, string.Empty));
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Users = JsonSerializer.Deserialize<List<User>>(content, _serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return Users;
        }

        public Task SaveUserAsync(User user, bool isNewUser)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUserAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
