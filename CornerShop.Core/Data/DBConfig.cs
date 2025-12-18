namespace CornerShop.Core.Data
{
    public static class DBConfig
    {
        public static bool UseCloud { get; set; }=true;

        public const  string LocalConnection= "mongodb://localhost:27017";
        private const string CloudConnection = "mongodb+srv:";// TODO: Enter your MongoDB Atlas connection string here
        public static string ConnectionString
        {
            get
            {
                return UseCloud ? CloudConnection : LocalConnection;
            }
        }
        // Base URL for API calls. 
        // NOTE: To view Swagger UI in browser, please append "/swagger/index.html" to this URL.
        public const string AdminApiUrl = "https://localhost:7099/";
    }
}
