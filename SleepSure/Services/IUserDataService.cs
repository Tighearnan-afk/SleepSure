using SleepSure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepSure.Services
{
    public interface IUserDataService
    {
        public Task<List<User>> GetUsersAsync();
        public Task AddUserAsync(string email, string password);
    }
}
