using Domain.Models;

namespace Application.UseCases;

public class ProcessImageRequest
{
    public Stream ImageStream { get; set; } = null!;
    public string FileName { get; set; } = string.Empty;

    public string Directory { get; set; } = string.Empty;
    public Config Config { get; set; } = new();
}
