using System.ComponentModel.DataAnnotations;

namespace Microservice.HelloWorld;

public class Appsettings
{
    public Appsettings()
    {
    }

    public Appsettings(IConfiguration configuration)
    {
        configuration.Bind(this);
    }

    public AppsettingsPostgres Postgres { get; set; }

}

public class AppsettingsPostgres
{
    public string MigrationConnectionString { get; set; }

    public string AppConnectionString { get; set; }

    public string MigrationIdentity { get; set; }

    public string ApiIdentity { get; set; }
}


