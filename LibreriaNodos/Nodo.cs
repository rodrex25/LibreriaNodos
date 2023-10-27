using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serialization;

namespace LibreriaNodos
{
    public class Nodo
    {
        //atributos
        public Socket socketLsiten;

        public Thread listenThread;

        public Socket socketClient { get; set; }

        public List<User> userList;

        public List<Nodo> nodes;

        public LinkedList<Message> messages;

        public User localUser;

        public string fileDirectory;



        //constructor
        public Nodo(User user)
        {


            this.localUser = user;
            this.userList = new List<User>();
            this.nodes = new List<Nodo>();
            this.messages = new LinkedList<Message>();


        }


        public void start()
        {
            IPAddress localIp = IPAddress.Parse(this.localUser.getUserIpAdress());
            IPEndPoint endPoint = new IPEndPoint(localIp, this.getLocalUser().getUserPort());

            this.socketLsiten = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            socketLsiten.Bind(endPoint);

            socketLsiten.Listen();

            listenThread = new Thread(this.Listen);

            listenThread.Start();

           // this.tcpListener = new TcpListener(localIp, this.localUser.getUserPort());

            //el nodo escucha
           // tcpListener.Start();

        }
        public void stop()
        {
            this.socketLsiten.Close();
        }



        public void Listen()
        {


            Socket cliente;

            //si se conecta un usuario le hace un hilo


            while (true)
            {

                cliente = this.socketLsiten.Accept();
                this.listenThread = new Thread(this.HandleConn);
                this.listenThread .Start(cliente);

                //
                //TcpClient tcpClient = tcpListener.AcceptTcpClient();
                //si recibe un cliente maneja la conexion
                //ThreadPool.QueueUserWorkItem(this.HandleConn, tcpClient);
            }

        }

        //usuario que se conecta

        public void HandleConn(Object tcpClient)
        {
            

            Socket client = (Socket)tcpClient;
            Object recived;
            recived = this.Recive(client);

            if (recived != null)
            {
                if (recived is User)
                {
                    //handle new user 
                    this.HandleUser((User)recived, (Socket)tcpClient);


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
            else
            {
                Console.WriteLine("No se recibio nada");
            }
        }

        //handle new user

        //pendiente
        public void HandleUser(User user, Socket tcpCLient)
        {



            //verificar si existe en la lista actual
            User findUser = this.userList.Find(useri => useri.getUserIpAdress == user.getUserIpAdress);

            if (findUser == null)
            {
                //anadimos a lista de usuarios
                this.userList.Add(user);
                //creamos un nodo
                Nodo tempNodo = new Nodo(user);
                tempNodo.setTcpClient(tcpCLient);
                //añadimos a la lista de nodos 
                this.nodes.Add(tempNodo);
                //
                Console.WriteLine("Se ha conectado el usuario" + user.getUserName() + " ip: " + user.getUserIpAdress());


            }



        }
        public User FindUser(User user)
        {



            //verificar si existe en la lista actual
            User findUser = this.userList.Find(useri => useri.getUserIpAdress == user.getUserIpAdress);

            if (findUser != null)
            {

                return findUser;

            }

            return null;

        }


        //handle new message
        public void HandleMessages(Message message)
        {

            if (message is FileMessage)
            {
                try
                {


                    HandleFileMessage((FileMessage)message, fileDirectory);

                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }


            messages.AddLast(message);


        }

        public void HandleFileMessage(FileMessage fileMessage, String directorio)
        {

            string finalDirectory = directorio + fileMessage.getUserFrom();

            if (fileMessage != null && fileMessage.getFileData() != null)
            {

                if (!Directory.Exists(finalDirectory))
                {
                    Directory.CreateDirectory(finalDirectory);
                }

                string filePath = Path.Combine(finalDirectory, fileMessage.getMessageContent());

                File.WriteAllBytes(filePath, fileMessage.getFileData());

                Console.WriteLine("se guardo un archivo en: " + finalDirectory);

            }
            else
            {
                Console.WriteLine("no se guardo ni recibio nada valido" + finalDirectory);
            }

        }


        //handle list users update
        public void HandleListUsers(List<User> users)
        {
            this.userList = users;

        }

        public object Recive(Socket tcpClient)
        {
            byte[] buffer = new byte[1024];
            tcpClient.Receive(buffer);


            return BinarySerialization.Deserializate(buffer);


        }

        //enviar/



        public void Send(Socket tcpClient, Object objeto)
        {
            byte[] buffer = new byte[1024];
            byte[] obj = BinarySerialization.Serializate(objeto);

            Array.Copy(obj, buffer, obj.Length);

            tcpClient.Send(buffer);

            



        }

        //getters

        public User getLocalUser()
        {
            return this.localUser;
        }

        public Socket GetTcpClient()
        {
            return this.socketClient;
        }

        public List<User> GetUsers()
        {
            return this.userList;
        }

        //setters
        public void setTcpClient(Socket tcpClient)
        {
            this.socketClient = tcpClient;

        }

        public void setDirectory(string direction)
        {
            this.fileDirectory = direction;
        }


        public class CurrentAssemblyDeserializationBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                return Type.GetType(String.Format("{0}, {1}", typeName, Assembly.GetExecutingAssembly().FullName));
            }
        }



    }
}
