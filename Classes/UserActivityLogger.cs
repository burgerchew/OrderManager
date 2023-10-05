using OrderManagerEF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderManagerEF.Data;

namespace OrderManagerEF.Classes
{
    public class UserActivityLogger
    {
        private readonly int _userId;
        private readonly OMDbContext _context;

        public UserActivityLogger(int userId, OMDbContext context)
        {
            _userId = userId;
            _context = context;
        }

        public void Log(string description)
        {
            var activity = new UserActivity
            {
                UserId = _userId,
                ActivityDescription = description,
                Timestamp = DateTime.Now
            };

            _context.UserActivities.Add(activity);
            _context.SaveChanges();
        }
    }

}
