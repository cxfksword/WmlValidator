using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Net;

namespace WmlValidatorConsole
{
    /// <summary>
    /// wml格式完整性检查类
    /// </summary>
    public class WmlValidator
    {

        /// <summary>
        /// 验证wml字符串
        /// </summary>
        /// <param name="wmlstr"></param>
        /// <returns></returns>
        public static ResultMessage Verify(string wmlstr)
        {
            var preErrorElement = "";
            var errorMessage = "";
            try
            {
                using (StringReader str = new StringReader(wmlstr))
                {
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.ProhibitDtd = false;
                    settings.ValidationType = ValidationType.None;
                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wml_1.1.dtd");
                    settings.XmlResolver = new XmlLocalResolver(path);
                    using (var reader = XmlReader.Create(str, settings))
                    {
                        while (reader.Read())
                        {
                            var attrstr = "";
                            switch (reader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    for (var i = 0; i < reader.AttributeCount; i++)
                                    {
                                        reader.MoveToAttribute(i);
                                        attrstr += string.Format(" {0}=\"{1}\"", reader.Name, reader.Value);
                                    }
                                    reader.MoveToElement();
                                    preErrorElement += string.Format("<{0}{1}>", reader.Name, attrstr);
                                    break;
                                case XmlNodeType.Text:
                                    preErrorElement += reader.Value;
                                    break;
                                case XmlNodeType.CDATA:
                                    preErrorElement += string.Format("<![CDATA[{0}]]>", reader.Value);
                                    break;
                                case XmlNodeType.ProcessingInstruction:
                                    preErrorElement += string.Format("<?{0} {1}?>", reader.Name, reader.Value);
                                    break;
                                case XmlNodeType.Comment:
                                    preErrorElement += string.Format("<!--{0}-->", reader.Value);
                                    break;
                                case XmlNodeType.XmlDeclaration:
                                    preErrorElement += string.Format("<?xml {0}?>", reader.Value);
                                    break;
                                case XmlNodeType.Document:
                                    break;
                                case XmlNodeType.DocumentType:
                                    for (var i = 0; i < reader.AttributeCount; i++)
                                    {
                                        reader.MoveToAttribute(i);
                                        switch (reader.Name)
                                        {
                                            case "PUBLIC":
                                                attrstr += string.Format(" {0} \"{1}\"", reader.Name, reader.Value);
                                                break;
                                            case "SYSTEM":
                                                attrstr += string.Format(" \"{0}\"", reader.Value);
                                                break;
                                        }
                                    }
                                    reader.MoveToElement();
                                    preErrorElement += string.Format("<!DOCTYPE {0} {1}", reader.Name, attrstr);
                                    break;
                                case XmlNodeType.EntityReference:
                                    preErrorElement += reader.Name;
                                    break;
                                case XmlNodeType.EndElement:
                                    preErrorElement += string.Format("</{0}>", reader.Name);
                                    break;
                                case XmlNodeType.Attribute:
                                    preErrorElement += reader.Value;
                                    break;
                                case XmlNodeType.Whitespace:
                                    preErrorElement += reader.Value;
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }


            return new ResultMessage()
            {
                Success = string.IsNullOrEmpty(errorMessage),
                ErrorMessage = errorMessage,
                ErrorPosition = preErrorElement + "[]<--"
            };
        }

        /// <summary>
        /// 验证wml文件
        /// </summary>
        /// <param name="wmlFilePath"></param>
        /// <returns></returns>
        public static ResultMessage VerifyFile(string wmlFilePath)
        {
            return Verify(File.ReadAllText(wmlFilePath));
        }

        /// <summary>
        /// 验证指定地址的wml格式完整性
        /// </summary>
        /// <param name="url">需验证的网址</param>
        /// <returns></returns>
        public static ResultMessage VerifyFromUrl(string url)
        {
            var content = "";
            var req = WebRequest.Create(url);
            req.Timeout = 5000;
            using (var response = req.GetResponse())
            {
                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);
                content = sr.ReadToEnd();
            }
            return Verify(content);
        }
    }

    /// <summary>
    /// 返回消息
    /// </summary>
    public class ResultMessage
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success;

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage;

        /// <summary>
        /// 出错字符位置
        /// </summary>
        public string ErrorPosition;
    }
}
