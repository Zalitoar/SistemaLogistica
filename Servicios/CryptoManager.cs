using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Servicios
{
    public static class CryptoManager
    {        
        public static string Hash(string textoPlano)
        {            
            if (string.IsNullOrEmpty(textoPlano)) return "";

            using (SHA256 sha256 = SHA256.Create())
            {                
                byte[] bytesOriginales = Encoding.UTF8.GetBytes(textoPlano);
                                
                byte[] bytesHasheados = sha256.ComputeHash(bytesOriginales);
                                
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < bytesHasheados.Length; i++)
                {                    
                    sb.Append(bytesHasheados[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
