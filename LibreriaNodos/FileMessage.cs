using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaNodos 
{
    [Serializable]
    public class FileMessage : Message
    {

        //atributos
        private string filePath;
        //constructor
        public FileMessage(string userFrom, string userTo, string messageContent, string filePath) : base(userFrom, userTo, messageContent) { 

            this.filePath = filePath;
        
        }

        //getters
        public string getFilePath() { 

            return filePath;
            
        }



    }
}
