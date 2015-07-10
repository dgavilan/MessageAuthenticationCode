using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mac;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Mac.VertifyObjects;
using System.Xml;

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

            var macMessage = GenerateMacMessage();
            var macSecretPassword = GenerateMacSecretPassword();

            var macHelper = new MacHelper();

            var mac = macHelper.BuildMac(macSecretPassword, macMessage);
            var macValidate = macHelper.BuildMac(macSecretPassword, macMessage);

            Assert.IsTrue(mac == macValidate);
        }

        [TestMethod]
        public void SendRequest()
        {
            var macHelper = new MacHelper();
            var mac = macHelper.BuildMac(GenerateMacSecretPassword(), GenerateMacMessage());

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
            formData.Add("MAC", mac);

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

            var webResponseText = "";
            var encoding = System.Text.Encoding.GetEncoding(1252);

            using (var webResponse = request.GetResponse() as HttpWebResponse)
            {
                using (var webResponseStream = new StreamReader(webResponse.GetResponseStream(), encoding))
                {
                    webResponseText = webResponseStream.ReadToEnd();
                }
            }
        }

        private string GenerateMacSecretPassword()
        {
            return "sharedSecretPassword";
        }

        private string GenerateMacMessage()
        {

            var requestor = "sample.user";
            var session = "session1";
            var timestamp = DateTime.UtcNow.ToString();
            var routing = "routing1";
            var member = "member1";
            var account = "account1";
            var message = String.Format("{0}{1}{2}{3}{4}{5}", requestor, session, timestamp, routing, member, account);

            return message;
        }

        [TestMethod]
        public void ValidateVertifyReviewQueryResponse()
        {
            var vertifyHelper = new VertifyHelper();
            var reviewQueryResponse = new Mac.VertifyObjects.ReviewQueryResponse_1_6_1.Response();

            try
            {
                var reviewQueryResponseXml = @"<Response>
                                                <MessageValidation>
                                                    <InputValidation>OK</InputValidation>
                                                </MessageValidation>
                                                <Deposit>
                                                    <Account_Number/>
                                                    <Acct_Description/>
                                                    <DepositID/>
                                                    <Create_Timestamp/>
                                                    <Release_TImestamp/>
                                                    <Release_Timestamp_UTC/>
                                                    <Items/>
                                                    <Amount/>
                                                </Deposit>
                                            </Response>";

                reviewQueryResponse = vertifyHelper.MapToReviewQueryResponse_1_6_1(reviewQueryResponseXml);
            }
            catch (Exception)
            {
                //throw;
            }

            var status = ((XmlNode[])reviewQueryResponse.MessageValidation.InputValidation)[0].Value;

            Assert.IsTrue(status == "OK");
        }
    }
}
