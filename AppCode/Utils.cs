namespace Headlight.AppCode
{
    public class Utils
    {
        public static string GetDbConnectionString(string host, string port, string database, string username, string password)
        => string.Format("Host={0};Port={1};Database={2};Username={3};Password={4}", host, port, database, username, password);
    }
}
