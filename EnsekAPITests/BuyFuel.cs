using EnsekAPITests.Helpers;
using Microsoft.Playwright;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnsekAPITests
{
    [TestFixture]
    public class BuyFuelTests : BaseTest
    {
        [Test, Order(1)]
        public async Task BuyFuelWithOrderCheck()
        {
            Assert.IsNotNull(AuthHelper.BearerToken, "Login token is required before buying fuel");

            // Fuel quantities from config
            var fuelQuantities = new Dictionary<string, int>
            {
                { "gas", ConfigReader.GasQuantity },
                { "electric", ConfigReader.ElectricQuantity },
                { "oil", ConfigReader.OilQuantity },
                { "nuclear", ConfigReader.NuclearQuantity }
            };

            // Fetch energy types (GET) to get their IDs
            var energyResponse = await RequestContext.GetAsync(ConfigReader.BuyFuelEndpoint,
                new APIRequestContextOptions
                {
                    Headers = new Dictionary<string, string>
                    {
                        { "Authorization", $"Bearer {AuthHelper.BearerToken}" },
                        { "Accept", "application/json" }
                    }
                });

            Assert.That(energyResponse.Status, Is.EqualTo(200), "Failed to fetch energy types");
            var energyJson = JObject.Parse(await energyResponse.TextAsync());

            foreach (var fuel in fuelQuantities)
            {
                string fuelName = fuel.Key;
                int quantity = fuel.Value;
                int energyId = (int)energyJson[fuelName]["energy_id"];
                string unitType = energyJson[fuelName]["unit_type"].ToString();

                // PUT call to purchase
                var buyResponse = await RequestContext.PutAsync($"/ENSEK/buy/{energyId}/{quantity}",
                    new APIRequestContextOptions
                    {
                        Headers = new Dictionary<string, string>
                        {
                            { "Authorization", $"Bearer {AuthHelper.BearerToken}" },
                            { "Accept", "application/json" }
                        }
                    });

                var buyJson = JObject.Parse(await buyResponse.TextAsync());
                string message = buyJson["message"]?.ToString();

                if (buyResponse.Status == 200)
                    TestContext.WriteLine($"Purchased {quantity} {unitType} of {fuelName}. Message: {message}");
                else
                    TestContext.WriteLine($"Failed to purchase {fuelName}. Status: {buyResponse.Status}, Message: {message}");

                // Fetch orders after each purchase
                var ordersResponse = await RequestContext.GetAsync(ConfigReader.OrdersEndpoint,
                    new APIRequestContextOptions
                    {
                        Headers = new Dictionary<string, string>
                        {
                            { "Authorization", $"Bearer {AuthHelper.BearerToken}" },
                            { "Accept", "application/json" }
                        }
                    });

                var ordersJson = JArray.Parse(await ordersResponse.TextAsync());
                int totalOrders = ordersJson.Count;
                var today = DateTime.UtcNow.Date;
                int olderOrders = ordersJson.Count(o => DateTime.Parse(o["time"].ToString()).Date < today);

                TestContext.WriteLine($"After purchasing {fuelName}: Total Orders = {totalOrders}, Orders before today = {olderOrders}");
            }
        }
    }
}
