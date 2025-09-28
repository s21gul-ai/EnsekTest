using EnsekAPITests.Helpers;
using Microsoft.Playwright;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnsekAPITests
{
    [TestFixture]
    public class ValidateOrdersTests : BaseTest
    {
        [Test, Order(2)]
        public async Task VerifyOrdersList()
        {
            Assert.IsNotNull(AuthHelper.BearerToken, "Login token required");

            var ordersResponse = await RequestContext.GetAsync(ConfigReader.OrdersEndpoint,
                new APIRequestContextOptions
                {
                    Headers = new Dictionary<string, string>
                    {
                        { "Authorization", $"Bearer {AuthHelper.BearerToken}" },
                        { "Accept", "application/json" }
                    }
                });

            Assert.That(ordersResponse.Status, Is.EqualTo(200), "Failed to fetch orders list");

            var ordersJson = JArray.Parse(await ordersResponse.TextAsync());
            TestContext.WriteLine($"Total Orders = {ordersJson.Count}");
        }
    }
}
