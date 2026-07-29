using Core.QueueManager.Models;

namespace Core.QueueManager.DTO;

public class JobDto : ProcessingJob
{
    public JobDto(ProcessingJob job)
    {
        // Parameters
        FileName = job.FileName;
        OriginalSystemPath = job.OriginalSystemPath;
        CountPoints = job.CountPoints;
        CountSteps = job.CountSteps;
        Padding = job.Padding;

        // Results
        ResultImagePath = job.ResultImagePath;
        ResultRoutePath = job.ResultRoutePath;

        // Metadata
        Id = job.Id;
        Status = job.Status;
        Progress = job.Progress;
        ErrorMessage = job.ErrorMessage;
        CreatedAt = job.CreatedAt;
        CompletedAt = job.CompletedAt;
    }

}
