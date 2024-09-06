using System;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        byte[] sharedSecret = Encoding.UTF8.GetBytes("abc123");
        string sequence = "1174efedab186000";
        string body = "{\"hello\": \"world\"}";

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
            Console.WriteLine($"Calculated HMAC: {calculatedHmac}");
            Console.ReadLine();
        }
    }
}
