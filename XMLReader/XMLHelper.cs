using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLReader
{
    class XMLHelper
    {
        public static string UnescapeXml(string s)
        {
            string unxml = s;
            if (!string.IsNullOrEmpty(unxml))
            {
                // replace entities with literal values
                unxml = unxml.Replace("&apos;", "'");
                unxml = unxml.Replace("&quot;", "\"");
                unxml = unxml.Replace("&gt;", ">");
                unxml = unxml.Replace("&lt;", "<");
                unxml = unxml.Replace("&amp;", "&");
            }
            return unxml;
        }


        public static string EscapeXml(string s)
        {
            string unxml = s;
            if (!string.IsNullOrEmpty(unxml))
            {
                // replace entities with literal values
                unxml = unxml.Replace("&", "&amp;");
                unxml = unxml.Replace("'", "&apos;");
                unxml = unxml.Replace("\"", "&quot;");
                unxml = unxml.Replace(">", "&gt;");
                unxml = unxml.Replace("<", "&lt;");
            }
            return unxml;
        }
    }
}
