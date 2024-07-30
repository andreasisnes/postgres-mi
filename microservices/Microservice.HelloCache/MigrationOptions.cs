namespace Microservice.HelloCache;

public class MigrationOptions
{
    /// <summary>
    /// 
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool UseManagedIdentity { get; set; } = false;

    /// <summary>
    /// 
    /// </summary>
    public string MigrationClientId { get; set; }
}
