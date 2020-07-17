using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GA_SMO
{
    public class XMLMarshaller
    {
        string _XMLFileName = string.Empty;
        public XMLMarshaller(string XMLFileName)
        {
            string Directory = System.IO.Path.GetDirectoryName(XMLFileName);
            if(!(System.IO.File.Exists(XMLFileName)))
            {
                // So the file does not exist.  Does the directory exits?
                if(!( System.IO.Directory.Exists(Directory)))
                    System.IO.Directory.CreateDirectory(Directory);
            }
            _XMLFileName = XMLFileName;
        }

        public string XMLFileName
        {
            get { return _XMLFileName;}
            set { _XMLFileName = value; }
        }

        private string ApplicationPath()
        {
            string ApplPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            ApplPath = System.IO.Path.GetDirectoryName(ApplPath) + @"\";
            return ApplPath;
        }

    }
}
