using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;

namespace WmlValidatorConsole
{
    /// <summary>
    /// 读取本地dtd文件类
    /// 用来替换wml文件中的dtd引用，避免需从网上下载dtd验证，导致执行速度过慢
    /// </summary>
    public class XmlLocalResolver : XmlResolver
    {
        private string dtdFilePath = "";
        public XmlLocalResolver(string dtdPath)
        {
            this.dtdFilePath = dtdPath;
        }

        public override System.Net.ICredentials Credentials
        {
            set { throw new NotImplementedException(); }
        }

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (ofObjectToReturn == null || ofObjectToReturn == typeof(Stream) || ofObjectToReturn == typeof(object))
            {
                var content = File.ReadAllText(dtdFilePath);
                byte[] byteArray = Encoding.UTF8.GetBytes(content);
                return new MemoryStream(byteArray);
            }
            throw new XmlException("Xml_UnsupportedClass");
        }
    }
}
