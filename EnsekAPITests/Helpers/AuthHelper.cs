namespace EnsekAPITests.Helpers
{
    // Simple static class to hold the bearer token for shared access across tests
    public static class AuthHelper
    {
        public static string BearerToken { get; set; }
    }
}
