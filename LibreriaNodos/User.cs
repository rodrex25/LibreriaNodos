using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaNodos
{
    [Serializable]
    public class User
    {
        //atributos

        public string userName;
        public string userIpAdress;
        public int userPort;


        //constructor

        public User(string userName, string userIpAdress, int userPort) {
            
            this.userName = userName;
            this.userIpAdress = userIpAdress;
            this.userPort = userPort;
        }

        //metodos

        //get

        public string getUserName() {
            return userName;
        }
        public string getUserIpAdress()
        {
            return userIpAdress;
        }
        public int getUserPort()
        {
            return userPort;
        }

        //set
        

        //to stringnn

        
    }
}
