﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace GA_SMO
{
   public class Encryption
   {
      public static string EncryptData(string Message)
      {
         byte[] Results;
         System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
         MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
         byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes("HTICyberNetics2018"));
         TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
         TDESAlgorithm.Key = TDESKey;
         TDESAlgorithm.Mode = CipherMode.ECB;
         TDESAlgorithm.Padding = PaddingMode.PKCS7;
         byte[] DataToEncrypt = UTF8.GetBytes(Message);
         try
         {
            ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
            Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
         }
         finally
         {
            TDESAlgorithm.Clear();
            HashProvider.Clear();
         }
         return Convert.ToBase64String(Results);
      }

      public static string DecryptString(string Message)
      {
         byte[] Results;
         System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
         MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
         byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes("HTICyberNetics2018"));
         TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
         TDESAlgorithm.Key = TDESKey;
         TDESAlgorithm.Mode = CipherMode.ECB;
         TDESAlgorithm.Padding = PaddingMode.PKCS7;
         byte[] DataToDecrypt = Convert.FromBase64String(Message);
         try
         {
            ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
            Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
         }
         finally
         {
            TDESAlgorithm.Clear();
            HashProvider.Clear();
         }

         return UTF8.GetString(Results);
      }

   }
}
