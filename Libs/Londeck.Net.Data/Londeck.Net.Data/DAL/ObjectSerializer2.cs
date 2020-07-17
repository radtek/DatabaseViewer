using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.AccessControl;
using System.Xml;
using System.Xml.Serialization;

    /// <summary> 
    /// Note: if serializing an object is throwing an exception, check the inner most 
    /// InnerException. This may mean you need to dig down 3 layers. 
    /// </summary> 
    /// <remarks></remarks> 
public class ObjectSerializer2
{
   public ObjectSerializer2()
   { }
   /// <summary> 
   /// Serializes an object to Xml and returns the string. 
   /// </summary> 
   /// <param name="objectToSerialize"></param> 
   /// <returns></returns> 
   /// <remarks></remarks> 
   public static string Serialize<T>(T objectToSerialize)
   {
      try
      {
         return Serialize<T>(objectToSerialize, System.Text.Encoding.UTF8);
      }
      catch (Exception ex)
      { throw ex; }
   }

   /// <summary> 
   /// Serializes an object to Xml and returns the string. 
   /// </summary> 
   /// <param name="object"></param> 
   /// <returns></returns> 
   /// <remarks></remarks> 
   public static string Serialize<T>(T @object, System.Text.Encoding encoding)
   {
      string result = string.Empty;
      XmlSerializer serializer = new XmlSerializer(typeof(T));

      using (MemoryStream memStream = new MemoryStream())
      {
         using (XmlTextWriter xmlWriter = new XmlTextWriter(memStream, encoding))
         {
            serializer.Serialize(xmlWriter, @object);
            if (memStream.Position != 0) memStream.Position = 0;

            using (StreamReader sr = new StreamReader(memStream, encoding))
            {
               result = sr.ReadToEnd();
            }
         }
      }
      return result;
   }

   /// <summary>
   /// Serializes an object to Xml and returns the string. (Browser Friendly, Used mostly by CacheManager and SessionManager) 
   /// </summary>
   /// <param name="object"></param>
   /// <returns></returns> 
   /// <remarks></remarks> 
   public static string Serialize(object @object)
   {
      System.Text.StringBuilder Result = null;
      StringWriter sw = XmlSerialize(@object);
      Result = sw.GetStringBuilder();
      return Result.ToString();
   }

   /// <summary> 
   /// Serializes an object to a given path 
   /// Note: if serializing an object is throwing an exception, check the inner most 
   /// InnerException. This may mean you need to dig down 3 layers. 
   /// </summary> 
   /// <param name="obj">Object being serialized to xml</param> 
   /// <param name="path">Full path of xml document</param> 
   /// <remarks></remarks> 
   public static void Serialize(object obj, string path)
   {
      if (System.IO.File.Exists(path))
      {
         FileSecurity fileSecurity = File.GetAccessControl(path);
         fileSecurity.AddAccessRule(new FileSystemAccessRule("Users",
                                                      FileSystemRights.FullControl,
                                                      AccessControlType.Allow));
         File.SetAccessControl(path, fileSecurity);
      }

      using (TextWriter writer = new StreamWriter(path))
      {
         XmlSerializer serializer = new XmlSerializer(obj.GetType());
         serializer.Serialize(writer, obj);
      }
   }

   public static T Deserialize<T>(string xml)
   {
      T result = default(T);
      XmlManager xmlMan = new XmlManager(xml, XmlManager.XML_TYPE.RawXml);
      using (XmlReader reader = new XmlNodeReader(xmlMan.XmlDoc.DocumentElement))
      {
         XmlSerializer serializer = new XmlSerializer(typeof(T));
         result = (T)serializer.Deserialize(reader);
      }
      return result;
   }

   /// <summary> 
   /// Deserializes an xml document to a given object type 
   /// </summary> 
   /// <param name="type">Type of object expected</param> 
   /// <param name="path">A valid XmlDocument</param> 
   /// <returns></returns> 
   /// <remarks></remarks> 
   public static object Deserialize(Type type, string path)
   {
      if ((!System.IO.File.Exists(path))) return null;

      try
      {
         if (System.IO.File.Exists(path))
         {
            FileSecurity fileSecurity = File.GetAccessControl(path);
            fileSecurity.AddAccessRule(new FileSystemAccessRule("Users",
                                                         FileSystemRights.FullControl,
                                                         AccessControlType.Allow));
            File.SetAccessControl(path, fileSecurity);
         }
         XmlDocument xmlDoc = new XmlDocument();
         xmlDoc.Load(path);
         return Deserialize(type, xmlDoc);
      }
      catch (XmlException xmlex)
      { throw xmlex; }
   }

   /// <summary> 
   /// Deserializes and XML string to a given object type 
   /// </summary> 
   /// <param name="type">Type of object expected to be returned</param> 
   /// <param name="xmlString">xml in string form</param> 
   /// <returns></returns> 
   /// <remarks></remarks> 
   public static object DeserializeFromString(Type type, string xmlString)
   {
      if (string.IsNullOrEmpty(xmlString))
         return null;

      XmlDocument xmlDoc = new XmlDocument();
      xmlDoc.LoadXml(xmlString);
      return Deserialize(type, xmlDoc);
   }

   /// <summary> 
   /// Deserializes an xml document to a given object Type 
   /// </summary> 
   /// <param name="type">Type of object expected</param> 
   /// <param name="xmlDoc">A valid XmlDocument</param> 
   /// <returns></returns> 
   /// <remarks></remarks> 
   public static object Deserialize(Type type, XmlDocument xmlDoc)
   {
      object obj = null;
      using (XmlNodeReader reader = new XmlNodeReader(xmlDoc.DocumentElement))
      {
         XmlSerializer serializer = new XmlSerializer(type);
         obj = serializer.Deserialize(reader);
      }
      return obj;
   }

   public static object DeserializeFromString2(Type type, string xmlDoc)
   {
      object obj = null;

      //using (XmlNodeReader reader = new XmlNodeReader(xmlDoc.DocumentElement))
      //{
      //using (TextReader tr = 
      StringReader stringReader;
      stringReader = new StringReader(xmlDoc);
      XmlTextReader xmlReader;
      xmlReader = new XmlTextReader(stringReader);
      xmlReader.WhitespaceHandling = WhitespaceHandling.None;
      xmlReader.Normalization = false;
      XmlSerializer serializer = new XmlSerializer(type);
      obj = serializer.Deserialize(xmlReader);
      //}
      return obj;
   }
   /// <summary> 
   /// 
   /// </summary> 
   /// <remarks> 
   /// Created, Andrew Powell 10.26.07 
   /// Because the plain old StringWriter doesnt accept an Encoding parameter, 
   /// and I didnt want to modify 
   /// XmlSerialize -that- much. 
   /// </remarks> 
   private class EncodedStringWriter : System.IO.StringWriter
   {
      private System.Text.Encoding _Encoding;
      public EncodedStringWriter(System.Text.Encoding encoding)
         : base()
      {
         _Encoding = encoding;
      }

      public override System.Text.Encoding Encoding
      {
         get { return _Encoding; }
      }
   }

   /// <summary> 
   /// Serialize object into StringWriter using XmlSerialization 
   /// </summary> 
   /// <param name="objectToSerialize">object to serialize</param> 
   /// <returns></returns> 
   /// <remarks> 
   /// Created, Andrew Powell 10.26.07 
   /// </remarks> 
   private static StringWriter XmlSerialize(object objectToSerialize)
   {
      return XmlSerialize(objectToSerialize, System.Text.Encoding.UTF8);
   }

   /// <summary> 
   /// Serialize object into StringWriter using XmlSerialization 
   /// </summary> 
   /// <param name="objectToSerialize">object to serialize</param> 
   /// <returns></returns> 
   /// <remarks> 
   /// Unknown Original Author 
   /// Edited, Andrew Powell 10.26.07 
   /// - Modified method to use EncodedStringWriter. 
   /// </remarks> 
   private static StringWriter XmlSerialize(object objectToSerialize, System.Text.Encoding encoding)
   {
      EncodedStringWriter writer = new EncodedStringWriter(encoding);
      try
      {
         XmlSerializer serializer = new XmlSerializer(objectToSerialize.GetType());

         //This is used to remove the standard XMl namespaces from the serialized output 
         //so as to make it easier to see in the browser output 
         XmlQualifiedName[] dummyNamespaceName = new XmlQualifiedName[1];
         dummyNamespaceName[0] = new XmlQualifiedName();
         serializer.Serialize(writer, objectToSerialize, new XmlSerializerNamespaces(dummyNamespaceName));
      }
      catch (InvalidOperationException ex)
      {
         //If we cannot serialize then we can't leave it at that 

      }
      catch (System.Runtime.Serialization.SerializationException ex)
      {
         //Ignore This can happen when some objects are just not Serializable using XML serialization 
      }
      catch (Exception ex)
      {
         //Ignore. This can happen when storing a set of custom objects in a collection. 
         //The XmlSerializer will start to serialize the collection come across the custom objects 
         //amd not know what to do. The use of custom serialization attributes will help the serializer. 
      }
      //This will only be hit by a failed serialization execution 
      return writer;
   }
}