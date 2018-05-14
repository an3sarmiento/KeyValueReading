using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Collections;

namespace XMLReader
{
    public class XMLReader
    {
        private string url;
        private string parameters;
        private List<Client> clients;

        public XMLReader(string url, string parameters)
        {
            this.url = url;
            this.parameters = parameters;
            this.clients = new List<Client>();
        }

        public int GetClients()
        {
            using (var client = new HttpClient())
            {

                try
                {
                    var MyURL = url + parameters;
                    var response = client.GetAsync(MyURL).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        // by calling .Result you are performing a synchronous call
                        var responseContent = response.Content;

                        // by calling .Result you are synchronously reading the result
                        string responseString = responseContent.ReadAsStringAsync().Result;
                        
                        List<string> findings = BetweenBrackets(responseString, "row");

                        foreach(string find in findings)
                            clients.Add(new Client(find));

                        return findings.Count;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    return -1;
                }
            }
            return 0;
        }

        public static List<string> BetweenBrackets(string text, string attrib)
        {
            List<string> findings = new List<string>();

            Regex regex = new Regex(@"(<" + attrib + ">(.*?)</" + attrib + ">)", RegexOptions.Singleline);
            MatchCollection matches = regex.Matches(text);

            foreach (Match match in matches)
            {
                Console.WriteLine(match);
                findings.Add(match.Groups[2].Value);
            }

            return findings;
        }

        public string getAttribute(int Client, string attribute)
        {
            return clients[Client].GetAttrib(attribute);
        }

        static void Main(string[] args)
        {
            // Display the number of command line arguments:
            string url = "http://localhost:8081";
            string path ="/listUsers";
            int clientToPrint = 0;
            string attribute = "attribute_1";
            if(args.Length >= 4)
            {
                url = args[0];
                path = args[1];
                clientToPrint = int.Parse(args[2]);
                attribute = args[3];
            }

            XMLReader Reader = new XMLReader(url, path);
            int XMLSize = Reader.GetClients();
            Console.WriteLine("Number of clients: " + XMLSize);
            if (XMLSize >= 0)
            {
                for (int i = 0; i != XMLSize; i++)
                {
                    Console.WriteLine("Result Client (" + i + "): " + Reader.getAttribute(i, attribute));
                }
            }
            else
            {
                Console.WriteLine("TIME OUT!");
                Console.WriteLine("TIME OUT!");
            }
        }
    }

    public class Client
    {
        private string row;
        Hashtable myHashTable;

        public string GetAttrib(string attrib)
        {
            return myHashTable.ContainsKey(attrib)? (string)myHashTable[attrib]:null;
        }

        public Client(string row)
        {
            this.row = row;
            this.myHashTable = new Hashtable(); ;

            List<string> findings = XMLReader.BetweenBrackets(row,"it");
            foreach(string find in findings)
            {
                string _nm = XMLReader.BetweenBrackets(find, "nm")[0];
                string _val = XMLReader.BetweenBrackets(find, "vl")[0].Trim();

                string response = "nm: " + _nm + ", vl: " + _val;
                Console.WriteLine(response);
                myHashTable.Add(_nm, _val);
            }
        }
    }
}
