using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using Microsoft.VisualBasic;

namespace Londeck.Net.Data.DAL
{
    using Microsoft.VisualBasic;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;

    class xsltTransform
    {
        public XmlDocument HTMLtoXML(string HTML)
        {
            //HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            //htmlDoc.LoadHtml(HTML);
            //htmlDoc.OptionOutputAsXml = true;
            XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.LoadXml(htmlDoc.DocumentNode.OuterHtml.ToString());
            //return xmlDoc;
            return xmlDoc;
        }

        //public static string TransformFile(string XMLText, string XSLText, string Path)
        //public static void TransformFile(System.IO.FileInfo XMLText,
        //    string XSLText,
        //    System.IO.FileInfo fi)
        //{
        //    System.IO.FileStream XmlR = new System.IO.FileStream(XMLText.FullName, System.IO.FileMode.Open);
        //    try
        //    {
        //        TextReader trXSL = new StringReader(XSLText);
        //        XmlTextReader trXSLRead = new XmlTextReader(XmlR);
        //        StringReader XSLStyleSheet = new StringReader(XSLText);
        //        XmlTextReader ssReader = new XmlTextReader(XSLStyleSheet);

        //        XmlWriterSettings settings = new XmlWriterSettings();
        //        settings.Indent = true;

        //        System.Xml.XmlWriter xmlwriter = System.Xml.XmlWriter.Create(fi.FullName, settings);
        //        XslCompiledTransform xslt = new XslCompiledTransform();
        //        XsltArgumentList xslArgs = new XsltArgumentList();

        //        //create custom object
        //        CustomXsltProcessor hmc = new CustomXsltProcessor();

        //        //pass an instance of the custom object
        //        xslArgs.AddExtensionObject("urn:HMCustomCode", hmc);

        //        xslt.Load(ssReader);
        //        xslt.Transform(trXSLRead, xslArgs, xmlwriter);
        //        xmlwriter.Flush();
        //        xmlwriter.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public string Transform(string XMLText, XsltArgumentList args, string XSLText)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                TextReader trXmlPage = new StringReader(XMLText);
                XmlTextReader trXMLRead = new XmlTextReader(trXmlPage);
                XPathDocument xPathDocument = new XPathDocument(trXMLRead);

                TextReader trXSL = new StringReader(XSLText);
                XmlTextReader trXSLRead = new XmlTextReader(trXSL);
                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(trXSLRead);

                TextWriter tw = new StringWriter(sb);
                xslt.Transform(xPathDocument, args, tw);
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            return sb.ToString().Replace("utf-16", "utf-8");
        }

        //
        //        public string Transform(string XMLText, string XSLText)
        //        {
        //            StringBuilder sb = new StringBuilder();
        //            try
        //            {
        //                TextReader trXmlPage = new StringReader(XMLText);
        //                XmlTextReader trXMLRead = new XmlTextReader(trXmlPage);
        //                XPathDocument xPathDocument = new XPathDocument(trXMLRead);
        //
        //                TextReader trXSL = new StringReader(XSLText);
        //                XmlTextReader trXSLRead = new XmlTextReader(trXSL);
        //                XslCompiledTransform xslt = new XslCompiledTransform();
        //                xslt.Load(trXSLRead);
        //
        //                TextWriter tw = new StringWriter(sb);
        //                xslt.Transform(xPathDocument, null, tw);
        //            }
        //            catch (Exception ex)
        //            {
        //                return ex.Message.ToString();
        //            }
        //            return sb.ToString().Replace("utf-16", "utf-8");
        //        }
        //

        public string Transform(string XMLText, string XSLTextFile)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                // XML
                TextReader trXmlPage = new StringReader(XMLText);
                XmlTextReader trXMLRead = new XmlTextReader(trXmlPage);
                XPathDocument xPathDocument = new XPathDocument(trXMLRead);

                // XSLT
                // TextReader trXSL = new StringReader(XSLTextFile);
                XmlReader xslRead = XmlReader.Create(XSLTextFile);
                xslRead.ReadToDescendant("xsl:stylesheet");

                XslCompiledTransform xForm = new XslCompiledTransform();
                xForm.Load(xslRead);

                TextWriter tw = new StringWriter(sb);

                xForm.Transform(xPathDocument, null, tw);
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            return sb.ToString().Replace("utf-16", "utf-8");
        }

        public string Transform(XmlDocument XMLDoc, string XSLText)
        {
            StringBuilder sb = new StringBuilder();
            string XMLText = XMLDoc.OuterXml.ToString();
            try
            {
                TextReader trXmlPage = new StringReader(XMLText);
                XmlTextReader trXMLRead = new XmlTextReader(trXmlPage);
                XPathDocument xPathDocument = new XPathDocument(trXMLRead);

                TextReader trXSL = new StringReader(XSLText);
                XmlTextReader trXSLRead = new XmlTextReader(trXSL);
                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(trXSLRead);

                TextWriter tw = new StringWriter(sb);
                xslt.Transform(xPathDocument, null, tw);
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            return sb.ToString();
        }

    }
}
