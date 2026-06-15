using Repository;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class UserService
    {
        UserRepository userrepository;
        public UserService()
        {
            userrepository = new UserRepository();
        }

        public User? Login(String username, String password)
        { 
            if (username.Length <3 || password.Length <3)
            {
                return null;
            }

            var user = userrepository.GetUser(username, password);
            return user;
        }
    }
}
