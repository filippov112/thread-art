namespace Infrastructure.QueueManager.Settings;

public class ProcessingOptions
{
    public const string SectionName = "Processing";
    public int MaxConcurrency { get; set; }
}
