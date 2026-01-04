namespace saas_template.Configuration;

public class AppSettings
{
    public string ApplicationName { get; set; } = "SaaS Template";
    public int MaxRetryAttempts { get; set; } = 3;
    public int RequestTimeoutSeconds { get; set; } = 30;
    public int CacheExpirationMinutes { get; set; } = 5;
    public bool EnableSwagger { get; set; } = true;
}

