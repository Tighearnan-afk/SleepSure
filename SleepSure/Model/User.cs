using SQLite;

namespace SleepSure.Model
{
    [Table("user")]
    public class User
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public User() { }

        public User(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
