namespace API.Services
{
    public class UserDatabaseService
    {
        private string? _connectionString;

        public void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string? GetConnectionString()
        {
            return _connectionString;
        }
    }

}
