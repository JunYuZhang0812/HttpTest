using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Server
{
    public class ServerCfgUtil
    {
        private static Regex m_regIP = new Regex(@"(?<=^IP)(\d|\.)+");
        private static XmlOp m_ipNameXml;
        public static XmlOp IPNameXml
        {
            get
            {
                if( m_ipNameXml == null )
                {
                    m_ipNameXml = new XmlOp("Server_IP_Name");
                }
                return m_ipNameXml;
            }
        }
        public static void AddIP( string ip )
        {
            var key = "IP" + ip;
            var ele = IPNameXml.GetXmlElement(key);
            if (ele != null) return;
            IPNameXml.AddXmlElement(key,ip);
        }
        public static string GetIP( string ip)
        {
            return m_regIP.Match(ip).Value;
        }
        public static string GetIPName( string ip )
        {
            var key = "IP" + ip;
            return IPNameXml.GetXmlElementValue(key);
        }
    }
}
