//Demonstrate Sockets
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using System.Threading;

class Server
{
    static Dictionary<string, string> studentLocation = new Dictionary<string, string> { }; // dictionary of strings set up called student location. 
    private string[] args;                      // argument array declared. 
    private string protocol = "whois";          // protocol defined and set to whois as default. 
    private StreamReader sr;                    // streamReader set to sr
    private StreamWriter sw;                    // stream writer set to sw.
    private string[] sections;                  // sections array declared
    private string[] newSections;               // newSections array declared. 
    private string newLine = String.Empty;      // new line declared as an empty string. 
    private string line = String.Empty;         // line declared as an empty string. 
    private string name = String.Empty;         // name declared as an empty string. 
    private string location = String.Empty;     // location declared as an empty string. 
    private TcpListener listener;               // TCPListener set to be called listner. 
    private Socket connection;                  // socket set to connection. 
    private NetworkStream socketStream;         // network stream set to socket stream. 
    /// <summary>
    /// Function sets up the server by initialising
    /// the arguments and calls the run server function.
    /// </summary>
    /// <param name="_args"></param>
    public Server(string[] _args)
    {
        args = _args;                           // initialises args. sets arguments to the arguments passed in as a parameter. 
        runServer();                            // calls the run sever function. 
    }
    private void runServer()
    {
        listener = new TcpListener(IPAddress.Any, 43);     // initialises listener to be set up with pot 43. 
        listener.Start();                                  // calls the start function of the TCPlistener. 
        Console.WriteLine("Server started Listening");     // output message. 
        while (true)                                       // loop to continue while true. 
        {
            try
            {
                connection = listener.AcceptSocket();       // when request is recieved created a socket to handle it. 
                connection.ReceiveTimeout = 1000;           //sets the time out value to 1 second. 
                connection.SendTimeout = 1000;
                Console.WriteLine("Connection Recieved");
                Thread t = new Thread(() => doRequest());   // initialises the thread and calls do request function.
                t.Start();                                  // starts the thread. 


            }
            catch (Exception e)
            {                                               // exception caught 
                Console.WriteLine("Exception while attempted to handle client:");   //error message displayed. 
                Console.WriteLine(e.ToString());            // outputs the exception. 
            }
        }
    }

    private void doRequest()
    {
        socketStream = new NetworkStream(connection);       // initialises socket stream 
        try
        {
            sw = new StreamWriter(socketStream);            // initialises the stream writer
            sr = new StreamReader(socketStream);            // initialises the stream reader
            seperateProtocols();                            // calls the seperate protocol function. 
            checkHTTPProtocol();                            // calls the check http protocol function. 
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception while attempted to handle client:");
            Console.WriteLine(e.ToString());
        }
        finally
        {
            socketStream.Close();           //socket no longer needed once request is complete. 
            connection.Close();
        }

    }
    private void seperateProtocols()
    {
        try
        {
            line = sr.ReadLine().Trim();                    // reads in the line by the stream reader and the trim function is called to remove all white space characters. 
            Console.WriteLine("Respond sent: " + line);     // writes the line read in. 
            sections = line.Split(new char[] { ' ' }, 2);   // sections is set to the line read in but is split into 2 sections where the space is. 
            string[] protocolSections = line.Split(new char[] { ' ' });     // Array of protocolSections defined and set to the line read in but then split. 

            if (protocolSections[0] == "GET" && protocolSections.Length == 2)   // if the 1st part of the line read in is GET and the length of the protocol sections is 2 then...
            {
                protocol = "h9G";               //sets the protocol to h9G.
            }
            else if (protocolSections[0] == "GET" && protocolSections[2] == "HTTP/1.0") // if the 1st part of the line read in is GET and the 2nd part matches "HTTP/1.0"...
            {
                protocol = "h0G";              // sets the protocol to h0G
            }
            else if (protocolSections[0] == "GET" && protocolSections[2] == "HTTP/1.1") // if the 1st part of the line read in is GET and the 2nd part matches "HTTP/1.1"... 
            {
                protocol = "h1G";             // sets the protocol to h1G
            }
            else if (protocolSections[0] == "PUT")  // if the 1st part of the line read in matches PUT...
            {
                protocol = "h9P";            // sets the protocol to h9P
            }
            else if (protocolSections[0] == "POST" && protocolSections[2] == "HTTP/1.0") // if the 1st part of the line read in matches "POST" and the 2nd part matches "HTTP/1.0"...
            {
                protocol = "h0P";           // sets the protocol to h0P
            }
            else if (protocolSections[0] == "POST" && protocolSections[2] == "HTTP/1.1")    // if the 1st part of the line read in matches "POST" and the 2nd matches "HTTP/1.1"...
            {
                protocol = "h1P";           // sets the protocol to h1p
            }
        }


        catch (IOException e)
        {
            Console.WriteLine("Error thrown: " + e);
        }

    }
    private void checkHTTPProtocol()
    {
        try
        {
            if (protocol == "whois")            // if the protocol matches whois...
            {
                whoIsProtocol();                // calls the whois protocol function. 
            }
            else if (protocol == "h0G")         // if the protocol matches h0G...
            {
                h0LookUp();                     // calls the h0LookUp function.  
            }
            else if (protocol == "h0P")         // if the protocol matches h0P...
            {
                h0Update();                     // calls the h0Update function. 
            }
            else if (protocol == "h1G")         // if the protocol matches h1G...
            {
                h1LookUp();                     // calls the h1LookUp function. 
            }
            else if (protocol == "h1P")         // if the protocol matches h1P...
            {
                h1Update();                     // calls the h1Update function. 
            }
            else if (protocol == "h9G")         // if the protocol matches h9G...
            {
                h9LookUp();                     // calls the h9LookUp function.  
            }
            else if (protocol == "h9P")         // if the protocol matches h9P...
            {
                h9Update();                     // calls the h9Update function. 
            }
        }
        catch (IOException e)
        {                                       // if the protocol doesnt match any of the http protocols then an exception is thrown and caught here. 
            Console.Write("Caught exception:" + e); // outputs error and the exception. 
        }
    }
    private void whoIsProtocol()
    {
        if (sections.Length > 1)                            // if the length of the sections is greater than 1 
        {
            if (studentLocation.ContainsKey(sections[0]))   // if the dictionary contains section position 1
            {
                studentLocation[sections[0]] = sections[1]; // set studentLocation position 1 to position 2
                sw.WriteLine("OK\r\n");                     // Writes ok to the stream writer.
                sw.Flush();                                 // flushes the stream to the file to clear the buffer.

            }
            else
            {
                studentLocation.Add(sections[0], sections[1]);  // add sections 1 and sections 2 to the dictionary. 
                sw.WriteLine("OK\r\n");                         // write ok to the stream writer
                sw.Flush();                                     // flushes the stream to the file to clear the buffer.
            }
        }
        else
        {
            if (studentLocation.ContainsKey(sections[0]))       // if the dictionary contains sections 0
            {

                sw.WriteLine(studentLocation[sections[0]]);     // write studentLocation position 1 to the stream writer. 
                sw.Flush();                                     // flushes the stream to the file to clear the buffer.
            }

            else
            {
                sw.WriteLine("ERROR: no entries found");        // otherwise write error message to the stream writer. 
                sw.Flush();
            }
        }
    }
    private void h0Update()
    {
        newLine = line.Replace("/", "");                                // replace the / with nothing 
        newSections = newLine.Split(new char[] { ' ' }, 2);             // newSections is set to the newline and split into 2 at the space. 
        name = newSections[1];                                          // name is set to position 0 in newSections. 
        name = name.Substring(0, name.Length - 7).Trim();               //Gets rid of HTTP1.0 from the end of the name
        string lineTwo = sr.ReadLine();                                 //Reads in a line and is set to string variable. 
        string lineThree = sr.ReadLine();
        string location = sr.ReadLine();                                // location is set to the 4th line read in from the stream reader. 
        if (studentLocation.ContainsKey(name))                          // if the dictionary contains the name read in...
        {
            studentLocation[name] = location;                           // student location of the person found is set to the location read in from the server. 
        }
        else
        {
            studentLocation.Add(name, location);                         // adds the name and location read in to the dictionary if it isn't already there. 
        }
        sw.WriteLine("HTTP/1.0 200 OK");
        sw.WriteLine("Content-Type: text/plain");                       // writes to the stream writer. 
        sw.WriteLine();
        sw.Flush();                                                     // flushes the stream to the file to clear the buffer.
    }
    private void h0LookUp()
    {
        newLine = line.Replace("/", "");                                //Replaces all the / with blank. 
        newSections = newLine.Split(new char[] { ' ' }, 2);             // splits newline up into 2 at the space.
        name = newSections[1];                                      
        name = name.Substring(1, name.Length - 8).Trim();               //Removes the '?' at the start and 'HTTP1.0' at the end in one go
        if (studentLocation.ContainsKey(name))                          //if the dictionary contains the name 
        {
            studentLocation.TryGetValue(name, out location);            // gets the location from the dictionary using the name 
            sw.WriteLine("HTTP/1.0 200 OK");                            // writes protocol response to the stream writer
            sw.WriteLine("Content-Type: text/plain");
            sw.WriteLine();                                             //writes a blank line 
            sw.WriteLine(location);                                     // writes the location to the stream writer. 
            sw.Flush();                                                 // flushes the stream to the file to clear the buffer.
        }
        else
        {                                                               // if the name cannot be found in the dictionary
            sw.WriteLine("HTTP/1.0 404 Not Found");
            sw.WriteLine("Content-Type: text/plain");
            sw.WriteLine();
            sw.Flush();                                                 // flushes the stream to the file to clear the buffer.
        }

    }
    private void h1LookUp()
    {
        newLine = line.Replace("/", "");                            // Replaces all "/" with blank 
        newSections = newLine.Split(new char[] { ' ' }, 2);         //splits the new line into 2 sections at the space. 
        name = newSections[1];                                      
        name = name.Substring(6);                                   //Remove '?name='
        name = name.Substring(0, name.Length - 7).Trim();           //Remove HTTP1.1
        if (studentLocation.ContainsKey(name))                      // if dictionary contains the name 
        {
            studentLocation.TryGetValue(name, out location);        // gets the location from the dictionary using the name
            sw.WriteLine("HTTP/1.1 200 OK");
            sw.WriteLine("Content-Type: text/plain");
            sw.WriteLine();
            sw.WriteLine(location);
            sw.Flush();                                             // flushes the stream to the file to clear the buffer.
        }
        else
        {                                                           // if the name cannot be found in the dictionary 
            sw.WriteLine("HTTP/1.1 404 Not Found");
            sw.WriteLine("Content-Type: text/plain");               // write protocol h/1.1 error 
            sw.WriteLine();
            sw.Flush();                                              // flushes the stream to the file to clear the buffer.
        }
    }
    private void h1Update()
    {
        newLine = line.Replace("/", "");                        // replaces all / with blank. 
        newSections = newLine.Split(new char[] { ' ' }, 2);     // split new line into 2 at the space. 
        name = newSections[1];
        string lineTwo = sr.ReadLine();                         // read in the second line. 
        string lineThree = sr.ReadLine();                       // reads in the third line. 
        string[] length = lineThree.Split(new char[] { ' ' });  // splits line 3 into 2 at the space. 
        char[] buffer = new char[Convert.ToInt32(length[1])];   
        sr.ReadBlock(buffer, 0, Convert.ToInt32(length[1]));    // reads the block because there is no end to the line. 
        string lineFour = new string(buffer);
        string[] newFour = lineFour.Split(new char[] { '&' }, 2); // splits line 4 into 2 at the &. 
        name = newFour[0].Replace("name=", "");                 // replaces name= with blank. 
        location = newFour[1].Replace("location=", "");         // replaces location= with blank. 

        if (studentLocation.ContainsKey(name))                  // if dictionary contains the name read in...
        {
            studentLocation[name] = location;                   // sets the dictionary with that name to the location. 
        }
        else
        {                                                       // if the person with that name cannot be found then...
            studentLocation.Add(name, location);                // add the name and location to the dictionary. 
        }
        sw.WriteLine("HTTP/1.1 200 OK");
        sw.WriteLine("Content-Type: text/plain");
        sw.WriteLine();
        sw.Flush();
    }
    private void h9LookUp()
    {
        newLine = line.Replace("/", "");                         //replaces all / with blank. 
        newSections = newLine.Split(new char[] { ' ' }, 2);     // splits new line into 2 sections at the space. 
        name = newSections[1];
        if (studentLocation.ContainsKey(name))                  // if the dictionary contains the person read in...
        {
            studentLocation.TryGetValue(name, out location);    // get the location from the dictionary by the name 
            sw.WriteLine("HTTP/0.9 200 OK");
            sw.WriteLine("Content-Type: text/plain");           // write the protocol 0.9 get message to the stream writer. 
            sw.WriteLine();
            sw.WriteLine(location);                             // write the location 
            sw.Flush();                                         // flushes the stream to the file to clear the buffer.
        }
        else
        {                                                       // if the person cannot be found in the dictionary
            sw.WriteLine("HTTP/0.9 404 Not Found");
            sw.WriteLine("Content-Type: text/plain");
            sw.WriteLine();
            sw.Flush();
        }
    }
    private void h9Update()
    {
        newLine = line.Replace("/", "");                        //replaces all / with blank. 
        newSections = newLine.Split(new char[] { ' ' }, 2);     // splits new line into 2 at the space. 
        name = newSections[1];
        string lineTwo = sr.ReadLine();                         // reads in the 2nd line
        string lineThree = sr.ReadLine();                       // reads in the 3rd line
        string location = lineThree;                            // sets location to the 3rd line read in 
        if (studentLocation.ContainsKey(name))                  // if the dictionary contains a person with the name read in...
        {
            studentLocation[name] = location;                   // sets that person with the location read in. 
        }
        else
        {                                                       //if the dictionary doesnt contain that person.
            studentLocation.Add(name, location);                // add the person and location to dictionary 
        }
        sw.WriteLine("HTTP/0.9 200 OK");
        sw.WriteLine("Content-Type:  text/plain");              // write protocol h/0.9 error to stream writer.
        sw.WriteLine();
        sw.Flush();                                             // flushes the stream to the file to clear the buffer.
    }

}
