namespace ToDo.Data.Configurations
{
    public class DbConfiguration
    {
        public DbConfiguration(
            string server,
            string database
            )
        {
            Server = server;
            Database = database;
        }
        public string Server { get; set; }
        public string Database { get; set; }
        public string ConnectionString =>
            $"Server={Server};Database={Database};Trusted_Connection=True;Persist Security Info=True;TrustServerCertificate=true;Encrypt=True;";
    }
}
