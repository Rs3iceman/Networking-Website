using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;

namespace location
{

    class Client
    {
        private TcpClient myClient;                     //sets the TCP client to myClient. 
        private List<string> list;                      // creates a list called list. 
        private string name = String.Empty;             // name declared as an empty string. 
        private string location = String.Empty;         // location declared as an empty string. 
        private string[] args;                          // string of arguments declared.  
        private string server = "whois.net.dcs.hull.ac.uk";            // sets server to local host. 
        private int port = 43;                          // sets the port to 43
        private string protocol = "whois";              // protocol set to whois as a default protocol. 
        private StreamWriter sw;                        // stream writer set to sw.
        private StreamReader sr;                        // stream reader set to sr. 
        private string serverResponse = String.Empty;   //server response declared as an empty string. 

        /// <summary>
        /// Function takes in arguemnts and these arguments are set to an arguemnt variable 
        /// this function then calls the function to start running the client 
        /// </summary>
        /// <param name="_args"></param>
        public Client(string[] _args)
        {
            args = _args;                           // args is set to the arguments passed in as a parameter. 
            runClient();                            // calls the runClient function. 
        }
        /// <summary>
        /// Function initialises stream reader and writer and the client.
        /// It then calls the functions for the rest of the client program. 
        /// </summary>
        private void runClient()
        {
            myClient = new TcpClient();                     //Initialises myClient TCPClient  
            argumentSplitter();                             // calls the artgument splitter function. 
            myClient.Connect(server, port);                 // connects the client to the whois at port 43
            myClient.ReceiveTimeout = 1000;                 
            myClient.SendTimeout = 1000;
            sw = new StreamWriter(myClient.GetStream());    // initialises the stream writer and sets it to the variable sw.
            sr = new StreamReader(myClient.GetStream());    // initialises the strea reader and sets it to the variable rw.
            checkLength();                                  //calls the checkLength function. 
            checkHTTPProtocol();                            // calls the check http protocol function. 
        }
        /// <summary>
        /// Function loops through the list of arguments and splits the arguemnts 
        /// into protocol /h,/p,/h0, /h0, /h1
        /// </summary>
        private void argumentSplitter()
        {
            list = new List<string>(args);          // creates a list of arguemnts

            for (int i = 0; i < list.Count; i++)    // lops through the list of arguemnts until the end of the list. 
            {
                if (list[i] == "/h")                // if the arguemnt in the list has the protocol /h then... 
                {
                    server = list[i + 1];           
                    list.RemoveAt(i);               // removes the argument at position i in the list from the list to remove /h. 
                    list.RemoveAt(i);               // removes the argument at position i in the list to remove the remaining argument. 
                    i = -1;                         
                    continue;
                }
                else if (list[i] == "/p")           // if the argument in the list has the protocol /p then.. 
                {
                    port = int.Parse(list[i + 1]);  // set port to the converted integer of the argument in the list plus 1.
                    list.RemoveAt(i);               // removes the argument at position i top remove /p
                    list.RemoveAt(i);               // removes the argument at position i to remove the remaining argument.
                    i = -1;
                    continue;
                }
                else if (list[i] == "/h0")          // if the arguemnt in the list has the protocol /h0
                {
                    protocol = "h0";                // set the protocol variable to h0
                    list.RemoveAt(i);               // remove argument at position i from the list to remove /h0
                    i = -1;
                    continue;
                }
                else if (list[i] == "/h1")          //if the argument in the list has the protocol /h1 then...
                {
                    protocol = "h1";                // set the protocolo variable to h1
                    list.RemoveAt(i);               // remove argument at position i from the list to remove /h1
                    i = -1;
                    continue;
                }
                else if (list[i] == "/h9")          //if the argument in the list has the protocol /h9 then...
                {
                    protocol = "h9";                // sets the protocol variable to h9
                    list.RemoveAt(i);               // removes argument at position i from the list to remove the h9
                    i = -1;
                    continue;
                }
            }
        }
        /// <summary>
        /// Goes through the list of arguemnts and checks the length 
        /// of each argument to determine the input. 
        /// </summary>
        private void checkLength()
        {
            switch (list.Count)                                 // goes through the list of arguments. 
            {
                case 0:                                         // if no arguments are found...
                    Console.WriteLine("No arguments provided"); // error message is displayed stating no arguments were provided. 
                    break;                                      // breaks out of the case. 
                case 1:                                         // if only one argument is provided...
                    name = list[0];                             // name is set to that arguemnt. 
                    break;
                case 2:                                         // if two arguments are found...
                    name = list[0];                             // the first argument is set to the name 
                    location = list[1];                         // the second argument is set to the list. 
                    break;
                default:                                        // if any other number of arguments are provided...
                    Console.WriteLine("Invalid argument length.");  // error message is displayed. 
                    break;
            }
        }

        /// <summary>
        /// Function checks to see what the protocol is set to
        /// then calls the corresponding function. 
        /// </summary>
        private void checkHTTPProtocol()
        {
            try
            {
                if (protocol == "whois")        // if the protocol is set to whois...
                {
                    whoIsProtocol();            // call the whois function. 
                }
                else if (protocol == "h0")      // if the protocol is set to h0...
                {
                    h0Protocol();               // call the h0 protocol function. 
                }
                else if (protocol == "h1")      // if the protocol is set to h1...
                {
                    h1Protocol();               // call the h1protocol function. 
                }
                else if (protocol == "h9")      // if the protocol is set h9...
                {
                    h9Protocol();               // call the h9 protocol function. 
                }
            }
            catch (IOException e)               // if none of the if statements are reached and exception is thrown and caught here. 
            {
                Console.Write("Caught exception:" + e); // error message displays stating an exception has been thrown and states the exception. 
            }

        }
        /// <summary>
        /// Function checks the arguemnts provided for the whois protocol, reads the servers response and
        /// displays the appropriate message. 
        /// </summary>
        private void whoIsProtocol()
        {
            
            if (list.Count == 1)                                        // goes through the list until the end of the list and if there is only 1 arguemnt in the list. 
            {
                sw.WriteLine(name);                                     // writes name to the stream writer.  
                sw.Flush();                                             // flushes the name in the stream to the file to clear the buffer. 
                serverResponse = sr.ReadLine();                         // reads the line in from the stream reader. 
                if (location == "ERROR: no entries found")              // if the location sent from the server is that no entries can be found..
                {
                    Console.WriteLine("ERROR: no entries found");       // error message is displayed. 
                }
                else
                {
                    Console.WriteLine(name + " is " + serverResponse);     // otherwise output the name and the location of that person by displaying the response. 
                }
            }
            else if (list.Count == 2)                                    // goes through the list until the end and if there are 2 arguments in the list..
            {
                sw.WriteLine(name + " " + location);                    // writes the name space and location to the stream writer. 
                sw.Flush();                                             // flushes the writeline in the stream to the file to clear the buffer. 
                if (sr.ReadLine().Trim() == "OK")                       // reads the line from the stream reader and if it says ok...
                {
                    Console.WriteLine(name + " location changed to be " + location);    // display the name and the location. 
                }
                else
                {
                    Console.WriteLine("unable to update");              // otherwise outputs error message. 
                }
            }
            else
            {
                Console.WriteLine("failure, too many argurments");     // if 1 or 2 arguemnts are not provided error message is displayed.
            }
        }
        /// <summary>
        /// Goes through the arguments for the h/1.0 protocol to determine
        /// whether it is a GET or POST and outputs the correct responses. 
        /// </summary>
        private void h0Protocol()
        {
            if (list.Count == 1)                                        // goes through thee list and if 1 argument is found in the list...
            {
                sw.Write("GET /?" + name + " HTTP/1.0\r\n\r\n");        // writes the protocol request for /h1.0 to the stream writer. 
                sw.WriteLine();                                         // writes a line to the stream writer. 
                sw.Flush();                                             // flushes the request written in the stream to the file to clear the buffer. 
                serverResponse = sr.ReadLine();                         // reads the servers response from the stream reader and sets the variable serverResponse to that. 
                if (serverResponse == "HTTP/1.0 404 Not Found")         // if the server responds with this string
                {
                    Console.WriteLine("ERROR: No Entries Found");       // outputs error message
                }
                else
                {                                                       // otherswise...
                    Console.WriteLine(name + " is " + serverResponse);  //outputs the name and where that person is. 
                    Console.WriteLine(sr.ReadToEnd());                  // reads to the end of the line read in. 
                }
            }
            else if (list.Count == 2)                                   // goes through the list and if 2 arguments are found in the list...
            {
                int length = location.Length;                           // sets length to the length of the location argument.
                sw.WriteLine("POST /" + name + " HTTP/1.0");            // writes post the persons name and protocol to the stream writer. 
                sw.WriteLine("Content-Length: " + length);              //writes the message and the length.
                sw.WriteLine();                                         // writes a blank line. 
                sw.WriteLine(location);                                 // writes the location. 
                sw.Flush();                                             // flushes the request written in the stream to the file to clear the buffer. 
                serverResponse = sr.ReadLine();                         // server response is set to the line read in the stream reader.
                if (serverResponse == "HTTP/1.0 200 OK")                // if the server responds with this string then...
                {
                    Console.WriteLine(sr.ReadToEnd());                  // reads to the end of the stream reader and outputs this. 
                }
                else
                {                                                       // otherwise...
                    Console.WriteLine("Unable to update location");     // outputs an error message. 
                }
            }
            else
            {
                Console.WriteLine("Too many arguments provided");       // if more than 2 arguemnts are provided then an error message is displayed. 
            }
        }
        /// <summary>
        /// Goes through the arguments for the h/1.1 protocol to determine whether it is a GET or POST and outputs the 
        /// correct response. 
        /// </summary>           
        private void h1Protocol()
        {
            int stringLength = 15 + name.Length + location.Length;                          // string length is set to the length of the name + length of location + length of "name=&location="
            if (list.Count == 1)                                                            // goes through the list and if 1 argument is found in the list...
            {
                sw.Write("GET /?name=" + name + " HTTP/1.1\r\nHost: " + server + "\r\n\r\n");   // writes the 1st part of the string, the persons name, the second part of the string and the server name.  
                sw.Flush();                                                                     // flushes the request written in the stream to the file to clear the buffer. 
                serverResponse = sr.ReadLine();                                                 // set serverResponse to the line read in by the stream reader from the server. 
                if (serverResponse == "HTTP/1.1 404 Not Found")                                 // if the line read in was this string...
                {
                    Console.WriteLine("ERROR: No Entries Found");                               // output error message. 
                }
                else
                {                                                                               // otherwise...
                    Console.WriteLine(name + " is " + serverResponse);                          // writes the name of the person and the servers response to where they are. 
                    Console.WriteLine(sr.ReadToEnd());
                }
            }
            else if (list.Count == 2)                                                           // goes through the list and if 2 arguments are found in the list...
            {
                sw.Write("POST / HTTP/1.1\r\nHOST: " + server + "\r\n" + "Content-Length: " + stringLength + "\r\nname=" + name + "&location=" + location); // writes a string inc the server name, string length, name and location. 
                sw.WriteLine();                                                                 // writes an empty line.          
                sw.Flush();                                                                     // flushes the request written in the stream to the file to clear the buffer. 
                serverResponse = sr.ReadLine();                                                 // serverResponse set to the line read in by the stream reader from the server. 
                if (serverResponse == "HTTP/1.1 200 OK")                                        // if the line read in matches the string...
                {
                    Console.WriteLine(sr.ReadToEnd());                                          // reads to the end of the stream reader and writes it. 
                }
                else
                {   
                    Console.WriteLine("Unable to update location");                             // else error mesage displayed. 
                }
            }
            else
            {                                                                                   // if more than 2 arguments are provided... 
                Console.WriteLine("Too many arguments provided");                               // error message provided. 
            }
        }
        /// <summary>
        /// /Goes through the arguments for the h/0.9 protocol to determine whether it is a GET or POST and outputs the 
        /// correct response.
        /// </summary>
        private void h9Protocol()
        {
            if (list.Count == 1)                                        // goes through the list and if one argument if found... 
            {
                sw.WriteLine("GET" + " /" + name);                      // writes get/ then the name of the person to the stream writer. 
                sw.WriteLine();                                         //writes an empty line 
                sw.Flush();                                             // flushes the request written in the stream to the file to clear the buffer. 
                serverResponse = sr.ReadLine();                         // serverResponse set to the line read from the stream reader which is the server responding. 
                if (serverResponse == "HTTP/0.9 404 Not Found")         // if the server responds with this string. 
                {
                    Console.WriteLine("ERROR: No Entries Found");       // output error message. 
                }
                else
                {                                                       // otherwise...
                    Console.WriteLine(name + " is " + serverResponse);  // write the name of the person is in the location.
                    Console.WriteLine(sr.ReadToEnd());                  // output what the stream reader reads to the end of the stream. 
                }
            }
            else if (list.Count == 2)                                   // goes through the list and if two arguments are found...
            {
                sw.WriteLine("PUT /" + name);                           // write a line that says put/ and the persons name to the stream writer. 
                sw.WriteLine();                                         // writes an empty line. 
                sw.WriteLine(location);                                 // writes the persons location. 
                sw.Flush();                                             // flushes the request written in the stream to the file to clear the buffer.
                serverResponse = sr.ReadLine();                         // serverResponse is set to the line read from the stream reader from the server. 
                if (serverResponse == "HTTP/0.9 200 OK")                // if the line read in is this string...
                {
                    Console.WriteLine(sr.ReadToEnd());                  // read to the end of the stream reader and write this. 
                }
                else
                {                                                       // otherwise... 
                    Console.WriteLine("Unable to update location");     // write error message. 
                }
            }
            else
            {                                                           // if more than 2 arguments are provided then.. 
                Console.WriteLine("Too many arguments provided");       // error message is displayed. 
            }

        }
    }
}
