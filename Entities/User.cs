using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagerEF.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // IMPORTANT: Store hashed passwords, not plaintext!
        public ICollection<UserActivity> Activities { get; set; }
    }

    public class UserActivity
    {
        public int Id { get; set; }
        public string ActivityDescription { get; set; }
        public DateTime Timestamp { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }

}
