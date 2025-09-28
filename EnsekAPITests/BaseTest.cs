using Microsoft.Playwright;
using NUnit.Framework;
using EnsekAPITests.Helpers;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace EnsekAPITests
{
    [SetUpFixture]
    public class BaseTest
    {
        public static IPlaywright PlaywrightInstance { get; private set; }
        public static IAPIRequestContext RequestContext { get; private set; }
        public static ApiClient ApiClient { get; private set; }
        public static string BaseUrl => ConfigReader.BaseUrl;

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            PlaywrightInstance = await Playwright.CreateAsync();
            RequestContext = await PlaywrightInstance.APIRequest.NewContextAsync(new APIRequestNewContextOptions
            {
                BaseURL = BaseUrl,
                ExtraHTTPHeaders = new Dictionary<string, string> { { "Accept", "application/json" } }
            });

            ApiClient = new ApiClient(RequestContext);

            // Login once globally
            var payload = new
            {
                username = ConfigReader.CredentialsUsername,
                password = ConfigReader.CredentialsPassword
            };

            var response = await ApiClient.PostAsync(ConfigReader.LoginEndpoint, payload);
            Assert.That(response.Status, Is.EqualTo(200), "Login failed during global setup");

            var json = JObject.Parse(await response.TextAsync());
            AuthHelper.BearerToken = json["access_token"]?.ToString();
            Assert.IsNotNull(AuthHelper.BearerToken, "Bearer token should not be null");

            TestContext.WriteLine($"Login successful. Token set globally: {AuthHelper.BearerToken.Substring(0, 10)}...");
        }

        [OneTimeTearDown]
        public async Task GlobalTeardown()
        {
            await RequestContext.DisposeAsync();
            PlaywrightInstance.Dispose();
            TestContext.WriteLine("Global teardown complete");
        }
    }
}
