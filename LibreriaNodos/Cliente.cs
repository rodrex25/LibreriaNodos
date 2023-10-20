using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaNodos
{

    

    
    public class Cliente : Nodo
    {
        private TcpClient clientServerTcpClient;
        private int serverPort;
        private string serverIpAddress;


        public Cliente(User user, int serverPort, string serverIpAddress):base(user) {

            this.localUser = user;
            this.serverPort = serverPort;
            this.serverIpAddress = serverIpAddress;

        }

        public void connServer() { 

            this.clientServerTcpClient = new TcpClient(this.serverIpAddress, this.serverPort);
            ThreadPool.QueueUserWorkItem(this.HandleConn, this.clientServerTcpClient);
            
        }

        public void connNode(User user)
        {

            TcpClient userTcpClient = new TcpClient(user.getUserIpAdress(), user.getUserPort());
            ThreadPool.QueueUserWorkItem(this.HandleConn, userTcpClient);

        }


    }
}
