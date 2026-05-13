using Domain.Enums;

namespace Domain.Models;

public class ProcessingJob
{
    // Parameters
    public required string FileName { get; set; }
    public required string OriginalSystemPath { get; set; }
    public required string OriginalWebPath { get; set; }
    public int CountPoints { get; set; } = 240;
    public int CountSteps { get; set; } = 4000;
    public int Padding { get; set; } = 10;

    // Results
    public string? ResultImagePath { get; set; }
    public string? ResultRoutePath { get; set; }

    // Metadata
    public Guid Id { get; set; } = Guid.NewGuid();
    public JobStatus Status { get; set; } = JobStatus.Pending;
    public int Progress { get; set; } = 0;
    public string? ErrorMessage { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; } = null;
}
