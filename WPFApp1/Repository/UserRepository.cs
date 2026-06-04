using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
      public class UserRepository
    {
        Fpf1LoginContext Context;

        public UserRepository()
        {
            Context = new Fpf1LoginContext();
        }

        //null safety is implemented by using the nullable reference type syntax (Account ?), which indicates that the method may return null if no matching user is found.
        public Account ? GetUserByUsername(string username, string password){
            return Context.Accounts.FirstOrDefault(u => u.Username == username && u.Password == password);
        }
    }
}
