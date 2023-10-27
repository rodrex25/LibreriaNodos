using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LibreriaNodos
{
    [Serializable]
    public class Message
    {
        //atributos
        public string userFrom;
        public string userTo;
        public string messageContent;


        //constructor
        public Message(string userFrom, string userTo, string messageContent) {

            this.userFrom = userFrom;
            this.userTo = userTo;
            this.messageContent = messageContent;

        }

        //metodos 

        //getters
        public string getUserFrom() {
            return userFrom;
        }
        public string getUserTo()
        {
            return userTo;
        }
        public string getMessageContent()
        {
            return messageContent;
        }
        //setters

       

    }
}
