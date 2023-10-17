using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaNodos
{
    internal class Nodo
    {
        //atributos
        private TcpListener tcpListener;

        private TcpClient tcpClient;
        
        private List<User> userList;

        private List<Nodo> nodes;

        private LinkedList<Message> messages;

        private User localUser;

        //constructor
        public Nodo(IPAddress ipAddress, int Port) {


            this.tcpListener = new TcpListener(ipAddress, Port);

        
        }

         
        

        public void Listen()
        {
            //el nodo escucha
            tcpListener.Start();

            //si se conecta un usuario le hace un hilo

            
            while (true)
            {
                //
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                //si recibe un cliente maneja la conexion
                ThreadPool.QueueUserWorkItem(this.HandleConn, tcpClient);
            }

        }

        //usuario que se conecta

       private void HandleConn(Object tcpClient) {

            Object recived = Recive((TcpClient)tcpClient);

            if (recived != null)
            {
                if (recived is User)
                {
                    //handle new user 
                    this.HandleUser((User)recived);

                }
                else if (recived is Message) 
                {
                    //handle new message
                    this.HandleMessages((Message)recived);
                }
                else if (recived is List<User>)
                {
                    //handle List users
                    this.HandleListUsers((List<User>)recived);
                }

            }
            else {
                Console.WriteLine("No se recibio nada");
            }
        }

        //handle new user

        //pendiente
        private void HandleUser(User user) { 
            //verificar si existe en la lista actual
            //si no agregarlo y generar una instancia Nodo y agregar a la lista de nodos
            
        }

        //pendiente
        //handle new message
        private void HandleMessages(Message message) { 
            messages.AddLast(message);
        }

        //pendiente
        //handle list users update
        private void HandleListUsers(List<User> users)
        {
            this.userList = users;

        }

        private object Recive(TcpClient tcpClient)
        {
            //binariza
            BinaryFormatter formatter = new BinaryFormatter();

            //lo recibe en stream del cliente
            NetworkStream stream = tcpClient.GetStream();

            //genera el objeto y los deserializa el stream recibido
            Object newObj = (Object)formatter.Deserialize(stream);

            return newObj;
            
        }

        //enviar
       public void Send(TcpClient tcpClient, Object objeto)
        {
            //obtiene el strem de red
            NetworkStream stream = tcpClient.GetStream();
            //binarizar 
            IFormatter formatter = new BinaryFormatter();

            //envia al cliente
            formatter.Serialize(stream, objeto);

        }

        //getters
    }
}
