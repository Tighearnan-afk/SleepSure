using SleepSure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepSure.Services
{
    public interface IUserRESTService
    {
        public Task<List<User>> RefreshUsersAsync();
        public Task SaveUserAsync(User user, bool isNewUser);
        public Task DeleteUserAsync(int id);
    }
}
