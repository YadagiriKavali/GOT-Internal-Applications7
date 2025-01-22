using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Serialization;
using MSDGAPI.BL;

namespace MSDGAPI.Controllers
{
    [AuthorizeRequest]
    public class CommonController : ApiController
    {
        #region [ HTTP POST ]

        [HttpPost]
        public HttpResponseMessage Details(object data)
        {
            object responseData = ProcessRequest.Process(Request, data);

            string mediaType = string.Empty;
            IEnumerable<string> headerValues;
            if (Request.Headers.TryGetValues("CustomMediaType", out headerValues))
                mediaType = headerValues.FirstOrDefault();

            if (mediaType.ToLower() == "application/xml")
            {
                var result = string.Empty;
                try
                {
                    var stringWriter = new StringWriter();
                    var serializer = new XmlSerializer(responseData.GetType());
                    serializer.Serialize(stringWriter, responseData);
                    result = stringWriter.ToString().Replace("\r\n", "");
                }
                catch { }

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, responseData);
            }
        }

        #endregion [ HTTP POST ]

        #region [ HTTP GET ]

        [HttpGet]
        public HttpResponseMessage GetDetail(object data)
        {
            object responseData = ProcessRequest.Process(Request, data);
            return Request.CreateResponse(HttpStatusCode.OK, responseData);
        }

        #endregion [ HTTP GET ]
    }
}
