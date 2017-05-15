using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;
using Auction.Data;
using Auction.Desktop;
using Auction.Service;
using Auction.WebSite;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void calculate()
        {
            int a = 5;
            a++;
            Assert.AreEqual(6, a);
        }

        /*private HttpClient _client;

        [TestInitialize]
        public void Initialize()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://localhost:57223/");
            _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        [TestMethod]
        public async Task LoginInvalid()
        {
            HttpResponseMessage response = await _client.GetAsync("api/auctionobject/admin/wrongpassword");

            Assert.AreEqual(false, response.IsSuccessStatusCode);
        }

        [TestMethod]
        public async Task LoginValid()
        {
            HttpResponseMessage response = await _client.GetAsync("api/auctionobject/admin/alma1234");

            Assert.AreEqual(true, response.IsSuccessStatusCode);
        }*/
    }
}


