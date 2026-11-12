namespace ta_crm_api;

public static class EnvironmentVariables
{
    private static IConfiguration? Configuration;
    
    public static string MySqlConnectionString => 
        Configuration!.GetValue<string>("MYSQL_CONNECTION_STRING")!;

    public static void Initialize(IConfiguration configuration)
    {
        Configuration = configuration;
    }
}