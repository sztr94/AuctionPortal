using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Auction.Data;
using Auction.Desktop;
using Auction.Service;
using System.Linq;
using System.Collections.Generic;
using Auction.Service.Models;
using Auction.Desktop.Persistence;
using Microsoft.Practices.Unity;
using Moq;
using System.Data.Entity;
using System.Web.Http;
using Auction.Service.Controllers;
using System.Web.Http.Results;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Owin.Host.SystemWeb;
using System.Net.Http;
using System.Text;

namespace UnitTestProject
{
    [TestClass]
    public class WebServiceUnitTest
    {
        private List<AuctionAdvertiser> _advertiserData;
        private List<AuctionGuest> _guestData;
        private List<AuctionObject> _objectData;
         private List<AuctionCategory> _categoryData;
         private List<Bidding> _biddingData;
        private Mock<DbSet<AuctionAdvertiser>> _advertiserMock;
        private Mock<DbSet<AuctionGuest>> _guestMock;
        private Mock<DbSet<AuctionObject>> _objectMock;
         private Mock<DbSet<AuctionCategory>> _categoryMock;
         private Mock<DbSet<Bidding>> _biddingMock;
         private Mock<IAuctionEntities> _entityMock;
         private HttpServer _server;
         private AuctionObjectController objectController;
         private AccountController accountController;

         [TestInitialize]
         public void Initialize()
         {
            _advertiserData = new List<AuctionAdvertiser>
            {
                new AuctionAdvertiser { Name = "TESTADVERTISERNAME", UserName = "admin", UserPassword = Encoding.ASCII.GetBytes("alma1234") }
            };
            _guestData = new List<AuctionGuest>
            {
                new AuctionGuest { Name = "TESTGUESTNAME", UserName = "TESTGUEST1", UserPassword = Encoding.ASCII.GetBytes("alma1234"), Email="TESZT@TESZT.COM", PhoneNumber = "12345678" }
            };
            _categoryData = new List<AuctionCategory>
             {
                 new AuctionCategory { Id = 1, Name = "TESTCATEGORY" }
             };

             _objectData = new List<AuctionObject>
             {
                 new AuctionObject { Id = 1, Name = "TESTOBJECT1", CategoryId = _categoryData[0].Id, Description = "TESTOBJECT1 DESCRIPTION", AdvertiserId = "admin", StartBiddingAmount = 10, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), ObjectImage = null },
                 new AuctionObject { Id = 2, Name = "TESTOBJECT2", CategoryId = _categoryData[0].Id, Description = "TESTOBJECT2 DESCRIPTION",  AdvertiserId = "admin", StartBiddingAmount = 20, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(2), ObjectImage = null },
                 new AuctionObject { Id = 3, Name = "TESTOBJECT3", CategoryId = _categoryData[0].Id, Description = "TESTOBJECT3 DESCRIPTION",  AdvertiserId = "admin", StartBiddingAmount = 30, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(3), ObjectImage = null }
             };

             _biddingData = new List<Bidding>
             {
                 new Bidding { Id = 1, ObjectId = 1, UserName = "TESTGUEST1", BiddingAmount = 10,  BiddingDate = DateTime.Now.AddSeconds(10) }
             };

            IQueryable<AuctionAdvertiser> queryableAdvertiserData = _advertiserData.AsQueryable();
            _advertiserMock = new Mock<DbSet<AuctionAdvertiser>>();
            _advertiserMock.As<IQueryable<AuctionAdvertiser>>().Setup(mock => mock.ElementType).Returns(queryableAdvertiserData.ElementType);
            _advertiserMock.As<IQueryable<AuctionAdvertiser>>().Setup(mock => mock.Expression).Returns(queryableAdvertiserData.Expression);
            _advertiserMock.As<IQueryable<AuctionAdvertiser>>().Setup(mock => mock.Provider).Returns(queryableAdvertiserData.Provider);
            _advertiserMock.As<IQueryable<AuctionAdvertiser>>().Setup(mock => mock.GetEnumerator()).Returns(_advertiserData.GetEnumerator());

            IQueryable<AuctionGuest> queryableGuestData = _guestData.AsQueryable();
            _guestMock = new Mock<DbSet<AuctionGuest>>();
            _guestMock.As<IQueryable<AuctionGuest>>().Setup(mock => mock.ElementType).Returns(queryableGuestData.ElementType);
            _guestMock.As<IQueryable<AuctionGuest>>().Setup(mock => mock.Expression).Returns(queryableGuestData.Expression);
            _guestMock.As<IQueryable<AuctionGuest>>().Setup(mock => mock.Provider).Returns(queryableGuestData.Provider);
            _guestMock.As<IQueryable<AuctionGuest>>().Setup(mock => mock.GetEnumerator()).Returns(_guestData.GetEnumerator());

            IQueryable<AuctionCategory> queryableCategoryData = _categoryData.AsQueryable();
             _categoryMock = new Mock<DbSet<AuctionCategory>>();
             _categoryMock.As<IQueryable<AuctionCategory>>().Setup(mock => mock.ElementType).Returns(queryableCategoryData.ElementType);
             _categoryMock.As<IQueryable<AuctionCategory>>().Setup(mock => mock.Expression).Returns(queryableCategoryData.Expression);
             _categoryMock.As<IQueryable<AuctionCategory>>().Setup(mock => mock.Provider).Returns(queryableCategoryData.Provider);
             _categoryMock.As<IQueryable<AuctionCategory>>().Setup(mock => mock.GetEnumerator()).Returns(_categoryData.GetEnumerator());

             IQueryable<AuctionObject> queryableObjectData = _objectData.AsQueryable();
             _objectMock = new Mock<DbSet<AuctionObject>>();
             _objectMock.As<IQueryable<AuctionObject>>().Setup(mock => mock.ElementType).Returns(queryableObjectData.ElementType);
             _objectMock.As<IQueryable<AuctionObject>>().Setup(mock => mock.Expression).Returns(queryableObjectData.Expression);
             _objectMock.As<IQueryable<AuctionObject>>().Setup(mock => mock.Provider).Returns(queryableObjectData.Provider);
             _objectMock.As<IQueryable<AuctionObject>>().Setup(mock => mock.GetEnumerator()).Returns(_objectData.GetEnumerator());

             _objectMock.Setup(mock => mock.Add(It.IsAny<AuctionObject>())).Callback<AuctionObject>(ao => { _objectData.Add(ao); }).Returns(_objectData.Last()); 

             _objectMock.Setup(mock => mock.Include(It.IsAny<String>())).Returns(_objectMock.Object); 

             IQueryable<Bidding> queryableBiddingData = _biddingData.AsQueryable();
             _biddingMock = new Mock<DbSet<Bidding>>();
             _biddingMock.As<IQueryable<Bidding>>().Setup(mock => mock.ElementType).Returns(queryableBiddingData.ElementType);
             _biddingMock.As<IQueryable<Bidding>>().Setup(mock => mock.Expression).Returns(queryableBiddingData.Expression);
             _biddingMock.As<IQueryable<Bidding>>().Setup(mock => mock.Provider).Returns(queryableBiddingData.Provider);
             _biddingMock.As<IQueryable<Bidding>>().Setup(mock => mock.GetEnumerator()).Returns(_biddingData.GetEnumerator());

             _entityMock = new Mock<IAuctionEntities>();
            _entityMock.Setup<DbSet<AuctionAdvertiser>>(entity => entity.AuctionAdvertiser).Returns(_advertiserMock.Object);
            _entityMock.Setup<DbSet<AuctionGuest>>(entity => entity.AuctionGuest).Returns(_guestMock.Object);
            _entityMock.Setup<DbSet<AuctionCategory>>(entity => entity.AuctionCategory).Returns(_categoryMock.Object);
             _entityMock.Setup<DbSet<AuctionObject>>(entity => entity.AuctionObject).Returns(_objectMock.Object);
             _entityMock.Setup<DbSet<Bidding>>(entity => entity.Bidding).Returns(_biddingMock.Object);

             HttpConfiguration config = new HttpConfiguration();
             WebApiConfig.Register(config);

             UnityContainer container = new UnityContainer();
             container.RegisterInstance<IAuctionEntities>(_entityMock.Object);

             config.DependencyResolver = new UnityResolver(container);

             _server = new HttpServer(config);

            accountController = new AccountController()
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            objectController = new AuctionObjectController(_entityMock.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
        }

         [TestCleanup]
         public void Cleanup()
         {
             _server.Dispose();
         }

        [TestMethod]
        public void LoginFailed()
        {
            accountController.Login("admin", "alma1234");
        }

         [TestMethod]
         public void ReadObjectsTest()
         {
            objectController.GetObjects();
            
    }
}
