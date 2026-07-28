namespace Domain.QueueManager.Services;

/// <summary>
/// Логгер прогресса долгих операций
/// </summary>
public class ProgressLogger
{
    public event Action<int>? ProgressUpdated;

    public void UpdateProgress(int val)
    {
        ProgressUpdated?.Invoke(val);
    }
}
