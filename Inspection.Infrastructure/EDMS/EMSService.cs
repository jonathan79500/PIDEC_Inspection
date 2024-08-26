using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.Models;
using Inspection.Domain.Entities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace Inspection.Infrastructure.EDMS
{
    public class EMSService : IEDMSService
    {
        private EDMSServiceSettings _edmsServiceSettings { get; }        

        private HttpClient _httpClient;
        private IEDMSService _edmsService;


        public EMSService(IOptions<EDMSServiceSettings> edmsServiceSettings)
        {
            _edmsServiceSettings = edmsServiceSettings.Value;
            _httpClient= new HttpClient();
            _httpClient.BaseAddress = new Uri(_edmsServiceSettings.APIAddress);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.AcceptCharset.Clear();
            _httpClient.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
          
        }
        public Task<List<Vendor>> GetVendorInformation(string vendorUserName, int domainId)
        {

            //var values = new Dictionary<string, string> { { "username", vendorUserName }, { "dom_Id", domainId.ToString() }, { "code", _edmsServiceSettings.SecurityCode } };
            //var content = new FormUrlEncodedContent(values);

            var query = "{\r\n    \"username\": \"1235-el-509A-1345\",\r\n    \"dom_Id\": 1,\r\n    \"code\":\"7eau2cfp-1ays-3718-65h1-a12417628bnp\"\r\n}";
            var query1 = "{\"username\": \"1235-el-509A-1345\",\"dom_Id \": 1,\"code\":\"7eau2cfp-1ays-3718-65h1-a12417628bnp\"}";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_edmsServiceSettings.APIAddress);
                var content0 = new StringContent(query, Encoding.UTF8, "application/json");

                ////Add client header
                //client.DefaultRequestHeaders.Add("client_uuid", "2fd77dd8-ed76-4bba-b0e1-5cda454c8d6e");

                HttpResponseMessage response0 =   client.PostAsync("GetProject_Order_VendorDocs_ByUserName", content0).Result;


                List<Vendor> re=new List<Vendor>();
                if (response0.IsSuccessStatusCode)
                {
                    var vendor = response0.Content.ReadAsStringAsync().Result;
                }
                return Task.FromResult(re);

            }



            var result= new List<Vendor>();


            

            //HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri(_edmsServiceSettings.APIAddress);

            //parameters as FormBody

          

            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

           

            System.Text.Json.JsonSerializerOptions op = new System.Text.Json.JsonSerializerOptions();

            string str1= "{\"username\":\"" + vendorUserName + "\",\"dom_Id\":" + domainId.ToString() + ",\"code\":\"" + _edmsServiceSettings.SecurityCode+"\"}";

            JObject json = JObject.Parse(str1);

            // _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            _httpClient.DefaultRequestHeaders.AcceptEncoding.Clear();
            _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Transfer-Encoding", "chunked");
            _httpClient.DefaultRequestHeaders.TransferEncodingChunked = true;
           

        
            HttpResponseMessage response = _httpClient.PostAsJsonAsync("GetProject_Order_VendorDocs_ByUserName",query).Result;
            HttpRequestOptions requestOptions = new HttpRequestOptions();
                        
            if (response.IsSuccessStatusCode)
            {
                var vendor = response.Content.ReadAsStringAsync().Result;
            }
            return Task.FromResult(result);
        }

        public Task<bool> UpdateDocumentToStateInuseInEDMS(string DocumentId, bool isUsed)
        {
            throw new NotImplementedException();
        }

        public Task<bool> VendorIsValid(string userName, string password, int domainId)
        {
            bool VendorIsValid = false;
            var values = new Dictionary<string, string> { { "username", userName }, { "dom_Id", domainId.ToString() }, { "code", _edmsServiceSettings.SecurityCode } };
            var content = new FormUrlEncodedContent(values);

         
            HttpResponseMessage response = _httpClient.PostAsJsonAsync("IsAuthenticated", content).Result;

            if (response.IsSuccessStatusCode)
            {
                var VendorIsValids = response.Content.ReadAsStringAsync().Result;
            }
            return Task.FromResult(VendorIsValid);
        }
    }
}
