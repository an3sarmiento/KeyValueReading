using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;

namespace XMLReader
{
    class SOAPUtil
    {
        public static string Call(string url, string soapAction, string soapMessage)
        {
            HttpWebRequest webRequest = CreateWebRequest(url, soapAction);

            return InvokeWS(webRequest, soapMessage);
        }

        public static string Call(string url, string domain, string userName, string password, string soapAction, string soapMessage)
        {
            HttpWebRequest webRequest = CreateWebRequest(url, soapAction);

            AddAuthentication(webRequest, domain, userName, password);

            return InvokeWS(webRequest, soapMessage);
        }

        public static string InvokeWS(HttpWebRequest webRequest, string soapMessage)
        {
            SendRequest(soapMessage, webRequest);
            return ReceiveResult(webRequest);
        }

        private static string ReceiveResult(HttpWebRequest webRequest)
        {
            // begin async call to web request.
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            // suspend this thread until call is complete. You might want to
            // do something usefull here like update your UI.
            asyncResult.AsyncWaitHandle.WaitOne();

            // get the response from the completed web request.
            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
            }
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(soapResult);
            XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
            nsManager.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            XmlNode node = doc.SelectSingleNode("s:Envelope/s:Body", nsManager);

            if (node.FirstChild != null && node.FirstChild.FirstChild != null)
            {
                string result = node.FirstChild.FirstChild.InnerXml;
                result = XMLHelper.UnescapeXml(result);
                return result;
            }
            else
            {
                return "";
            }
        }


        private static void SendRequest(string soapMessage, HttpWebRequest webRequest)
        {
            string soapEnvelopeXml = CreateSoapEnvelope(soapMessage);

            Console.Write(soapEnvelopeXml);

            using (Stream stream = webRequest.GetRequestStream())
            {
                byte[] b = UTF8Encoding.UTF8.GetBytes(soapEnvelopeXml);
                stream.Write(b, 0, b.Length);
            }
        }

        public static HttpWebRequest CreateWebRequest(string url, string soapAction)
        {
            HttpWebRequest webRequest = CreateHttpWebRequest(url, soapAction);

            return webRequest;
        }

        public static void AddAuthentication(HttpWebRequest webRequest, string domain, string username, string password)
        {
            webRequest.Credentials = new NetworkCredential(username, password, domain);
        }

        private static HttpWebRequest CreateHttpWebRequest(string url, string soapAction)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

            webRequest.Headers.Add("SOAPAction", soapAction);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";

            return webRequest;
        }

        private static string CreateSoapEnvelope(string soapMessage)
        {
            string[] envelop = new string[] {
                "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\">",
                "   <soapenv:Header/>",
                "   <soapenv:Body>",
                soapMessage,
                "   </soapenv:Body>",
                "</soapenv:Envelope>"
            };
            string fullSoapMessage = string.Join("\n", envelop);

            return fullSoapMessage;
        }

        public static void SetTimeout(HttpWebRequest web, int timeout)
        {
            web.Timeout = timeout;
        }
    }
}
