using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaNodos
{
    public class Server:Nodo
    {
        
        public Server (User user) : base(user)
        {
            
        }

        public void ServerStart()
        {

            this.start();


        }

        public void ServerBroadcastUserOnlineList() {

            foreach (Nodo nodo in this.nodes) {

                Send(nodo.GetTcpClient(), this.userList);

            }
            
        }

        public void ServerStop()
        {
            this.ServerStop();
        }
    }
}
