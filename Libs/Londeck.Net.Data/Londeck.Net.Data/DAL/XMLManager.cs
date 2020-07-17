using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

    public class XmlManager : IDisposable
    {
        public XmlDocument XmlDoc = new XmlDocument();
        private XmlNodeList _NodeList;
        private XmlNode _Node;

        private bool _XMLDocOpen = false;
        private string _XmlFile;
        private string _XmlErrorDetail;
        private bool _XmlError;

        public enum XML_TYPE
        {
            Filename,
            RawXml
        }

        public XmlManager()
        {

        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            CloseXMLDoc();
            _NodeList = null;
            _Node = null;
        }
        public XmlManager(string Xml)
        {
            //If I want to do from cache at a certain point then i can use LoadXml and I have to put another param in 
            Load(Xml, XML_TYPE.Filename);
        }

        public XmlManager(string Xml, XML_TYPE Type)
        {
            //If I want to do from cache at a certain point then i can use LoadXml and I have to put another param in 
            Load(Xml, Type);
        }

        public XmlManager(Uri Url)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            System.Net.WebClient web = new System.Net.WebClient();
            Stream rs = null;
            StreamReader sr = null;
            try
            {
                try
                {
                    rs = web.OpenRead(Url.AbsoluteUri);
                }
                catch (Exception webex)
                {
                    _XmlError = true;
                    _XmlErrorDetail = string.Concat(webex.ToString(), " ", webex.Message);

                    return;
                }

                try
                {
                    web.Dispose();
                    web = null;
                    sr = new StreamReader(rs);

                    string Xml = sr.ReadToEnd();
                    Load(Xml, XML_TYPE.RawXml);
                }
                catch (Exception ex)
                {
                    _XmlError = true;
                    _XmlErrorDetail = string.Concat(ex.ToString(), " ", ex.Message);
                }
            }
            finally
            {
                if ((sr != null)) sr.Close();

                if ((rs != null)) rs.Close();

                //Another method. Had really odd, random characters in the beginning making the xml invalid. 
                //Dim enc As New System.Text.ASCIIEncoding 
                //Dim web As New System.Net.WebClient 
                //Dim res As Byte() 

                //Try 
                // res = web.DownloadData(Url.AbsoluteUri) 

                //Catch webex As System.Net.WebException 
                //End Try 

                //Try 
                // web.Dispose() 
                // web = Nothing 

                // Dim Xml As String = enc.GetString(res) 

                // Load(Xml, XML_TYPE.RawXml) 

                //Catch ex As Exception 
                // _XmlError = True 
                // _XmlErrorDetail = String.Concat(ex.ToString, " ", ex.Message) 

                //End Try 

            }
        }

        #region " XML Functions "
        public string GetItem(string Path, string Attrib)
        {
            string Result = null;
            try
            {
                _Node = SelectNode(Path);
                if (_Node == null)
                {
                    return string.Empty;
                }
                else
                {
                    if (Attrib.Equals(string.Empty))
                    {
                        Result = _Node.InnerText;
                    }
                    else
                    {
                        XmlNode Attribute = _Node.Attributes.GetNamedItem(Attrib);
                        if ((Attribute != null))
                        {
                            Result = Attribute.Value;
                            Attribute = null;
                        }
                    }
                }
                _Node = null;
            }

            catch (XmlException xmlexp)
            {
                _XmlError = true;
                _XmlErrorDetail = string.Concat(xmlexp.ToString(), " ", xmlexp.Message);
                Result = string.Empty;
            }
            if (Result == null) Result = string.Empty;
            return Result;
        }

        public void AddItem(string Path, string Attrib, string Value)
        {
            try
            {
                _Node = SelectNode(Path);

                if ((_Node != null))
                {
                    if (string.IsNullOrEmpty(Attrib))
                    {
                        _Node.InnerText = Value;
                    }
                    else
                    {
                        _Node.Attributes.GetNamedItem(Attrib).Value = Value;
                    }
                }
                _Node = null;
            }
            catch (XmlException xmlexp)
            {
                _XmlError = true;
                _XmlErrorDetail = string.Concat(xmlexp.ToString(), " ", xmlexp.Message);
            }
        }

        public System.Collections.Generic.List<XmlNode> List(string Path)
        {
            System.Collections.Generic.List<XmlNode> ValueList = new System.Collections.Generic.List<XmlNode>();
            XmlNodeList NodeList = GetNodeList(Path, true);

            if ((NodeList != null))
            {
                foreach (XmlNode Node in NodeList)
                {
                    ValueList.Add(Node);
                }
            }
            return ValueList;
        }

        public XmlNode Node(string path)
        {
            return SelectNode(path);
        }

        protected bool Load(string XmlPath, XML_TYPE Type)
        // ERROR: Optional parameters aren't supported in C# XML_TYPE Type) 
        {
            bool retcode = false;
            try
            {
                //Apparently MSDN says that this will autoformat the xml if I leave it false. We'll see 
                XmlDoc.PreserveWhitespace = false;
                try
                {
                    if (XmlPath == null || XmlPath.Equals(string.Empty))
                    {
                        _XMLDocOpen = false;
                        _XmlError = true;
                        _XmlErrorDetail = "File not found.";
                        retcode = false;
                    }

                    switch (Type)
                    {
                        case XML_TYPE.Filename:

                            FileInfo XmlFile = new FileInfo(XmlPath);
                            if (!XmlFile.Exists)
                            {
                                _XMLDocOpen = false;
                                _XmlError = true;
                                _XmlErrorDetail = "File not found.";

                                return false;
                            }

                            XmlDoc.Load(XmlPath);
                            _XmlFile = XmlPath;

                            break;
                        case XML_TYPE.RawXml:
                            XmlDoc.LoadXml(XmlPath);

                            break;
                    }
                    _XMLDocOpen = true;
                }
                catch (System.IO.FileNotFoundException io)
                { }
                //Console.WriteLine(io.FileName)
            }
            catch (XmlException xmlexp)
            {
                _XMLDocOpen = false;
                _XmlError = true;
                _XmlErrorDetail = string.Concat(xmlexp.ToString(), " ", xmlexp.Message);

                retcode = false;
            }
            return retcode;
        }

        public string Xml
        {
            get { return XmlDoc.OuterXml; }
        }

        public bool XmlOpen
        {
            get { return _XMLDocOpen; }
        }

        public bool XmlError
        {
            get { return _XmlError; }
        }

        public string XmlErrorDetail
        {
            get { return _XmlErrorDetail; }
        }

        public void CloseXMLDoc()
        {
            XmlDoc = null;
            _XMLDocOpen = false;
        }

        protected XmlNode SelectNodeFromList(string Path, string AttributeName, string AttributeValue)
        {

            XmlNodeList List = XmlDoc.SelectNodes(Path);

            if (List == null) return null;
            if (List.Count <= 0) return null;

            foreach (XmlNode Node in List)
            {

                if (!(Node.NodeType == XmlNodeType.Whitespace))
                {

                    foreach (XmlAttribute Attribute in Node.Attributes)
                    {

                        if (Attribute.Name == AttributeName & Attribute.Value == AttributeValue) return Node;

                    }

                }
            }


            return null;
        }

        protected XmlNode SelectNodeFromList(XmlNode ParentNode, string Path, string AttributeName, string AttributeValue)
        {

            XmlNodeList List = ParentNode.SelectNodes(Path);

            if (List == null) return null;
            if (List.Count <= 0) return null;

            foreach (XmlNode Node in List)
            {
                if (!(Node.NodeType == XmlNodeType.Whitespace))
                {
                    foreach (XmlAttribute Attribute in Node.Attributes)
                    {
                        if (Attribute.Name == AttributeName & Attribute.Value == AttributeValue) return Node;
                    }
                }
            }
            return null;
        }

        protected XmlNodeList GetNodeList(string szPath, bool bChildren)
        {
            try
            {
                if (!bChildren)
                {
                    return XmlDoc.SelectNodes(szPath);
                }
                else
                {
                    XmlDoc.LoadXml(SelectNode(szPath).OuterXml);
                    return _Node.ChildNodes;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        protected XmlNodeList GetNodeList(XmlNode _Node, string szPath, bool bChildren)
        // ERROR: Optional parameters aren't supported in C# bool bChildren) 
        {
            try
            {
                if (!bChildren)
                {
                    return _Node.SelectNodes(szPath);
                }
                else
                {
                    _Node = _Node.SelectSingleNode(szPath);
                    return _Node.ChildNodes;
                }
            }
            catch (Exception e)
            {
                _XmlError = true;
                _XmlErrorDetail = string.Concat(e.ToString(), " ", e.Message);

                return null;
            }
        }


        protected XmlNode SelectNode(string szPath)
        {
            XmlNode functionReturnValue = default(XmlNode);
            try
            {
                _NodeList = XmlDoc.SelectNodes(szPath);

                functionReturnValue = _NodeList.Item(0);

                _Node = null;
                _NodeList = null;
            }
            catch (XmlException xmlexp)
            {
                functionReturnValue = null;
                _Node = null;
                _NodeList = null;
                _XmlError = true;

                _XmlErrorDetail = string.Concat(xmlexp.ToString(), " ", xmlexp.Message);
            }
            catch (Exception ex)
            {
                functionReturnValue = null;
                _Node = null;

                _NodeList = null;
            }

            return functionReturnValue;
        }

        protected string GetSettingNode(XmlNode _Node)
        {
            string functionReturnValue = null;
            try
            {
                if (_Node == null)
                {
                    functionReturnValue = "";
                }
                else
                {
                    functionReturnValue = _Node.InnerText;
                }

                _Node = null;
            }
            catch (XmlException xmlexp)
            {
                _XmlError = true;
                _XmlErrorDetail = string.Concat(xmlexp.ToString(), " ", xmlexp.Message);

                return string.Empty;
            }

            return functionReturnValue;
        }

        public void Save(bool Backup)  // ERROR: Optional parameters aren't supported in C# bool Backup) 
        {

            if (_XmlFile == null) return;

            if (_XmlFile.Equals(string.Empty)) return;


            try
            {

                bool BackupMade = false;

                try
                {

                    if (Backup)
                    {
                        FileInfo File = new FileInfo(_XmlFile);
                        if (File.Exists)
                        {

                            System.Text.StringBuilder sb = new System.Text.StringBuilder();

                            sb.Append(DateTime.Now.Hour);
                            sb.Append(DateTime.Now.Minute);
                            sb.Append(DateTime.Now.Second);
                            sb.Append(DateTime.Now.Day);
                            sb.Append(DateTime.Now.Month);
                            sb.Append(DateTime.Now.Year);
                            sb.Append(File.Extension);

                            File.CopyTo(File.FullName.Replace(File.Extension, sb.ToString()));
                        }
                    }
                    XmlDoc.Save(_XmlFile);
                }
                catch (Exception ex)
                {
                    System.Threading.Thread.Sleep(100);

                    if (!BackupMade & Backup)
                    {
                        FileInfo File = new FileInfo(_XmlFile);
                        if (File.Exists)
                        {
                            System.Text.StringBuilder sb = new System.Text.StringBuilder();

                            sb.Append(DateTime.Now.Hour);
                            sb.Append(DateTime.Now.Minute);
                            sb.Append(DateTime.Now.Second);
                            sb.Append(DateTime.Now.Day);
                            sb.Append(DateTime.Now.Month);
                            sb.Append(DateTime.Now.Year);
                            sb.Append(File.Extension);

                            File.CopyTo(File.FullName.Replace(File.Extension, sb.ToString()));
                        }
                    }

                    XmlDoc.Save(_XmlFile);
                }
            }
            catch (XmlException xmlexp)
            {
                //System.Windows.Forms.Application.DoEvents() 
                _XmlError = true;

                _XmlErrorDetail = string.Concat(xmlexp.ToString(), " ", xmlexp.Message);

            }
        }

        public XmlNode CreateReturnNode(string szName, string szParentPath, string szVal, bool Backup)
        {
            XmlNode functionReturnValue = default(XmlNode);
            try
            {
                XmlNode oNewNode = default(XmlNode);

                oNewNode = XmlDoc.CreateNode(XmlNodeType.Element, szName, "");
                oNewNode.InnerText = szVal;
                _Node = SelectNode(szParentPath);

                functionReturnValue = _Node.AppendChild(oNewNode);

                //XmlDoc.Save(_XmlFile) 
                Save(Backup);

                oNewNode = null;

                _Node = null;
            }
            catch (XmlException xmlexp)
            {
                _XmlError = true;
                _XmlErrorDetail = string.Concat(xmlexp.ToString(), " ", xmlexp.Message);

                return null;
            }

            return functionReturnValue;
        }

        public bool NodeExists(string szPath)
        {
            bool functionReturnValue = false;
            try
            {
                _Node = SelectNode(szPath);
                functionReturnValue = (_Node != null);

                _Node = null;
            }
            catch (XmlException xmlexp)
            {
                _XmlError = true;
                _XmlErrorDetail = string.Concat(xmlexp.ToString(), " ", xmlexp.Message);

                return false;
            }

            return functionReturnValue;
        }

        public static XmlElement RenameElement(XmlElement e, string newName)
        {
            XmlDocument doc = e.OwnerDocument;
            XmlElement newElement = doc.CreateElement(newName);
            while (e.HasChildNodes)
            {

                newElement.AppendChild(e.FirstChild);
            }
            XmlAttributeCollection ac = e.Attributes;
            while (ac.Count > 0)
            {

                newElement.Attributes.Append(ac[0]);
            }
            XmlNode parent = e.ParentNode;
            parent.ReplaceChild(newElement, e);


            return newElement;
        }
        #endregion

        #region " Legacy Methods From SDNetLib and SDSArticles "
        public static XmlElement GetElementbyName(XmlNode thisElement, string strElementValue, ref bool result)
        {
            XmlNode xml_TempElement = default(XmlNode);
            XmlElement xml_Element = null;

            //Debug.Print thisElement.baseName & ": " & thisElement.Text 
            if (thisElement.Name == strElementValue)
            {
                //                xml_Element = thisElement;
                result = true;
            }
            else if (result != true)
            {
                if (thisElement.HasChildNodes)
                {
                    for (int ctr = 0; ctr <= thisElement.ChildNodes.Count - 1; ctr++)
                    {
                        //If (result <> True) And (thisElement.ChildNodes(0).GetType().FullName = "System.XML.XMLElement") Then 
                        if ((result != true) && (thisElement.ChildNodes[ctr].GetType().Name == "XmlElement"))
                        {
                            xml_TempElement = thisElement.ChildNodes[ctr];
                            xml_Element = GetElementbyName(xml_TempElement, strElementValue, ref result);
                        }
                    }
                }
            }
            return xml_Element;
        }
        #endregion
    }
