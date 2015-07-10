using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Mac
{
    public class MacHelper
    {
        public string BuildMac(string key, string message)
        {
            // NOTE: Code taken from http://buchananweb.co.uk/security01i.aspx

            /*
             * Security is handled by use of a Message Authentication Code (MAC).  The MAC is generated using 
             * the Keyed-Hashing for Message Authentication (HMAC) algorithm (as defined in RFC 2104).  
             * HMAC utilizes the MD5 Message-Digest Algorithm (as defined in RFC 1321) as the hash function.
             */

            var encoding = new ASCIIEncoding();
            var keyByte = encoding.GetBytes(key);

            var hmacmd5 = new HMACMD5(keyByte);
            //var hmacsha1 = new HMACSHA1(keyByte);
            //var hmacsha256 = new HMACSHA256(keyByte);
            //var hmacsha384 = new HMACSHA384(keyByte);
            //var hmacsha512 = new HMACSHA512(keyByte);


            


            var messageBytes = encoding.GetBytes(message);
            var hashMessage = hmacmd5.ComputeHash(messageBytes);




            return ByteToString(hashMessage);

            //hashmessage = hmacsha1.ComputeHash(messageBytes);

            //this.hmac2.Text = ByteToString(hashmessage);

            //hashmessage = hmacsha256.ComputeHash(messageBytes);

            //this.hmac3.Text = ByteToString(hashmessage);

            //hashmessage = hmacsha384.ComputeHash(messageBytes);

            //this.hmac4.Text = ByteToString(hashmessage);

            //hashmessage = hmacsha512.ComputeHash(messageBytes);

            //this.hmac5.Text = ByteToString(hashmessage);

        }



        public static string ByteToString(byte[] buff)
        {
            string sbinary = "";
            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); // hex format
            }
            return (sbinary);
        }

    }
}
