using Auction.Data;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net.Http.Formatting;
using System.IO;
using System.Reflection;

namespace Auction.Desktop.Persistence
{
    public class AuctionServicePersistence : IAuctionPersistence
    { 
        private ILogger _log;
        private HttpClient _client;
        
        public AuctionServicePersistence(String baseAddress)
        {
            _client = new HttpClient(); 
            _client.BaseAddress = new Uri(baseAddress);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _log = LogManager.GetLogger("service"); 
        }

        public AuctionServicePersistence(String baseAddress, HttpMessageHandler handler)
        {
            _client = new HttpClient(handler); 
            _client.BaseAddress = new Uri(baseAddress); 

            _log = LogManager.GetLogger("service"); 
        }

        public async Task<IEnumerable<ObjectDTO>> ReadObjectsAsync()
        {
            try
            {
                _log.Info("GET query on service " + _client.BaseAddress + ", path: api/auctionobject/"); 

                HttpResponseMessage response = await _client.GetAsync("api/auctionobject/");
                if (response.IsSuccessStatusCode) 
                {
                    IEnumerable<ObjectDTO> objects = await response.Content.ReadAsAsync<IEnumerable<ObjectDTO>>();

                    return objects;
                }
                else
                {
                    _log.Warn("GET query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase); 

                    throw new PersistenceUnavailableException("Service returned response: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "GET query aborted with exception."); 

                throw new PersistenceUnavailableException(ex);
            }

        }
        
        public async Task<IEnumerable<CategoryDTO>> ReadCategoriesAsync()
        {
            try
            {
                _log.Info("GET query on service " + _client.BaseAddress + ", path: api/category/");

                HttpResponseMessage response = await _client.GetAsync("api/category/");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<IEnumerable<CategoryDTO>>();
                }
                else
                {
                    _log.Warn("GET query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                    throw new PersistenceUnavailableException("Service returned response: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "GET query aborted with exception.");

                throw new PersistenceUnavailableException(ex);
            }
        }

        public async Task<IEnumerable<BiddingDTO>> ReadBiddingsAsync(ObjectDTO auctionObject)
        {
            try
            {
                Int32 auctionObjectId = auctionObject.Id;

                _log.Info("GET query on service " + _client.BaseAddress + ", path: api/bidding/getbiddings/" + auctionObjectId.ToString());

                HttpResponseMessage response = await _client.GetAsync("api/bidding/getbiddings/" + auctionObjectId.ToString());
                if (response.IsSuccessStatusCode)
                {
                    IEnumerable<BiddingDTO> biddings = await response.Content.ReadAsAsync<IEnumerable<BiddingDTO>>();

                    return biddings;
                }
                else
                {
                    _log.Warn("GET query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                    throw new PersistenceUnavailableException("Service returned response: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "GET query aborted with exception.");

                throw new PersistenceUnavailableException(ex);
            }
        }


        public async Task<Boolean> CreateObjectAsync(ObjectDTO auctionObject)
        {
            try
            {
                _log.Info("POST query on service " + _client.BaseAddress + ", path: api/auctionobject/");
                _log.Trace("POST query content: " + JsonConvert.SerializeObject(auctionObject, Formatting.None));

                HttpResponseMessage response = await _client.PostAsJsonAsync("api/auctionobject/", auctionObject); 
                auctionObject.Id = (await response.Content.ReadAsAsync<ObjectDTO>()).Id; 

                if (!response.IsSuccessStatusCode)
                    _log.Warn("POST query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "POST query aborted with exception.");

                throw new PersistenceUnavailableException(ex);
            }
        }
        public async Task<Boolean> CloseObjectAsync(ObjectDTO auctionObject)
        {
            try
            {
                _log.Info("PUT query on service " + _client.BaseAddress + ", path: api/auctionobject/");
                _log.Trace("PUT query content: " + JsonConvert.SerializeObject(auctionObject, Formatting.None));

                HttpResponseMessage response = await _client.PutAsJsonAsync("api/auctionobject/", auctionObject);

                if (!response.IsSuccessStatusCode)
                    _log.Warn("PUT query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "PUT query aborted with exception.");

                throw new PersistenceUnavailableException(ex);
            }
        }
        public async Task<Boolean> CreateImageAsync(ImageDTO image)
        {
            try
            {
                _log.Info("POST query on service " + _client.BaseAddress + ", path: api/objectimage");
                _log.Trace("POST query imageID: " + image.Id + ", buildingID: " + image.ObjectId);

                HttpResponseMessage response = await _client.PostAsJsonAsync("api/objectimage/", image); 
                if (response.IsSuccessStatusCode)
                {
                    image.Id = await response.Content.ReadAsAsync<Int32>(); 
                }
                else
                {
                    _log.Warn("POST query returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "POST query aborted with exception.");

                throw new PersistenceUnavailableException(ex);
            }
        }

        public async Task<Boolean> LoginAsync(String userName, String userPassword)
        {
            try
            {
                _log.Info("LOGIN on service {0}, path: api/account/login/, user name: {1}", _client.BaseAddress, userName);

                HttpResponseMessage response = await _client.GetAsync("api/account/login/" + userName + "/" + userPassword);

                if (!response.IsSuccessStatusCode)
                    _log.Warn("LOGIN returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                return response.IsSuccessStatusCode; 
            }
            catch (Exception ex)
            {
                _log.Error(ex, "LOGIN aborted with exception.");

                throw new PersistenceUnavailableException(ex);
            }
        }
        
        public async Task<Boolean> LogoutAsync()
        {
            try
            {
                _log.Info("LOGOUT on service " + _client.BaseAddress + ", path: api/account/logout/");

                HttpResponseMessage response = await _client.GetAsync("api/account/logout");

                if (!response.IsSuccessStatusCode)
                    _log.Warn("LOGOUT returned response {0} with reason: {1}", response.StatusCode, response.ReasonPhrase);

                return !response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "LOGOUT aborted with exception.");

                throw new PersistenceUnavailableException(ex);
            }
        }
    }
}

