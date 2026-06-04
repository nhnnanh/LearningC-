using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class UserService
    {
        UserRepository userRepository;

        public UserService()
        {
            userRepository = new UserRepository();
        }

        public bool AuthenticateUser(string username, string password)
        {
            if(username.Length <3 || password.Length < 6)
            {
                return false;
            }

            var user = userRepository.GetUserByUsername(username, password);
            return user != null;
        }
    }
}
