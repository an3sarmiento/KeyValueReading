# Key Value Reading

## Description
This componenent allows the connection via REST Get web service to a systems which response is in a Key Value structure. The project is defined to read from structure like: 
``` XML
<rows>
  <row>   
    <it><nm>Attribute Name</nm><vl>Attribute Value</vl></it>
    <it><nm>Attribute Name</nm><vl>Attribute Value</vl></it>
  </row>
  <row>
    <it><nm>Attribute Name</nm><vl>Attribute Value</vl></it>
    <it><nm>Attribute Name</nm><vl>Attribute Value</vl></it>  
  </row>
</rows>
```

## Component Code
Setup service location
``` c#
string url = "http://localhost:8081";
string path ="/listUsers";
```
Create a new object and get the response
``` c#
XMLReader Reader = new XMLReader(url, path);
int XMLSize = Reader.GetClients();
```
If the number of items is *-1* there was a problem with the invocation, for example, a timeout. Then for each of the clients any attribute can be requested using the attribute name:
``` c#
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
}
```

## How to be used from Bizagi

Defines the URL and service variables to be used 
``` c#
var URL = CHelper.getParameterValue("URL_ESB") + "/QueryRestApiS";

var Service = "/CORE_CLIENTS_CONSENT_NEW_EOD/setUpDate/" + CHelper.FormatDate(<M_UpdateCustomerRequests.dRequestedDate>, "yyyyMMdd");
```
Then an object is instantiated by using the previously defined URL:
``` c#
//Instanciates the object with the address and parameters
var XMLReaderVar = new XMLReader.XMLReader(URL,Service);
```

Once the process gets a response returns initially the number of clients. If the response is ‘-1’ it means that there was an error on
the execution and an error message will be displayed.
``` c#
//Gets the number of clients avaiable for those parameters
varClientsSize = XMLReaderVar.GetClients();
```

Then, for each of the clients, we are going to read its attribute we can get the value for an attribute of a client by using the function getAttribute(int ClientPosition, string AttributeName):
 ``` c#
//Gets the attribute X of that client
var CoreId = XMLReaderVar.getAttribute(i, "Attribute_Name");
```

