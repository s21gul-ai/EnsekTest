using System.IO;
using Newtonsoft.Json.Linq;

namespace EnsekAPITests.Helpers
{
    /// <summary>
    /// Reads configuration values from appsettings.json
    /// To avoid hardcoding URLs, endpoints, and credentials.
    /// </summary>
    public static class ConfigReader
    {
        private static readonly JObject _config;

        static ConfigReader()
        {
            var json = File.ReadAllText("appsettings.json");
            _config = JObject.Parse(json);
        }

        // Base URL
        public static string BaseUrl => _config["BaseUrl"]?.ToString();

        // Endpoints
        public static string LoginEndpoint => _config["Endpoints"]["Login"].ToString();
        public static string BuyFuelEndpoint => _config["Endpoints"]["BuyFuel"].ToString();
        public static string OrdersEndpoint => _config["Endpoints"]["Orders"].ToString();

        // Credentials
        public static string CredentialsUsername => _config["Credentials"]["Username"].ToString();
        public static string CredentialsPassword => _config["Credentials"]["Password"].ToString();

        // Quantities
        public static int GasQuantity => int.Parse(_config["Quantities"]["Gas"].ToString());
        public static int ElectricQuantity => int.Parse(_config["Quantities"]["Electric"].ToString());
        public static int OilQuantity => int.Parse(_config["Quantities"]["Oil"].ToString());
        public static int NuclearQuantity => int.Parse(_config["Quantities"]["Nuclear"].ToString());
    }
}
