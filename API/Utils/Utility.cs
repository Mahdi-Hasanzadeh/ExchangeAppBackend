namespace API.Utils
{
    public static class Utility
    {
        public static string HashPassword(this string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(this string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        public static string GenerateConnectionString(string databaseName)
        {
            string serverName = "YourServerName"; // Change to your SQL Server name
            string userId = "YourDBUser"; // Change to your SQL Server user
            string password = "YourDBPassword"; // Change to your SQL Server password

            return $"Server={serverName};Database={databaseName};User Id={userId};Password={password};TrustServerCertificate=True;";
        }

    }
}
