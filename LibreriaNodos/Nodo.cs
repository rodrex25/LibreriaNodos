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

namespace LibreriaNodos
{
    public  class Nodo
    {
        //atributos
        protected TcpListener tcpListener;

        protected TcpClient tcpClient { get; set; }

        protected List<User> userList;

        protected List<Nodo> nodes;

        protected LinkedList<Message> messages;

        protected User localUser;

        protected string fileDirectory;

       

        //constructor
        public Nodo(User user) {

            
            this.localUser = user;
            this.userList = new List<User>();
            this.nodes = new List<Nodo>();
            this.messages = new LinkedList<Message>();


        }
       

        public void start()
        {
            IPAddress localIp = IPAddress.Parse(this.localUser.getUserIpAdress());

            this.tcpListener = new TcpListener(localIp, this.localUser.getUserPort());

            //el nodo escucha
            tcpListener.Start();

        }
        public void stop()
        {
            tcpListener.Stop();
        }
         
        

        public void Listen()
        {
            

           

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

       protected void HandleConn(Object tcpClient) {

            Object recived = Recive((TcpClient)tcpClient);

            if (recived != null)
            {
                if (recived is User)
                {
                    //handle new user 
                    this.HandleUser((User)recived, (TcpClient)tcpClient);

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
        protected void HandleUser(User user, TcpClient tcpCLient) {



            //verificar si existe en la lista actual
            User findUser = this.userList.Find(useri => useri.getUserIpAdress == user.getUserIpAdress);

            if (findUser != null)
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
        protected User FindUser(User user)
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
        protected void HandleMessages(Message message) {

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

        public void HandleFileMessage(FileMessage fileMessage, String directorio) {

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
            else {
                Console.WriteLine("no se guardo ni recibio nada valido" + finalDirectory);
            }
            
        }


        //handle list users update
        protected void HandleListUsers(List<User> users)
        {
            this.userList = users;

        }

        protected object Recive(TcpClient tcpClient)
        {
            //binariza
            BinaryFormatter formatter = new BinaryFormatter();

            //lo recibe en stream del cliente
            NetworkStream stream = tcpClient.GetStream();

            formatter.Binder = new CurrentAssemblyDeserializationBinder();

            //genera el objeto y los deserializa el stream recibido
            Object newObj = (Object)formatter.Deserialize(stream);

            return newObj;

        }

        //enviar
        protected void Send(TcpClient tcpClient, Object objeto)
        {

            
            //obtiene el strem de red
            NetworkStream stream = tcpClient.GetStream();

            //binarizar 
            IFormatter formatter = new BinaryFormatter();
           
            //envia al cliente
            formatter.Serialize(stream, objeto);
            
            

        }

        //getters

        public User getLocalUser()
        {
            return this.localUser;
        }

        public TcpClient GetTcpClient()
        {
            return this.tcpClient;
        }
        //setters
        public void setTcpClient(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;

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
