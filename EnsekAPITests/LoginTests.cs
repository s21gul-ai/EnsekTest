using EnsekAPITests;
using EnsekAPITests.Helpers;
using Newtonsoft.Json.Linq;

[TestFixture]
public class LoginTests : BaseTest  // inherit BaseTest to use global RequestContext
{
    [Test, Order(1)]
    public async Task LoginValidation()
    {
        // Prepare payload
        var payload = new
        {
            username = ConfigReader.CredentialsUsername,
            password = ConfigReader.CredentialsPassword
        };

        // POST to login endpoint using global ApiClient
        var response = await BaseTest.ApiClient.PostAsync(ConfigReader.LoginEndpoint, payload);

        Assert.That(response.Status, Is.EqualTo(200), $"Login failed, status: {response.Status}");

        var json = JObject.Parse(await response.TextAsync());
        AuthHelper.BearerToken = json["access_token"]?.ToString();
        Assert.IsNotNull(AuthHelper.BearerToken, "Bearer token should not be null");

        TestContext.WriteLine($"Login successful! Bearer Token: {AuthHelper.BearerToken} (HTTP {response.Status})");
    }
}
