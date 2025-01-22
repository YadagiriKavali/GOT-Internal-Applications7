using System.Net;
using System.Net.Http;
using System.Web.Http;
using SMSAPI.BL;
using SMSAPI.Models.Requests;

namespace SMSAPI.Controllers
{
    public class CommonController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage GetBillDetail([FromUri]BillDetailReq data)
        {
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
            string _strResponse = new ProcessBillDetailReq().ProcessRequest(data);
            httpResponse.Content = new StringContent(_strResponse, System.Text.Encoding.UTF8, "text/plain");

            return httpResponse;
           // return Request.CreateResponse(HttpStatusCode.OK, new  ProcessBillDetailReq().ProcessRequest(data),);
        }
    }
}
