using Database.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModelsLibrary.Queries
{

    public class LoginQueries: ILoginQueries
    {
        private cvup00001Context dbContext;

        public LoginQueries()
        {
            dbContext = new cvup00001Context();
        }

        public List<user> getUsersByEmailPassword(string email,string password)
        {
            var usersList = dbContext.users.Where(x => x.email == email && x.passwaord == password).ToList();
            return usersList;
        }

    }
}
