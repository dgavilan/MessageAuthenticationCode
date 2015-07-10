using Mac.VertifyObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Mac
{
    public class VertifyHelper
    {
        public Mac.VertifyObjects.ReviewQueryResponse_1_6_1.Response MapToReviewQueryResponse_1_6_1(string xml)
        {
            try
            {
                var reader = new StringReader(xml);
                var xmlSerializer = new XmlSerializer(typeof(Mac.VertifyObjects.ReviewQueryResponse_1_6_1.Response));
                return xmlSerializer.Deserialize(reader) as Mac.VertifyObjects.ReviewQueryResponse_1_6_1.Response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
