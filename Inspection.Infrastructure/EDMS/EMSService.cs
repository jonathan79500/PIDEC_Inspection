using Inspection.Application.Contracts.Infrastructure;
using Inspection.Application.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Inspection.Infrastructure.EDMS.Entities;
using Inspection.Application.DTOs.User;
using Inspection.Application.DTOs.Common;
using Inspection.Domain.Entities.User.Enums;
using Inspection.Domain.Entities.Projects;
using Inspection.Domain.Entities.Orders;
using Inspection.Domain.Entities.User.Entities;
using Inspection.Domain.Entities.Vendor;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Inspection.Domain.Entities.EDMS;
using Inspection.Domain.Entities.Contacts;
using Mapster;

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
        public Task<Contact> GetVendorInformation(string vendorUserName, int domainId)
        {         
            var query = "{\r\n    \"username\": \"1235-el-509A-1345\",\r\n    \"dom_Id\": 1,\r\n    \"code\":\"7eau2cfp-1ays-3718-65h1-a12417628bnp\"\r\n}";

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_edmsServiceSettings.APIAddress);
                    var content0 = new StringContent(query, Encoding.UTF8, "application/json");

                    HttpResponseMessage response0 = client.PostAsync("GetProject_Order_VendorDocs_ByUserName", content0).Result;


                    EdmsVendorInfoResponse structuredResponse = new EdmsVendorInfoResponse();


                    Contact vendorInfo = new Contact();

                    if (response0.IsSuccessStatusCode)
                    {
                        var vendor = response0.Content.ReadAsStringAsync().Result;
                        structuredResponse = JsonConvert.DeserializeObject<EdmsVendorInfoResponse>(vendor);

                        vendorInfo = FillVendorData(vendorUserName, domainId, structuredResponse);
                    }
                    return Task.FromResult(vendorInfo);

                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        Contact FillVendorData(string vendorUserName,int domainId, EdmsVendorInfoResponse edmsVendorInfo) {


            var vendorProject = Project.Create(edmsVendorInfo.ProjectInfo.Description, edmsVendorInfo.ProjectInfo.Description, edmsVendorInfo.ProjectInfo.No, edmsVendorInfo.ProjectInfo.Pro_Id.ToString());//, edmsVendorInfo.ProjectInfo.Pro_Id.ToString());

            Infrastructure.EDMS.Entities.edmsVendorDocuments edmsVendorDocument;
            List<Order> orders = new List<Order>();            
            for (int i = 0; i < edmsVendorInfo.ProjectInfo.OrderInfo.Length; i++)
            {
                var order = Order.Create(edmsVendorInfo.ProjectInfo.OrderInfo[i].No, edmsVendorInfo.ProjectInfo.OrderInfo[i].No, edmsVendorInfo.ProjectInfo.OrderInfo[i].No, vendorProject, edmsVendorInfo.ProjectInfo.OrderInfo[i].Dot_Id.ToString());
                for (int v = 0; v < edmsVendorInfo.ProjectInfo.OrderInfo[i].VendorDocuments.Length; v++)
                {
                    edmsVendorDocument= edmsVendorInfo.ProjectInfo.OrderInfo[i].VendorDocuments[v];
                    var vDocument = VendorDocument.Create(edmsVendorDocument.Title, edmsVendorDocument.No, null,int.Parse(edmsVendorDocument.Version),edmsVendorDocument.Doc_Id.ToString(),edmsVendorDocument.PDFSize);
                    order.AddVendorDocument(vDocument);

                }                
                orders.Add(order);                
            }

            var vendorCP = Contact.Create(edmsVendorInfo.UserInfo.SAMAccountName, null, "", "", "", "", "", "", "", 2, true);//, domainId,vendorProject,orders);
           

            vendorCP.Orders = GenerateOrderContacts(orders,vendorCP);
           // Contact contact = new Contact();
            return vendorCP;
        }


        public List<OrderContact> GenerateOrderContacts(List<Order> orders, Contact contact)
        { 
            List<OrderContact> orderContacts = new List<OrderContact>();
            OrderContact orderContact;

            for (int i = 0; i < orders.Count; i++)
            {
                orderContact = new OrderContact();
                orderContact.Contact= contact;
                orderContact.ContactId = contact.ContactId;
                orderContact.Order  = orders[i];
                orderContact.OrderId = orders[i].OrderId;
                orderContacts.Add(orderContact);
            }
            return orderContacts;
        }
        public Task<bool> UpdateDocumentToStateInuseInEDMS(string DocumentId, bool isUsed)
        {
            throw new NotImplementedException();
        }

        public Task<SystematicUser> UserIsValid(string userName, string password, int domainId)
        {

            //var query = "{\r\n    \"username\": \"akbari.elahe\",\r\n    \"password\": \"79500Mordad\",\r\n    \"dom_Id\": 3,\r\n    \"code\":\"7eau2cfp-1ays-3718-65h1-a12417628bnp\"\r\n}";
            //var query = "{\r\n    \"username\": \"" + userName + "\",\r\n    \"password\": \"" + password + "\",\r\n    \"dom_Id\": " + domainId + ",\r\n    \"code\":\"7eau2cfp-1ays-3718-65h1-a12417628bnp\"\r\n}";
            var query = "{\r\n    \"username\": \"" + userName + "\",\r\n    \"password\": \"" + password + "\",\r\n    \"dom_Id\": " + 1 + ",\r\n    \"code\":\"7eau2cfp-1ays-3718-65h1-a12417628bnp\"\r\n}";
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_edmsServiceSettings.APIAddress);
                    var content0 = new StringContent(query, Encoding.UTF8, "application/json");

                    HttpResponseMessage response0 = client.PostAsync("IsAuthenticated", content0).Result;

                    EdmsUserValidationResponseStruct edmsUserValidationResponseStruct = new EdmsUserValidationResponseStruct();


                    if (response0.IsSuccessStatusCode)
                    {
                        var response = response0.Content.ReadAsStringAsync().Result;
                        edmsUserValidationResponseStruct = JsonConvert.DeserializeObject<EdmsUserValidationResponseStruct>(response);
                    }

                    SystematicUser userResult = new SystematicUser();
                    if (edmsUserValidationResponseStruct.message != null && edmsUserValidationResponseStruct.errors.Length == 0)
                    {
                        userResult.IsAuthenticated = true;
                    }
                    else
                    {
                        userResult.IsAuthenticated = false;
                    }

                    userResult.UserName = userName;
                    userResult.DomainId = domainId;

                    if (edmsUserValidationResponseStruct.data != null)
                    {
                        userResult.UserName = edmsUserValidationResponseStruct.data.SAMAccountName;
                        userResult.Use_Id = edmsUserValidationResponseStruct.data.Use_Id;
                        userResult.IsActive = edmsUserValidationResponseStruct.data!.IsActive;

                        if (edmsUserValidationResponseStruct.data.IsVendor)
                            userResult.UserType = UserType.Vendor.GetHashCode();
                        else if (edmsUserValidationResponseStruct.data.IsPIDEC)
                            userResult.UserType = UserType.PIDEC.GetHashCode();
                        else if (edmsUserValidationResponseStruct.data.IsOwner)
                            userResult.UserType = UserType.Owner.GetHashCode();
                        else if (edmsUserValidationResponseStruct.data.IsMC)
                            userResult.UserType = UserType.MC.GetHashCode();
                        else
                            userResult.UserType = UserType.UndefinedUser.GetHashCode();

                    }
                    

                    return Task.FromResult<SystematicUser>(userResult);
                }
            }
            catch (Exception exp)
            {
                return null;
            }
        }


        public Task<List<EDMSOrder>> GetEdmsOrders_ByProId(string projectId)
        {
            //var query = "{\r\n    \"pro_Id\": \"69\",\r\n    \"code\":\"7eau2cfp-1ays-3718-65h1-a12417628bnp\"\r\n}";
            var query = "{\r\n    \"pro_Id\": \""+projectId+"\",\r\n    \"code\":\"7eau2cfp-1ays-3718-65h1-a12417628bnp\"\r\n}";
            

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_edmsServiceSettings.APIAddress);
                    var content0 = new StringContent(query, Encoding.UTF8, "application/json");

                    HttpResponseMessage response0 = client.PostAsync("GetOrders_ByProId", content0).Result;


                    List<edmsOrderInfoResponse> structuredResponse = new List<edmsOrderInfoResponse>();


                    edmsOrderInfoResponse ordersInfo = new edmsOrderInfoResponse();
                    

                    if (response0.IsSuccessStatusCode)
                    {
                        var orders = response0.Content.ReadAsStringAsync().Result;

                        var rootObject = JsonConvert.DeserializeObject<Dictionary<string, List<EDMSOrder>>>(orders);

                        // Access the List<edmsOrderInfoResponse> from the "OrderInfo" key
                       List<EDMSOrder> orderInfoList = rootObject["OrderInfo"];

                        // structuredResponse = JsonConvert.DeserializeObject<List<edmsOrderInfoResponse>>(orders);

                        return Task.FromResult(orderInfoList);
                    }
                    //return Task.FromResult(orderInfoList);
                    return null;

                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        public Task<List<Order>> GetProjectOrders(string projectIds)
        {
            //return _edmsService.GetProjectOrders(projectId);
            Project newPrj = Project.Create("EDMS Project1", "assume this is received from EDMS", "1237_Project_prjNumber", "1");
            Order newOrder1 = Order.Create("Edms Order1", "Edms Order 1", "ORD_1237_001", newPrj, "1");
            Order newOrder2 = Order.Create("Edms Order2", "Edms Order 2", "ORD_1237_002", newPrj, "2");
            Order newOrder3 = Order.Create("Edms Order3", "Edms Order 3", "ORD_1237_003", newPrj, "3");
            Order newOrder4 = Order.Create("Edms Order4", "Edms Order 4", "ORD_1237_004", newPrj, "4");

            List<Order> orders = new List<Order>();
            orders.Add(newOrder1);
            orders.Add(newOrder2);
            orders.Add(newOrder3);
            orders.Add(newOrder4);

            return Task.FromResult(orders);
        }

        public Task<List<Order>> GetProjectOrders(string[] projectIds)
        {
            Project newPrj = Project.Create("EDMS Project1", "assume this is received from EDMS", "1237_Project_prjNumber", "1");
            Order newOrder1 = Order.Create("Edms Order1", "Edms Order 1", "ORD_1237_001", newPrj, "1");
            Order newOrder2 = Order.Create("Edms Order2", "Edms Order 2", "ORD_1237_002", newPrj, "2");
            Order newOrder3 = Order.Create("Edms Order3", "Edms Order 3", "ORD_1237_003", newPrj, "3");
            Order newOrder4 = Order.Create("Edms Order4", "Edms Order 4", "ORD_1237_004", newPrj, "4");

            List<Order> orders = new List<Order>();
            orders.Add(newOrder1);
            orders.Add(newOrder2);
            orders.Add(newOrder3);
            orders.Add(newOrder4);

            return Task.FromResult(orders);
        }

        public Task<EDMSOrderDepartmentLeads> GetOrderDepartmentLeads(string orderDotId, string projectId)
        {
            //var query = "{\r\n    \"pro_Id\": \"69\",\r\n    \"code\":\"7eau2cfp-1ays-3718-65h1-a12417628bnp\"\r\n}";
            var query = "{\r\n    \"pro_Id\": \"" + projectId + "\",\r\n   \"dot_Id\": \""+orderDotId+ "\",\r\n    \"code\":\"7eau2cfp-1ays-3718-65h1-a12417628bnp\"\r\n}";


            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_edmsServiceSettings.APIAddress);
                    var content0 = new StringContent(query, Encoding.UTF8, "application/json");

                    HttpResponseMessage response0 = client.PostAsync("GetOrderInfo_ByDotId_ProId", content0).Result;


                    List<edmsOrderInfoResponse> structuredResponse = new List<edmsOrderInfoResponse>();


                    edmsOrderInfoResponse ordersInfo = new edmsOrderInfoResponse();


                    if (response0.IsSuccessStatusCode)
                    {
                        var DeptLeads = response0.Content.ReadAsStringAsync().Result;

                        var rootObject = JsonConvert.DeserializeObject<Dictionary<string, EDMSOrderDepartmentLeads>>(DeptLeads);

                        // Access the List<edmsOrderInfoResponse> from the "OrderInfo" key
                        EDMSOrderDepartmentLeads orderInfoList = rootObject["orderInfo"];

                        // structuredResponse = JsonConvert.DeserializeObject<List<edmsOrderInfoResponse>>(orders);

                        return Task.FromResult(orderInfoList);
                    }
                    //return Task.FromResult(orderInfoList);
                    return null;

                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }


        //var values = new Dictionary<string, string> { { "username", userName }, { "dom_Id", domainId.ToString() }, { "code", _edmsServiceSettings.SecurityCode } };
        //var content = new FormUrlEncodedContent(values);


        //HttpResponseMessage response = _httpClient.PostAsJsonAsync("IsAuthenticated", content).Result;

        //if (response.IsSuccessStatusCode)
        //{
        //    var VendorIsValids = response.Content.ReadAsStringAsync().Result;
        //}
        //return Task.FromResult(VendorIsValid);

    }
}
