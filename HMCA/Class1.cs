using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HMCA
{
    public static class HMCA
    {
        public static string Encode(string sq, string key, string body1)
        {
            byte[] sharedSecret = Encoding.UTF8.GetBytes(key);
            string sequence = key;
            string body = body1;

            // Create HMAC
            using (HMACSHA256 hmac = new HMACSHA256(sharedSecret))
            {
                // Update HMAC with sequence
                byte[] sequenceBytes = Encoding.UTF8.GetBytes(sequence);
                hmac.TransformBlock(sequenceBytes, 0, sequenceBytes.Length, sequenceBytes, 0);

                // Update HMAC with space
                byte[] spaceByte = Encoding.UTF8.GetBytes(" ");
                hmac.TransformBlock(spaceByte, 0, spaceByte.Length, spaceByte, 0);

                // Update HMAC with body
                byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
                hmac.TransformFinalBlock(bodyBytes, 0, bodyBytes.Length);

                // Get the HMAC as a hexadecimal string
                string calculatedHmac = BitConverter.ToString(hmac.Hash).Replace("-", "").ToLower();
                return calculatedHmac;
            }
            return "";
        }
    }

    
}
