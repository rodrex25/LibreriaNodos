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
        public string filePath;
        public byte[] fileData;
       
        //constructor
        public FileMessage(string userFrom, string userTo, string messageContent, string filePath) : base(userFrom, userTo, messageContent) { 

            this.filePath = filePath;
            this.fileData = File.ReadAllBytes(this.filePath);
        
        }

        //getters
        public string getFilePath() { 

            return filePath;
            
        }

        public byte[] getFileData()
        {
            return fileData;
        }



    }
}
