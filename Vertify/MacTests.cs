using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mac;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Vertify
{
    [TestClass]
    public class MacTest
    {
        [TestMethod]
        public void CanGenerateValidMac()
        {
            /*
             The text string that will be hashed is the concatenation of the 
             * <requestor>, <session>, <timestamp>, <routing>, <member>, and <account> fields.  
             * RFC 2104 calls for a shared secret password that is used in creating the MAC.  
             * Vertifi and the home banking services provider mutually agree on a key 
             * (a minimum of 16-bytes in length), and a mechanism and schedule for periodically 
             * refreshing this key.
            
             * http://www.briangrinstead.com/blog/multipart-form-post-in-c
             */

            var mac = GenerateMac();

            Assert.IsNotNull(mac);
        }

        [TestMethod]
        public void SendRequest()
        {
            var postUrl = "http://testing.testing.com";
            var contentType = "multipart/form-data";
            var userAgent = "";

            var formDataStream = new System.IO.MemoryStream();
            var formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
            
            var formData = new Dictionary<string, string>();
            formData.Add("requestor", "requestor1");
            formData.Add("session", "session1");
            formData.Add("timestamp", DateTime.UtcNow.ToString());
            formData.Add("routing", "routing1");
            formData.Add("member", "member1");
            formData.Add("account", "account1");
            formData.Add("MAC", GenerateMac());

            foreach (var data in formData)
            {
                var postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                    formDataBoundary,
                    data.Key,
                    data.Value);

                formDataStream.Write(Encoding.UTF8.GetBytes(postData), 0, Encoding.UTF8.GetByteCount(postData));
            }

            formDataStream.Position = 0;
            byte[] formDataBytes = new byte[formDataStream.Length];
            formDataStream.Read(formDataBytes, 0, formDataBytes.Length);
            formDataStream.Close();

            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;
            
            request.Method = "POST";
            request.ContentType = contentType;
            request.UserAgent = userAgent;
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formDataBytes.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(formDataBytes, 0, formDataBytes.Length);
                requestStream.Close();
            }

            var response = request.GetResponse() as HttpWebResponse;
        }

        private string GenerateMac()
        {
            var requestor = "sample.user";
            var session = "session1";
            var timestamp = DateTime.UtcNow.ToString();
            var routing = "routing1";
            var member = "member1";
            var account = "account1";
            var sharedSecretPassword = "sharedSecretPassword";
            var message = String.Format("{0}{1}{2}{3}{4}{5}", requestor, session, timestamp, routing, member, account);

            var macHelper = new MacHelper();
            var mac = macHelper.BuildMac(
                key: sharedSecretPassword,
                message: message);

            return mac;
        }
    }
}
