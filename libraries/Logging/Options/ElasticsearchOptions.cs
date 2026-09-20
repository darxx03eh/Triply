namespace Logging.Options;

/// <summary>Configuration options of Elasticsearch.</summary>
public class ElasticsearchOptions
{
    /// <summary>Gets or sets the enabled.</summary>
    public bool Enabled { get; set; } = true;
    /// <summary>Gets or sets the uri.</summary>
    public string Uri { get; set; } = "http://localhost:9200";
    /// <summary>Logs are written to the data stream logs-triply.{service}-{environment}.</summary>
    public string DataSetPrefix { get; set; } = "triply";
}
