using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;

namespace location
{
    public class whois
    {
        static void Main(string[] args)
        {
            Client ClientInstance = new Client(args); // makes an instance of the client class. 
        }
    }    
           
}



