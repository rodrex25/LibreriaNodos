using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaNodos
{

    

    
    public class Cliente : Nodo
    {
        public Socket clientServerTcpClient;
        public int serverPort;
        public string serverIpAddress;


        public Cliente(User user, int serverPort, string serverIpAddress):base(user) {

            this.localUser = user;
            this.serverPort = serverPort;
            this.serverIpAddress = serverIpAddress;

        }

        public void connServer() {

            IPAddress localIp = IPAddress.Parse(this.serverIpAddress);
            IPEndPoint endPoint = new IPEndPoint(localIp, this.serverPort);

            this.clientServerTcpClient = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp); ;
            Send(clientServerTcpClient, localUser);

            ThreadPool.QueueUserWorkItem(this.HandleConn, this.clientServerTcpClient);
            
        }

        public void connNode(User user)
        {
            if (this.FindUser(user) != null) {

                IPAddress userIp = IPAddress.Parse(user.getUserIpAdress());
                IPEndPoint endPoint = new IPEndPoint(userIp, user.getUserPort());

                Socket userTcpClient = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                Send(userTcpClient, localUser);
                Nodo tempNodo = new Nodo(user);
                tempNodo.setTcpClient(clientServerTcpClient);

                this.nodes.Add(tempNodo);

                ThreadPool.QueueUserWorkItem(this.HandleConn, userTcpClient);

            }
            

        }

        public void stopClient()
        {
            foreach (Nodo nodo in this.nodes)
            {

                Send(nodo.GetTcpClient(), this.localUser);

            }

        }


    }
}
