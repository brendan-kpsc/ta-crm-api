using MySqlConnector;
using TaCrm.Common.Handlers;
using TaCrm.Common.Repositories;

namespace ta_crm_api.Extensions;

public static class WebAppBuilderExtensions
{
    extension(WebApplicationBuilder builder)
    {
        public void InitializeEnvironmentVariables()
        {
            EnvironmentVariables.Initialize(builder.Configuration);
        }

        public void AddDependencies()
        {
            builder.Services.AddMySqlDataSource(EnvironmentVariables.MySqlConnectionString);
            
            builder.Services.AddScoped<BookingUnitTypeRepository>();
            builder.Services.AddScoped<BookingPeriodRepository>();
            
            builder.Services.AddScoped<BookingUnitTypeHandler>();
            builder.Services.AddScoped<BookingPeriodHandler>();
        }
    }
}